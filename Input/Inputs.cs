namespace Haruka.MonoGameUtils.Input;

public static class Inputs {

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
    public static readonly Key UP = new Key("Up", "Left,Up");
    public static readonly Key DOWN = new Key("Down", "Right,Down");
    public static readonly Key ENTER = new Key("Enter", "Enter");

    public static readonly Key[] ALL_KEYS = new[] { SERVICE, TEST, ENTER, UP, DOWN, INT_EXCLUSIVE_FULLSCREEN, INT_FPS_CAP, INT_FPS_VIEWER, INT_RESOLUTION, INT_KBD_CTRL, INT_KBD_X, INT_KBD_C, INT_KBD_V, INT_KBD_ENTER, INT_KBD_ESC, INT_CLEAR_CACHE };

}