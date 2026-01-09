using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using OAS.IO.Driver;
using OAS.Util;
using OAS.Util.CodeHelpers;
using OAS.Util.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAS.Input {
    public class SerialInput : IButtonInputAPI {

        public const string TAG = nameof(SerialInput);

        public enum Button {
            SerialService,
            SerialTest
        }

        private List<Button> buttons;
        private BijectiveDictionary<Key, Button> keys;
        private Dictionary<Button, bool> state_prev;
        private Dictionary<Button, bool> state_now;

        public SerialInput() {
            keys = new BijectiveDictionary<Key, Button>();
            buttons = Enum.GetValues(typeof(Button)).Cast<Button>().ToList();
            state_prev = new Dictionary<Button, bool>();
            foreach (Button kbkey in buttons) {
                state_prev[kbkey] = false;
            }
            state_now = state_prev;
        }

        public override void Bind(Key key, string[] bindings) {
            foreach (string binding in bindings) {
                if (!Enum.TryParse(binding, out Button k)) {
                    Log.WriteWarning("Failed to parse key: " + binding, TAG);
                } else {
                    keys.Add(key, k);
                }
            }
        }

        public override bool IsJustPressed(Key key) {
            var list = keys.GetForward(key);
            if (list == null) { return false; }
            foreach (Button k in list) {
                if (state_now[k] && !state_prev[k]) {
                    return true;
                }
            }
            return false;
        }

        public override bool IsPressed(Key key) {
            var list = keys.GetForward(key);
            if (list == null) { return false; }
            foreach (Button k in list) {
                if (state_now[k]) {
                    return true;
                }
            }
            return false;
        }

        public override bool IsReleased(Key key) {
            var list = keys.GetForward(key);
            if (list == null) { return false; }
            foreach (Button k in list) {
                if (!state_now[k]) {
                    return true;
                }
            }
            return false;
        }

        public List<Button> GetAllPressedKeys() {
            List<Button> list = new List<Button>();
            foreach (Button kbkey in state_now.Keys) {
                if (state_now[kbkey]) {
                    list.Add(kbkey);
                }
            }
            return list;
        }

        public override void EarlyKeyUpdate(GameTime time) {
            state_now = new Dictionary<Button, bool>();
            TestServiceDevice ti = Program.DeviceMgr.GetFirstDeviceOfType<TestServiceDevice>();
            if (ti != null) {
                foreach (Button kbkey in buttons) {
                    state_now[kbkey] = ti.GetState(kbkey - Button.SerialService);
                }
            } else {
                foreach (Button kbkey in buttons) {
                    state_now[kbkey] = false;
                }
            }
        }

        public override void LateUpdate(GameTime time) {
            state_prev = state_now;
        }

        public override void Initialize(bool isErrorDisp) {
        }

        public override Exception GetError() {
            return null;
        }

        public override DateTime? GetErrorTime() {
            return null;
        }

        public override void ResetError() {
        }
    }
}
