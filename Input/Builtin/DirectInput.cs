using Haruka.MonoGameUtils.Input.Api;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using SharpDX.DirectInput;

namespace Haruka.MonoGameUtils.Input.Builtin;

public class JoystickInfo {
    public readonly DeviceInstance JoystickInstance;
    public Joystick JoystickClassInstance;
    public bool Active;

    public JoystickInfo(DeviceInstance deviceInstance) {
        JoystickInstance = deviceInstance;
    }
}

public class DirectInput : ButtonInputAPI {

    private SharpDX.DirectInput.DirectInput directInput;
    private List<JoystickInfo> joystickData;
    private readonly Dictionary<Joystick, JoystickState> prevstates;
    private readonly Dictionary<Joystick, JoystickState> states;

    public JoystickInfo GetByJoystick(Joystick j) {
        return joystickData.First(e => e.JoystickClassInstance == j);
    }

    public JoystickInfo GetByDeviceInstance(DeviceInstance di) {
        return joystickData.First(e => e.JoystickInstance == di);
    }

    internal string DeviceToString(JoystickInfo j) {
        return DeviceToString(j.JoystickInstance);
    } 

    internal string DeviceToString(Joystick j) {
        return DeviceToString(GetByJoystick(j).JoystickInstance);
    }

    internal static string DeviceToString(DeviceInstance j) {
        return j.ProductName + " / " + j.ProductGuid;
    }

    private readonly Dictionary<Key, List<Tuple<Joystick, int>>> mappings;

    public DirectInput() {

        joystickData = new List<JoystickInfo>();
        mappings = new Dictionary<Key, List<Tuple<Joystick, int>>>();
        states = new Dictionary<Joystick, JoystickState>();
        prevstates = new Dictionary<Joystick, JoystickState>();
        Reset();
    }

    private static void PrintAdd(DeviceInstance d) {
        InputManager.InputLog.LogInformation("  Instance ID: " + d.InstanceGuid);
        InputManager.InputLog.LogInformation("  Instance Name: " + d.InstanceName);
        InputManager.InputLog.LogInformation("  Product ID: " + d.ProductGuid);
        InputManager.InputLog.LogInformation("  Product Name: " + d.ProductName);
        InputManager.InputLog.LogInformation("  Type: " + d.Type);
        InputManager.InputLog.LogInformation("  Subtype: " + d.Subtype);
    }

    public override void Bind(Key key, string[] bindings) {

        List<Tuple<Joystick, int>> keys = new List<Tuple<Joystick, int>>();

        foreach (string binding in bindings) {
            if (!binding.Contains('.')) {
                continue;
            }
            
            string[] parts = binding.Split('.');
            if (parts.Length != 2) {
                InputManager.InputLog.LogWarning("Setting for " + key.Name + " is not in format DeviceIndex.ButtonIndex or DeviceName.ButtonIndex: " + binding);
                continue;
            }
            JoystickInfo instance = JoystickIndexByName(parts[0]);
            if (instance == null) {
                if (!Int32.TryParse(parts[0], out int index)) {
                    InputManager.InputLog.LogWarning("Unrecognized controller name or index for " + key.Name + ": " + parts[0]);
                    continue;
                }
                instance = joystickData[index];
            }
            if (!Int32.TryParse(parts[1], out int button)) {
                InputManager.InputLog.LogWarning("Setting for " + key.Name + " is not in format DeviceIndex.ButtonIndex or DeviceName.ButtonIndex: " + binding);
                continue;
            }

            Joystick controller = instance.JoystickClassInstance;

            if (button >= controller.Capabilities.ButtonCount) {
                InputManager.InputLog.LogWarning("Joystick " + DeviceToString(controller) + " for setting " + key.Name + " has no button " + button);
                continue;
            }

            InputManager.InputLog.LogInformation("Added binding for " + DeviceToString(controller) + ", button " + button);
            keys.Add(new Tuple<Joystick, int>(controller, button));

        }

        mappings.Add(key, keys);
    }

    private JoystickInfo JoystickIndexByName(string v) {
        foreach (JoystickInfo dev in joystickData) {
            if (dev.JoystickInstance.InstanceName == v || dev.JoystickInstance.ProductName == v || dev.JoystickInstance.InstanceGuid.ToString() == v || dev.JoystickInstance.ProductGuid.ToString() == v) {
                return dev;
            }
        }
        return null;
    }

    public override bool IsJustPressed(Key key) {
        foreach (Tuple<Joystick, int> binding in mappings[key]) {
            bool b = states[binding.Item1].Buttons[binding.Item2];
            bool b2 = prevstates[binding.Item1].Buttons[binding.Item2];
            if (b && !b2) {
                return true;
            }
        }
        return false;
    }

    public override bool IsPressed(Key key) {
        foreach (Tuple<Joystick, int> binding in mappings[key]) {
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
            prevstates[j.JoystickClassInstance] = states[j.JoystickClassInstance];
        }
    }

    public JoystickInfo[] GetDevices() {
        return joystickData.ToArray();
    }

    public JoystickState GetState(Joystick index) {
        return states[index];
    }

    public void Reset() {
        InputManager.InputLog.LogInformation("Resetting all devices");
        foreach (JoystickInfo j in joystickData.Where(j => j.JoystickClassInstance != null)) {
            try {
                j.JoystickClassInstance.Unacquire();
                j.JoystickClassInstance.Dispose();
            } catch {
                // ignored
            }
        }
        joystickData = new List<JoystickInfo>();
    }

    public override void EarlyKeyUpdate(GameTime time) {
        foreach (JoystickInfo j in joystickData.Where(j => j.Active)) {
            states[j.JoystickClassInstance] = j.JoystickClassInstance.GetCurrentState();
        }
    }

    public override void Initialize() {
        directInput = new SharpDX.DirectInput.DirectInput();
        ScanControllers();
    }

    public override void Start() {
        ScanControllers();

        foreach (JoystickInfo j in joystickData) {
            Joystick controller = new Joystick(directInput, j.JoystickInstance.InstanceGuid);
            controller.Properties.BufferSize = 128;
            controller.Acquire();

            j.JoystickClassInstance = controller;
            prevstates[j.JoystickClassInstance] = new JoystickState();
            states[j.JoystickClassInstance] = new JoystickState();
        }
    }

    private void ScanControllers() {
        joystickData = new List<JoystickInfo>();
        foreach (DeviceInstance deviceInstance in directInput.GetDevices(DeviceType.Gamepad, DeviceEnumerationFlags.AllDevices)) {
            InputManager.InputLog.LogInformation("Detected DirectInput device: ");
            PrintAdd(deviceInstance);
            joystickData.Add(new JoystickInfo(deviceInstance));
        }
        foreach (DeviceInstance deviceInstance in directInput.GetDevices(DeviceType.Joystick, DeviceEnumerationFlags.AllDevices)) {
            InputManager.InputLog.LogInformation("Detected DirectInput device: ");
            PrintAdd(deviceInstance);
            joystickData.Add(new JoystickInfo(deviceInstance));
        }
    }

    public override void Stop() {
        Reset();
    }

}