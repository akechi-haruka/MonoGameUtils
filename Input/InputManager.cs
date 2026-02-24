using Haruka.Common;
using Haruka.Common.Configuration;
using Haruka.MonoGameUtils.Input.Api;
using Haruka.MonoGameUtils.Input.Builtin;
using Haruka.MonoGameUtils.UI.Elements;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;

namespace Haruka.MonoGameUtils.Input;

public class InputManager {
    public const string SECTION_INPUT = "Input";
    internal static ILogger InputLog;

    public List<ICursorAPI> CursorInputs { get; } = new List<ICursorAPI>();
    public List<ButtonInputAPI> ButtonInputs { get; } = new List<ButtonInputAPI>();
    private readonly List<InputInstance> inputs = new List<InputInstance>();

    internal bool IsFocused { get; set; }
    internal bool AcceptForegroundInputOnly { get; }

    private int blockInputFrames;

    public InputManager(IniFile configuration, params IInputAPI[] customSystems) {
        InputLog = Log.GetOrCreate("Inpt");

        AcceptForegroundInputOnly = configuration.ReadBool("ForegroundInputOnly", SECTION_INPUT);

        List<IInputAPI> list = new List<IInputAPI> {
            new KeyboardInput(),
            new MouseInput(),
            new TouchInput(),
            new DirectInput(),
            new SerialInput(),
            new SoftInput()
        };
        list.AddRange(customSystems);

        foreach (IInputAPI input in list) {
            string n = input.GetType().Name;
            if (input is SoftInput || configuration.ReadBool(n, SECTION_INPUT, true)) {
                InputLog.LogInformation("Enabling input system: " + n);
                InitializeInput(configuration, input);
            } else {
                InputLog.LogInformation("Input system disabled: " + n);
            }
        }
    }

    private void InitializeInput(IniFile configuration, IInputAPI input) {
        input.Initialize();

        // TODO: input.start?
        
        if (input is ButtonInputAPI buttoninput) {
            foreach (Key key in Inputs.ALL_KEYS) {
                buttoninput.Bind(key, key.Name != null ? configuration.ReadString(key.Name, SECTION_INPUT, key.DefaultKeyboardKey).Split(',') : new string[] { key.DefaultKeyboardKey });
            }

            ButtonInputs.Add(buttoninput);
        } else if (input is ICursorAPI cursorinput) {
            CursorInputs.Add(cursorinput);
        }

        inputs.Add(new InputInstance(input));
    }

    #region Key Input

    public T GetInput<T>() {
        foreach (InputInstance input in inputs) {
            if (input.API is T api) {
                return api;
            }
        }

        return default;
    }

    public bool IsAnyPressed(params Key[] keys) {
        return ButtonInputs.Any(api => keys.Any(api.IsPressed));
    }

    public bool IsAllPressed(params Key[] keys) {
        foreach (ButtonInputAPI api in ButtonInputs) {
            bool all = true;
            foreach (Key key in keys) {
                if (!api.IsPressed(key)) {
                    all = false;
                }
            }

            if (all) {
                return true;
            }
        }

        return false;
    }

    public bool IsAnyJustPressed(params Key[] keys) {
        return ButtonInputs.Any(api => keys.Any(key => api.IsJustPressed(key)));
    }

    public void MonitorInput(params Key[] keys) {
        foreach (ButtonInputAPI api in ButtonInputs) {
            foreach (Key key in keys) {
                api.AddMonitor(key);
            }
        }
    }

    public void RemoveInputMonitor(params Key[] keys) {
        foreach (ButtonInputAPI api in ButtonInputs) {
            foreach (Key key in keys) {
                api.RemoveMonitor(key);
            }
        }
    }

    public int GetKeyPressDuration(Key key) {
        return ButtonInputs.Select(api => api.GetKeyHoldTime(key)).Prepend(0).Max();
    }

    public void ResetKeyPressDuration(Key key) {
        ResetKeyPressDuration(key, ButtonInputAPI.KeyRepeatStartTime);
    }

    public void ResetKeyPressDuration(Key key, int value) {
        foreach (ButtonInputAPI api in ButtonInputs) {
            api.ResetKeyHoldTime(key, value);
        }
    }

    public bool IsKeyRepeated(Key key, bool reset = true) {
        bool b = false;
        if (GetKeyPressDuration(key) > ButtonInputAPI.KeyHoldTimeGlobal) {
            b = true;
            if (reset) {
                ResetKeyPressDuration(key);
            }
        }

        return b;
    }

    public void ResetInputStates() {
        foreach (InputInstance input in inputs) {
            input.API.LateUpdate(new GameTime());
        }
    }

    public void SkipInputFrame() {
        blockInputFrames = 30; // todo hack
    }

    #endregion

    #region Mouse/Touch Input

    public bool IsInClickOrigin() {
        return CursorInputs.Any(api => api.IsPressed() && api.IsInClickOrigin());
    }

    public bool IsJustClicked(Rectangle rect) {
        return CursorInputs.Any(api => api.IsJustPressed() && rect.Contains(api.GetX(), api.GetY()));
    }

    public bool IsJustClickReleased(Rectangle rect, bool checkMoved = false) {
        return CursorInputs.Any(api => api.IsJustReleased() && rect.Contains(api.GetX(), api.GetY()) && (!checkMoved || api.IsInClickOrigin()));
    }

    public bool IsJustClicked(UIElement el) {
        return IsJustClicked(el.GetRect());
    }

    public bool IsJustClickReleased(UIElement el, bool checkMoved = false) {
        return IsJustClickReleased(el.GetRect(), checkMoved);
    }

    public bool IsClicked(Rectangle rect) {
        return CursorInputs.Any(api => api.IsPressed() && rect.Contains(api.GetX(), api.GetY()));
    }

    public bool IsClicked(UIElement el) {
        return IsClicked(el.GetRect());
    }

    public bool IsClicked() {
        return CursorInputs.Any(api => api.IsPressed());
    }

    public int GetTouchX() {
        return CursorInputs.Select(api => api.GetX()).FirstOrDefault(x => x != 0);
    }

    public int GetTouchY() {
        return CursorInputs.Select(api => api.GetY()).FirstOrDefault(y => y != 0);
    }

    public Vector2 GetDragDistance() {
        ICursorAPI input = CursorInputs.FirstOrDefault(api => api.IsDragging());
        return input?.GetDragDistance() ?? Vector2.Zero;
    }

    #endregion


    internal void EarlyUpdate(GameTime gameTime) {
        if ((AcceptForegroundInputOnly && !IsFocused) || blockInputFrames > 0) {
            return;
        }

        foreach (InputInstance input in inputs.Where(input => !input.HasError())) {
            try {
                input.API.EarlyUpdate(gameTime);
            } catch (InputException ex) {
                input.SetError(ex.Message, ex.InnerException);
            } catch (Exception ex) {
                input.SetError("Internal Error", ex);
            }
        }
    }

    internal void LateUpdate(GameTime gameTime) {
        if (!AcceptForegroundInputOnly || IsFocused) {
            
            foreach (InputInstance input in inputs.Where(input => !input.HasError())) {
                try {
                    input.API.LateUpdate(gameTime);
                } catch (InputException ex) {
                    input.SetError(ex.Message, ex.InnerException);
                } catch (Exception ex) {
                    input.SetError("Internal Error", ex);
                }
            }
            
        }

        if (blockInputFrames > 0) {
            blockInputFrames--;
        }
    }

    public InputInstance[] GetInputs() {
        return inputs.ToArray();
    }
}