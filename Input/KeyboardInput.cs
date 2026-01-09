using Haruka.Common;
using Haruka.Common.Collections;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Haruka.MonoGameUtils.Input;

public class KeyboardInput : ButtonInputAPI {

    public bool CapsLockEnabled {
        get { return current.CapsLock; }
    }

    public bool NumLockEnabled {
        get { return current.NumLock; }
    }

    private KeyboardState prev = default;
    private KeyboardState current;
    private readonly BijectiveDictionary<Key, Keys> keys = new BijectiveDictionary<Key, Keys>();

    public override void Bind(Key key, string[] bindings) {
        foreach (string binding in bindings) {
            if (!Enum.TryParse(binding, out Keys k)) {
                InputManager.inputLog.LogWarning("Failed to parse key: " + binding);
            }
            keys.Add(key, k);
        }
    }

    public override bool IsJustPressed(Key key) {
        ISet<Keys> list = keys.GetForward(key);
        foreach (Keys k in list) {
            if (current.IsKeyDown(k) && !prev.IsKeyDown(k)) {
                return true;
            }
        }
        return false;
    }

    public override bool IsPressed(Key key) {
        ISet<Keys> list = keys.GetForward(key);
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
        ISet<Keys> list = keys.GetForward(key);
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
            ISet<Key> list2 = keys.GetReverse(kbkey);
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

    public override void Initialize() {
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