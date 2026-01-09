using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using OAS.Util;
using OAS.Util.CodeHelpers;
using OAS.Util.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAS.Input {
    public class KeyboardInput : IButtonInputAPI {

        public const string TAG = "KeyboardInput";

        public bool CapsLockEnabled => current.CapsLock;
        public bool NumLockEnabled => current.NumLock;

        private KeyboardState prev;
        private KeyboardState current;
        private BijectiveDictionary<Key, Keys> keys;
        
        public KeyboardInput() {
            prev = default;
            keys = new BijectiveDictionary<Key, Keys>();
        }

        public override void Bind(Key key, string[] bindings) {
            foreach (string binding in bindings) {
                if (!Enum.TryParse(binding, out Keys k)) {
                    Log.WriteWarning("Failed to parse key: " + binding, TAG);
                }
                keys.Add(key, k);
            }
        }

        public override bool IsJustPressed(Key key) {
            var list = keys.GetForward(key);
            foreach (Keys k in list) {
                if (current.IsKeyDown(k) && !prev.IsKeyDown(k)) {
                    return true;
                }
            }
            return false;
        }

        public override bool IsPressed(Key key) {
            var list = keys.GetForward(key);
            foreach (Keys k in list) {
                if (current.IsKeyDown(k)) {
                    return true;
                }
            }
            return false;
        }

        public bool IsPressed(Keys key) {
            if (current.IsKeyDown(key)) {
                return true;
            }
            return false;
        }

        public override bool IsReleased(Key key) {
            var list = keys.GetForward(key);
            foreach (Keys k in list) {
                if (current.IsKeyUp(k)) {
                    return true;
                }
            }
            return false;
        }

        public List<Key> GetAllPressedKeys() {
            List<Key> list = new List<Key>();
            foreach (Keys kbkey in current.GetPressedKeys()) {
                var list2 = keys.GetReverse(kbkey);
                if (list2 != null) {
                    foreach (Key k in list2) {
                        list.Add(k);
                    }
                }
            }
            return list;
        }

        public override void EarlyKeyUpdate(GameTime time) {
            current = Keyboard.GetState();
        }

        public override void LateUpdate(GameTime time) {
            prev = current;
        }

        public override void Initialize(bool isErrorDisp) {
        }

        public override Exception GetError() {
            return null;
        }

        public override void ResetError() {
        }

        public override DateTime? GetErrorTime() {
            return null;
        }
    }
}
