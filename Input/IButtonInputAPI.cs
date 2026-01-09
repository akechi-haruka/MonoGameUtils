using Microsoft.Xna.Framework;
using OAS.Configuration;
using OAS.Error;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAS.Input {
    
    public abstract class IButtonInputAPI : IInputAPI {

        public static int KEY_HOLD_TIME_GLOBAL = 500;
        public static int KEY_REPEAT_START_TIME = 300;

        public IButtonInputAPI() {
            var ini = ConfigurationManager.ConfigurationProvider();
            if (ini != null) {
                KEY_HOLD_TIME_GLOBAL = ini.ReadInt("Input", "ButtonRepeatDelay", 500);
                KEY_REPEAT_START_TIME = KEY_HOLD_TIME_GLOBAL - ini.ReadInt("Input", "ButtonRepeatSpeed", 300);
            }
        }

        private Dictionary<Key, Int32> monitors = new Dictionary<Key, Int32>();

        public abstract void Bind(Key key, string[] bindings);

        public abstract bool IsPressed(Key key);

        public abstract bool IsJustPressed(Key key);

        public abstract bool IsReleased(Key key);

        public void EarlyUpdate(GameTime time) {

            foreach (Key key in monitors.Keys) {
                if (IsPressed(key)) {
                    monitors[key] += (int)time.ElapsedGameTime.TotalMilliseconds;
                } else {
                    monitors[key] = 0;
                }
            }


            EarlyKeyUpdate(time);
        }

        public abstract void EarlyKeyUpdate(GameTime time);

        public abstract void LateUpdate(GameTime time);

        public void AddMonitor(Key key) {
            monitors[key] = 0;
        }

        public void RemoveMonitor(Key key) {
            monitors.Remove(key);
        }

        public int GetKeyHoldTime(Key key) {
            return monitors.GetValueOrDefault(key, 0);
        }

        public void ResetKeyHoldTime(Key key, int value = 0) {
            monitors[key] = value;
        }

        public abstract void Initialize(bool isErrorDisp);
        public abstract Exception GetError();
        public abstract void ResetError();
        public abstract DateTime? GetErrorTime();
    }
}
