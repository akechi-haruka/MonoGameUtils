using Haruka.Common.Configuration;
using Microsoft.Xna.Framework;

namespace Haruka.MonoGameUtils.Input.Api;

public abstract class ButtonInputAPI : IInputAPI {

    public static int KeyHoldTimeGlobal = 500;
    public static int KeyRepeatStartTime = 300;

    private readonly Dictionary<Key, int> monitors = new Dictionary<Key, int>();

    public virtual void Initialize() {
        IniFile ini = ExtendedGame.Instance.Configuration;
        if (ini != null) {
            KeyHoldTimeGlobal = ini.ReadInt("ButtonRepeatDelay", InputManager.SECTION_INPUT, 500);
            KeyRepeatStartTime = KeyHoldTimeGlobal - ini.ReadInt("ButtonRepeatSpeed", InputManager.SECTION_INPUT, 300);
        }
    }

    public abstract void Start();

    public abstract void Stop();

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
    
}