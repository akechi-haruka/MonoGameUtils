namespace Haruka.MonoGameUtils.UI.Elements;

public class TextInputDialog : Dialog {

    public string Title { get; }
    public Action<string> Callback { get; }
    public bool AllowCancel { get; }

    private readonly ElementTextBox input;

    internal TextInputDialog(string title, string @default, Action<string> callback, bool allowCancel) : base(title) {
        Title = title;
        Callback = callback;
        AllowCancel = allowCancel;
        UserClosable = false;
        input = new ElementTextBox((int)X, (int)Y + Height / 2 - Game.Skin.DefaultFontHeight / 2, Width, @default) {
            AllowCancel = allowCancel
        };
        input.Activate();
        input.OnConfirm += Input_OnConfirm;
        AddChild(input);
    }

    private void Input_OnConfirm(string obj) {
        Callback?.Invoke(obj);
        Close(1);
    }

}