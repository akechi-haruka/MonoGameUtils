using Haruka.Common;
using Haruka.Common.Configuration;
using Haruka.MonoGameUtils.UI.Elements;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;

namespace Haruka.MonoGameUtils.Input;

public class InputManager {
    public const string SECTION_INPUT = "Input";
    internal static ILogger inputLog;

    public List<Exception> InputAPIInitializationErrors { get; private set; } = new List<Exception>();
    public List<ICursorAPI> CursorInputs { get; private set; } = new List<ICursorAPI>();
    public List<ButtonInputAPI> ButtonInputs { get; private set; } = new List<ButtonInputAPI>();
    private readonly List<IInputAPI> inputs = new List<IInputAPI>();
    private GameWindow window;

    internal bool IsFocused { get; set; }
    internal bool AcceptForegroundInputOnly { get; }

    private int blockInputFrames = 0;

    public InputManager(IniFile configuration, GameWindow window, params IInputAPI[] customSystems) {
        inputLog = Log.GetOrCreate("Inpt");

        AcceptForegroundInputOnly = configuration.ReadBool("ForegroundInputOnly", SECTION_INPUT);

        this.window = window;
        List<IInputAPI> list = new List<IInputAPI> {
            new KeyboardInput(),
            new MouseInput(),
            new TouchInput(),
            new DirectInput(),
            new SerialInput(),
            new SoftInput()
        };

        foreach (IInputAPI input in list) {
            string n = input.GetType().Name;
            if (input is SoftInput || configuration.ReadBool(n, SECTION_INPUT, true)) {
                inputLog.LogInformation("Enabling input system: " + n);

                InitializeInputSystem(configuration, input);
            } else {
                inputLog.LogInformation("Input System disabled: " + n);
            }
        }
    }

    private void InitializeInputSystem(IniFile configuration, IInputAPI input) {
        try {
            input.Initialize();
        } catch (Exception ex) {
            InputAPIInitializationErrors.Add(ex);
            return;
        }

        if (input is ButtonInputAPI buttoninput) {
            foreach (Key key in Inputs.ALL_KEYS) {
                buttoninput.Bind(key, key.Name != null ? configuration.ReadString(key.Name, SECTION_INPUT, key.DefaultKeyboardKey).Split(',') : new string[] { key.DefaultKeyboardKey });
            }

            ButtonInputs.Add(buttoninput);
        } else if (input is ICursorAPI cursorinput) {
            CursorInputs.Add(cursorinput);
        }

        inputs.Add(input);
    }

    #region Key Input

    public T GetInput<T>() {
        foreach (IInputAPI input in inputs) {
            if (input is T) {
                return (T)input;
            }
        }

        return default;
    }

    public bool IsAnyPressed(params Key[] keys) {
        foreach (ButtonInputAPI api in ButtonInputs) {
            foreach (Key key in keys) {
                if (api.IsPressed(key)) {
                    return true;
                }
            }
        }

        return false;
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
        foreach (ButtonInputAPI api in ButtonInputs) {
            foreach (Key key in keys) {
                if (api.IsJustPressed(key)) {
                    return true;
                }
            }
        }

        return false;
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
        int val = 0;
        foreach (ButtonInputAPI api in ButtonInputs) {
            int val2 = api.GetKeyHoldTime(key);
            if (val2 > val) {
                val = val2;
            }
        }

        return val;
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
        foreach (IInputAPI api in inputs) {
            api.LateUpdate(new GameTime());
        }
    }


    public void SkipInputFrame() {
        blockInputFrames = 30; // todo hack
    }

    #endregion

    #region Mouse/Touch Input

    public bool IsInClickOrigin() {
        foreach (ICursorAPI api in CursorInputs) {
            if (api.IsPressed() && api.IsInClickOrigin()) {
                return true;
            }
        }

        return false;
    }

    public bool IsJustClicked(Rectangle rect) {
        foreach (ICursorAPI api in CursorInputs) {
            if (api.IsJustPressed() && rect.Contains(api.GetX(), api.GetY())) {
                return true;
            }
        }

        return false;
    }

    public bool IsJustClickReleased(Rectangle rect, bool checkMoved = false) {
        foreach (ICursorAPI api in CursorInputs) {
            if (api.IsJustReleased() && rect.Contains(api.GetX(), api.GetY()) && (!checkMoved || api.IsInClickOrigin())) {
                return true;
            }
        }

        return false;
    }

    public bool IsJustClicked(UIElement el) {
        return IsJustClicked(el.GetRect());
    }

    public bool IsJustClickReleased(UIElement el, bool checkMoved = false) {
        return IsJustClickReleased(el.GetRect(), checkMoved);
    }

    public bool IsClicked(Rectangle rect) {
        foreach (ICursorAPI api in CursorInputs) {
            if (api.IsPressed() && rect.Contains(api.GetX(), api.GetY())) {
                return true;
            }
        }

        return false;
    }

    public bool IsClicked(UIElement el) {
        return IsClicked(el.GetRect());
    }

    public bool IsClicked() {
        foreach (ICursorAPI api in CursorInputs) {
            if (api.IsPressed()) {
                return true;
            }
        }

        return false;
    }

    public int GetTouchX() {
        foreach (ICursorAPI api in CursorInputs) {
            int x = api.GetX();
            if (x != 0) {
                return x;
            }
        }

        return 0;
    }

    public int GetTouchY() {
        foreach (ICursorAPI api in CursorInputs) {
            int y = api.GetY();
            if (y != 0) {
                return y;
            }
        }

        return 0;
    }

    public Vector2 GetDragDistance() {
        foreach (ICursorAPI api in CursorInputs) {
            if (api.IsDragging()) {
                return api.GetDragDistance();
            }
        }

        return Vector2.Zero;
    }

    #endregion


    internal void EarlyUpdate(GameTime gameTime) {
        if ((!AcceptForegroundInputOnly || IsFocused) && blockInputFrames <= 0) {
            foreach (IInputAPI input in inputs) {
                input.EarlyUpdate(gameTime);
            }
        }
    }

    internal void LateUpdate(GameTime gameTime) {
        if (!AcceptForegroundInputOnly || IsFocused) {
            foreach (IInputAPI input in inputs) {
                input.LateUpdate(gameTime);
            }
        }

        if (blockInputFrames > 0) {
            blockInputFrames--;
        }
    }

    public IInputAPI[] GetInputAPIList() {
        return inputs.ToArray();
    }
}