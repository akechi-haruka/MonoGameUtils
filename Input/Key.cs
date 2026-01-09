namespace Haruka.MonoGameUtils.Input;

public class Key {

    public string Name { get; private set; }
    public string DefaultKeyboardKey { get;private set; }

    public Key(string name, string defaultkb) {
        this.Name = name;
        this.DefaultKeyboardKey = defaultkb;
    }

    public override string ToString() {
        return Name;
    }
}