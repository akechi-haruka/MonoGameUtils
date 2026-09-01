using Haruka.Common.Util;
using Haruka.MonoGameUtils.Input;
using Haruka.MonoGameUtils.UI.Graphics;
using Haruka.MonoGameUtils.UI.Graphics.Animators;
using Haruka.MonoGameUtils.UI.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Haruka.MonoGameUtils.UI.Elements;

public class ElementTextBox : UIElement {

    public bool AllowCancel { get; set; }

    private string value;
    private int strWidth;

    private readonly ElementText theLine;
    private readonly FlashAnimator fa;

    public event Action<string> OnConfirm;

    public ElementTextBox(int x, int y, int width, string initial = null) : base(x, y) {
        Width = width;
        Height = (int)(Game.Skin.DefaultFontHeight * 1.5F);
        value = initial ?? "";
        AddChild(new ElementRectangle(x, y, width, Height, Color.White));
        theLine = new ElementText("|", x, y);
        fa = new FlashAnimator(theLine, 500);
        theLine.AddAnimator(fa);
        AddChild(theLine);
        UpdateStringWidth();
    }

    public void SetWidth(int w) {
        Width = w;
        foreach (UIElement child in Children) {
            child.Width = w;
        }
    }

    private void UpdateText(string str) {
        value = str;
        UpdateStringWidth();
    }

    private void UpdateStringWidth() {
        strWidth = (int)Game.Skin.DefaultFont.MeasureString(value).X;
        theLine.Position = new Vector2(X + strWidth, theLine.Y);
    }

    public void Activate() {
        ExtendedGame.Instance.Window.TextInput += Window_TextInput;
        // soft keyboard subscribe
    }

    public void Deactivate() {
        ExtendedGame.Instance.Window.TextInput -= Window_TextInput;
        // soft keyboard subscribe
    }

    public void Submit(string result) {
        Deactivate();
        OnConfirm?.Invoke(result);
    }

    private void Window_TextInput(object sender, TextInputEventArgs e) {
        //Log.WriteTraced("Text: " + e.Character + "/" + e.Key);
        if (e.Key == Keys.Back) {
            if (value.Length > 0) {
                value = value.Substring(0, value.Length - 1);
                UpdateText(value);
            }
        } else if (e.Key == Keys.Enter) {
            Submit(value);
        } else if (e.Key == Keys.Escape && AllowCancel) {
            Submit(null);
        } else if (!Char.IsControl(e.Character)) {
            value += e.Character;
            UpdateText(value);
        }
        fa.Restart();
    }

    protected override void DrawElement(GameTime gameTime, SpriteBatch spriteBatch) {
        spriteBatch.DrawString(value, X, Y);
    }

    protected override void UpdateElement(ExtendedGame game, InputManager inputManager, Screen screen, GameTime gameTime) {
        if (inputManager.IsAnyJustPressed(Inputs.INT_KBD_ESC) && AllowCancel) {
            Submit(null);
        } else if (inputManager.IsAnyJustPressed(Inputs.INT_KBD_CTRL)){
            if (inputManager.IsAnyJustPressed(Inputs.INT_KBD_X)) {
                value = "";
                UpdateText(value);
            } else if (inputManager.IsAnyJustPressed(Inputs.INT_KBD_C)) {
                Clipboard.Write(value);
            } else if (inputManager.IsAnyJustPressed(Inputs.INT_KBD_V)) {
                value += Clipboard.Read();
                UpdateText(value);
            }
        }
    }
}