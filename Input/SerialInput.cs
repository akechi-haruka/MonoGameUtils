using Haruka.Common.Collections;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;

namespace Haruka.MonoGameUtils.Input;

public class SerialInput : ButtonInputAPI {

    public const string TAG = nameof(SerialInput);

    public enum Button {
        SerialService,
        SerialTest
    }

    private readonly List<Button> buttons;
    private readonly BijectiveDictionary<Key, Button> keys;
    private Dictionary<Button, bool> statePrev;
    private Dictionary<Button, bool> stateNow;

    public SerialInput() {
        keys = new BijectiveDictionary<Key, Button>();
        buttons = Enum.GetValues<Button>().ToList();
        statePrev = new Dictionary<Button, bool>();
        foreach (Button kbkey in buttons) {
            statePrev[kbkey] = false;
        }
        stateNow = statePrev;
    }

    public override void Bind(Key key, string[] bindings) {
        foreach (string binding in bindings) {
            if (!Enum.TryParse(binding, out Button k)) {
                InputManager.inputLog.LogWarning("Failed to parse key: " + binding);
            } else {
                keys.Add(key, k);
            }
        }
    }

    public override bool IsJustPressed(Key key) {
        ISet<Button> list = keys.GetForward(key);
        if (list == null) { return false; }
        foreach (Button k in list) {
            if (stateNow[k] && !statePrev[k]) {
                return true;
            }
        }
        return false;
    }

    public override bool IsPressed(Key key) {
        ISet<Button> list = keys.GetForward(key);
        if (list == null) { return false; }
        foreach (Button k in list) {
            if (stateNow[k]) {
                return true;
            }
        }
        return false;
    }

    public override bool IsReleased(Key key) {
        ISet<Button> list = keys.GetForward(key);
        if (list == null) { return false; }
        foreach (Button k in list) {
            if (!stateNow[k]) {
                return true;
            }
        }
        return false;
    }

    public List<Button> GetAllPressedKeys() {
        List<Button> list = new List<Button>();
        foreach (Button kbkey in stateNow.Keys) {
            if (stateNow[kbkey]) {
                list.Add(kbkey);
            }
        }
        return list;
    }

    public override void EarlyKeyUpdate(GameTime time) {
        stateNow = new Dictionary<Button, bool>();
        foreach (Button kbkey in buttons) {
            stateNow[kbkey] = false;
        }
    }

    public override void LateUpdate(GameTime time) {
        statePrev = stateNow;
    }

    public override void Initialize() {
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