#region Licence - LGPLv3
// ***********************************************************************
// Assembly         : MonoGameUI
// Author           : Thomas Christof
// Created          : 02-15-2016
//
// Last Modified By : Thomas Christof
// Last Modified On : 05-31-2016
// ***********************************************************************
// <copyright>
// Company: Indie-Dev
// Thomas Christof (c) 2015
// </copyright>
// <License>
// GNU LESSER GENERAL PUBLIC LICENSE
// </License>
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU Lesser General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
// ***********************************************************************
#endregion Licence - LGPLv3
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using OAS;
using OAS.BootSequence;
using OAS.Error;
using OAS.Plugins;
using OAS.UI.Elements;
using OAS.Util.Configuration;
using OAS.Util.Logging;
using SharpDX.Direct3D9;
using static Emgu.CV.VideoCapture;

namespace OAS.Input {
    
    public class InputManager {

        public const String TAG = nameof(InputManager);

        public List<ICursorAPI> CursorInputs { get; private set; } = new List<ICursorAPI>();
        public List<IButtonInputAPI> ButtonInputs { get; private set; } = new List<IButtonInputAPI>();
        private List<IInputAPI> inputs = new List<IInputAPI>();
        private GameWindow window;

        internal bool IsFocused { get; set; }
        internal bool AcceptForegroundInputOnly { get; }

        private int blockInputFrames = 0;

        public InputManager(IniFile configuration, GameWindow window, bool isErrorDisp) {

            AcceptForegroundInputOnly = configuration.ReadBool("ForegroundInputOnly", IniFile.SECTION_GENERAL, false);

            this.window = window;
            List<IInputAPI> list = new List<IInputAPI> {
                new Input.KeyboardInput(),
                new MouseInput(),
                new TouchInput(),
                new DirectInput(),
                new SerialInput(),
                new SoftInput()
            };

            foreach (PluginInfo p in Program.PluginMgr.GetLoadedPlugins()) {
                IInputAPI[] inputs = p.Plugin.GetInputs()?.ToArray() ?? new IInputAPI[0];
                list.AddRange(inputs);
            }

            foreach (IInputAPI input in list) {
                string n = input.GetType().Name;
                if (input is SoftInput || configuration.ReadBool(n, "Input", true)) {
                    Log.Write("Enabling input system: " + n, TAG);

                    try {
                        input.Initialize(isErrorDisp);
                    } catch (OASException ex) {
                        SystemDeviceScan.AddError(ex);
                    }

                    if (input is IButtonInputAPI buttoninput) {
                        foreach (Key key in Inputs.ALL_KEYS) {
                            buttoninput.Bind(key, key.Name != null ? configuration.ReadString(key.Name, "Input", key.DefaultKeyboardKey).Split(',') : new string[] { key.DefaultKeyboardKey });
                        }
                        ButtonInputs.Add(buttoninput);
                    } else if (input is ICursorAPI cursorinput) {
                        CursorInputs.Add(cursorinput);
                    }

                    inputs.Add(input);
                } else {
                    Log.Write("Input System disabled: " + n, TAG);
                }
            }
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
            foreach (IButtonInputAPI api in ButtonInputs) {
                foreach (Key key in keys) {
                    if (api.IsPressed(key)) {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool IsAllPressed(params Key[] keys) {
            foreach (IButtonInputAPI api in ButtonInputs) {
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
            foreach (IButtonInputAPI api in ButtonInputs) {
                foreach (Key key in keys) {
                    if (api.IsJustPressed(key)) {
                        return true;
                    }
                }
            }
            return false;
        }

        public void MonitorInput(params Key[] keys) {
            foreach (IButtonInputAPI api in ButtonInputs) {
                foreach (Key key in keys) {
                    api.AddMonitor(key);
                }
            }
        }

        public void RemoveInputMonitor(params Key[] keys) {
            foreach (IButtonInputAPI api in ButtonInputs) {
                foreach (Key key in keys) {
                    api.RemoveMonitor(key);
                }
            }
        }

        public int GetKeyPressDuration(Key key) {
            int val = 0;
            foreach (IButtonInputAPI api in ButtonInputs) {
                int val2 = api.GetKeyHoldTime(key);
                if (val2 > val) {
                    val = val2;
                }
            }
            return val;
        }

        public void ResetKeyPressDuration(Key key) {
            ResetKeyPressDuration(key, IButtonInputAPI.KEY_REPEAT_START_TIME);
        }

        public void ResetKeyPressDuration(Key key, int value = 0) {
            foreach (IButtonInputAPI api in ButtonInputs) {
                api.ResetKeyHoldTime(key, value);
            }
        }

        public bool IsKeyRepeated(Key key, bool reset = true) {
            bool b = false;
            if (GetKeyPressDuration(key) > IButtonInputAPI.KEY_HOLD_TIME_GLOBAL) {
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

        public IInputAPI[] GetInputAPIs() {
            return inputs.ToArray();
        }
    }

}