namespace Haruka.MonoGameUtils.Input;

public class Inputs {

    public static readonly Key INT_FPS_VIEWER = new Key("InternalUseOnlyFPSViewer", "F12");
    public static readonly Key INT_EXCLUSIVE_FULLSCREEN = new Key("InternalUseOnlyExclusiveFullscreen", "F11");
    public static readonly Key INT_FPS_CAP = new Key("InternalUseOnlyFPSCapToggle", "F10");
    public static readonly Key INT_RESOLUTION = new Key("InternalUseOnlyResolution", "F9");
    public static readonly Key INT_CLEAR_CACHE = new Key("InternalUseOnlyClearCache", "F5");
    public static readonly Key INT_KBD_CTRL = new Key(null, "LeftControl");
    public static readonly Key INT_KBD_X = new Key(null, "X");
    public static readonly Key INT_KBD_C = new Key(null, "C");
    public static readonly Key INT_KBD_V = new Key(null, "V");
    public static readonly Key INT_KBD_ESC = new Key(null, "Escape");
    public static readonly Key INT_KBD_ENTER = new Key(null, "Enter");

    public static readonly Key SERVICE = new Key("Service", "F1");
    public static readonly Key TEST = new Key("Test", "F2");
    public static readonly Key P1Left = new Key("Player1Left", "Left,Up");
    public static readonly Key P1Right = new Key("Player1Right", "Right,Down");
    public static readonly Key P1Confirm = new Key("Player1Confirm", "Enter");
    public static readonly Key P1Cancel = new Key("Player1Cancel", "Escape");
    public static readonly Key P2Left = new Key("Player2Left", "NumPad4,NumPad8");
    public static readonly Key P2Right = new Key("Player2Right", "NumPad6,NumPad2");
    public static readonly Key P2Confirm = new Key("Player2Confirm", "NumPad0");
    public static readonly Key P2Cancel = new Key("Player2Cancel", "OemComma");
    public static readonly Key Operator = new Key("Operator", "NumPad9");

    public static readonly Key[] ALL_KEYS = new[] { SERVICE, TEST, P1Confirm, P1Cancel, P1Left, P1Right, P2Confirm, P2Cancel, P2Left, P2Right, Operator, INT_EXCLUSIVE_FULLSCREEN, INT_FPS_CAP, INT_FPS_VIEWER, INT_RESOLUTION, INT_KBD_CTRL, INT_KBD_X, INT_KBD_C, INT_KBD_V, INT_KBD_ENTER, INT_KBD_ESC, INT_CLEAR_CACHE };

}