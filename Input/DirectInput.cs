using Microsoft.Xna.Framework;
using OAS.Error;
using OAS.Util.CodeHelpers;
using OAS.Util.Logging;
using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAS.Input {

    public class JoystickInfo {
        public DeviceInstance joystickInstance;
        public Joystick joystickClassInstance;
        public bool errored;
        public Exception error;
        public bool active;
        public DateTime errorTime;

        public JoystickInfo(DeviceInstance deviceInstance) {
            joystickInstance = deviceInstance;
        }
    }

    public class DirectInput : IButtonInputAPI {
        public const string TAG = nameof(DirectInput);

        private SharpDX.DirectInput.DirectInput directInput;
        private List<JoystickInfo> joystickData;
        private Dictionary<Joystick, JoystickState> prevstates;
        private Dictionary<Joystick, JoystickState> states;

        public JoystickInfo GetByJoystick(Joystick j) {
            return joystickData.Where(e => e.joystickClassInstance == j).First();
        }

        public JoystickInfo GetByDeviceInstance(DeviceInstance di) {
            return joystickData.Where(e => e.joystickInstance == di).First();
        }

        internal string DeviceToString(JoystickInfo j) {
            return DeviceToString(j.joystickInstance);
        } 

        internal string DeviceToString(Joystick j) {
            return DeviceToString(GetByJoystick(j).joystickInstance);
        }

        internal string DeviceToString(DeviceInstance j) {
            return j.ProductName + " / " + j.ProductGuid;
        }

        private Dictionary<Key, List<Tuple<Joystick, Int32>>> mappings;

        public DirectInput() {

            joystickData = new List<JoystickInfo>();
            mappings = new Dictionary<Key, List<Tuple<Joystick, int>>>();
            states = new Dictionary<Joystick, JoystickState>();
            prevstates = new Dictionary<Joystick, JoystickState>();
            Reset();
        }

        public override void Initialize(bool isErrorDisp) {

            directInput = new SharpDX.DirectInput.DirectInput();

            foreach (var deviceInstance in directInput.GetDevices(DeviceType.Gamepad, DeviceEnumerationFlags.AllDevices)) {
                Log.Write("Detected DirectInput device: ", TAG);
                PrintAdd(deviceInstance);
                joystickData.Add(new JoystickInfo(deviceInstance));
            }
            foreach (var deviceInstance in directInput.GetDevices(DeviceType.Joystick, DeviceEnumerationFlags.AllDevices)) {
                Log.Write("Detected DirectInput device: ", TAG);
                PrintAdd(deviceInstance);
                joystickData.Add(new JoystickInfo(deviceInstance));
            }

            foreach (JoystickInfo j in joystickData) {

                Joystick controller = new Joystick(directInput, j.joystickInstance.InstanceGuid);
                controller.Properties.BufferSize = 128;
                controller.Acquire();

                j.joystickClassInstance = controller;
                prevstates[j.joystickClassInstance] = new JoystickState();
                states[j.joystickClassInstance] = new JoystickState();
            }
        }

        private void PrintAdd(DeviceInstance d) {
            Log.Write("  Instance ID: " + d.InstanceGuid, TAG);
            Log.Write("  Instance Name: " + d.InstanceName, TAG);
            Log.Write("  Product ID: " + d.ProductGuid, TAG);
            Log.Write("  Product Name: " + d.ProductName, TAG);
            Log.Write("  Type: " + d.Type, TAG);
            Log.Write("  Subtype: " + d.Subtype, TAG);
        }

        public override void Bind(Key key, string[] bindings) {

            List<Tuple<Joystick, int>> keys = new List<Tuple<Joystick, int>>();

            foreach (string binding in bindings) {
                JoystickInfo instance = null;
                int button = 0;
                if (binding.Contains('.')) {
                    String[] parts = binding.Split('.');
                    if (parts.Length != 2) {
                        Log.WriteWarning("Setting for " + key.Name + " is not in format DeviceIndex.ButtonIndex or DeviceName.ButtonIndex: " + binding, TAG);
                        continue;
                    }
                    instance = JoystickIndexByName(parts[0]);
                    if (instance == null) {
                        if (!Int32.TryParse(parts[0], out int index)) {
                            Log.WriteWarning("Unrecognized controller name or index for " + key.Name + ": " + parts[0], TAG);
                            continue;
                        }
                        instance = joystickData[index];
                    }
                    if (!Int32.TryParse(parts[1], out button)) {
                        Log.WriteWarning("Setting for " + key.Name + " is not in format DeviceIndex.ButtonIndex or DeviceName.ButtonIndex: " + binding, TAG);
                        continue;
                    }
                } else {
                    if (joystickData.Count == 0) {
                        Log.WriteWarning("No joysticks connected for: " + key.Name, TAG);
                        continue;
                    }
                    instance = joystickData[0];
                    if (!Int32.TryParse(binding, out button)) {
                        Log.WriteWarning("Setting for " + key.Name + " is not a button number: " + binding, TAG);
                        continue;
                    }
                }

                Joystick controller = instance.joystickClassInstance;

                if (button >= controller.Capabilities.ButtonCount) {
                    Log.WriteWarning("Joystick " + DeviceToString(controller) + " for setting " + key.Name + " has no button " + button, TAG);
                    continue;
                }

                Log.Write("Added binding for " + DeviceToString(controller) + ", button " + button, TAG);
                keys.Add(new Tuple<Joystick, int>(controller, button));

            }

            mappings.Add(key, keys);
        }

        private JoystickInfo JoystickIndexByName(string v) {
            foreach (JoystickInfo dev in joystickData) {
                if (dev.joystickInstance.InstanceName == v || dev.joystickInstance.ProductName == v || dev.joystickInstance.InstanceGuid.ToString() == v || dev.joystickInstance.ProductGuid.ToString() == v) {
                    return dev;
                }
            }
            return null;
        }

        public override void EarlyKeyUpdate(GameTime time) {
            foreach (JoystickInfo j in joystickData) {
                if (j.errored) {
                    try {
                        states[j.joystickClassInstance] = j.joystickClassInstance.GetCurrentState();
                    } catch (Exception ex) {
                        Log.WriteError("Failed to update joystick device!");
                        PrintAdd(j.joystickInstance);
                        states[j.joystickClassInstance] = new JoystickState();
                        j.errorTime = DateTime.Now;
                        j.errored = true;
                        j.error = new OASException(ErrorDictionary.Get(50001301), ex.Message, ex);
                        return;
                    }
                }
            }
        }

        public override bool IsJustPressed(Key key) {
            foreach (Tuple<Joystick, Int32> binding in mappings[key]) {
                bool b = states[binding.Item1].Buttons[binding.Item2];
                bool b2 = prevstates[binding.Item1].Buttons[binding.Item2];
                if (b && !b2) {
                    return true;
                }
            }
            return false;
        }

        public override bool IsPressed(Key key) {
            foreach (Tuple<Joystick, Int32> binding in mappings[key]) {
                if (states[binding.Item1].Buttons[binding.Item2]) {
                    return true;
                }
            }
            return false;
        }

        public override bool IsReleased(Key key) {
            return !IsPressed(key);
        }

        public override void LateUpdate(GameTime time) {
            foreach (JoystickInfo j in joystickData) {
                prevstates[j.joystickClassInstance] = states[j.joystickClassInstance];
            }
        }

        public JoystickInfo[] GetDevices() {
            return joystickData.ToArray();
        }

        public JoystickState GetState(Joystick index) {
            return states[index];
        }

        public override Exception GetError() {
            return joystickData.Where(j => j.errored).FirstOrDefault()?.error;
        }

        public void Reset() {
            Log.Write("Resetting all devices", TAG);
            foreach (JoystickInfo j in joystickData) {
                if (j.joystickClassInstance != null) {
                    try {
                        j.joystickClassInstance.Unacquire();
                        j.joystickClassInstance.Dispose();
                    } catch { }
                }
            }
            joystickData = new List<JoystickInfo>();
        }

        public override void ResetError() {
            Reset();
            Initialize(false);
        }

        public override DateTime? GetErrorTime() {
            return joystickData.Where(j => j.errored).FirstOrDefault()?.errorTime;
        }
    }
}
