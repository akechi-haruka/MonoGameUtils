namespace Haruka.MonoGameUtils.Input;

public class Key {
    public string Name { get; }
    public string DefaultKeyboardKey { get; private set; }

    public Key(string name, string defaultKey) {
        Name = name;
        DefaultKeyboardKey = defaultKey;
    }

    public override string ToString() {
        return Name;
    }
}