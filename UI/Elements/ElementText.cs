using Haruka.MonoGameUtils.Input;
using Haruka.MonoGameUtils.UI.Graphics;
using Haruka.MonoGameUtils.UI.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Haruka.MonoGameUtils.UI.Elements;

public class ElementText : UIElement, IColorable, IAlphaable {

    public static Color DefaultTextColor = Color.White;

    public SpriteFont Font {
        get;
        set;
    }

    public Color Color { get; set; } = ExtendedGame.Instance.Skin.UnselectedItemColor;
    public float Alpha {
        get {
            return Color.A / 255F;
        }
        set {
            Color = new Color(Color, (int)(value * 255));
        }
    }
    public string Text { get; private set; }
    private int width;
    private int height;
    private readonly bool centerX;
    private readonly bool centerY;
    private Vector2 origin;
    private Rectangle rect;

    public ElementText(string text, Point point, bool centerX = false, bool centerY = false) : this(text, point.X, point.Y, centerX, centerY) {
    }

    public ElementText(string text, Point point, int xoff = 0, int yoff = 0, bool centerX = false, bool centerY = false) : this(text, point.X + xoff, point.Y + yoff, centerX, centerY) {
    }

    public ElementText(string text, int x, int y, bool centerX = false, bool centerY = false, SpriteFont font = null) : base(x, y) {
        this.centerX = centerX;
        this.centerY = centerY;
        Font = font;
        if (Font == null) {
            Font = Game.Skin.DefaultFont;
        }

        if (Font == null) {
            Font = Game.Skin.FallbackFont;
        }
        UpdateText(text);
    }

    public override int GetWidth() {
        return width;
    }

    public override int GetHeight() {
        return height;
    }

    protected override void DrawElement(GameTime gameTime, SpriteBatch spriteBatch) {
        if (Text != null) {
            spriteBatch.DrawString(Font, Text, Position, Color, 0, origin, 1, SpriteEffects.None, 0);
        }
    }

    protected override void UpdateElement(ExtendedGame game, InputManager inputManager, Screen screen, GameTime gameTime) {
    }

    public override void SetWidth(int w) {
        throw new NotImplementedException("Can't resize a text object");
    }

    public override void SetHeight(int height) {
        throw new NotImplementedException("Can't resize a text object");
    }


    public void UpdateText(string text) {
        if (text == Text) {
            return;
        }
        Text = text;
        Vector2 vec = text != null ? Font.MeasureString(text) : Vector2.Zero;
        width = (int)vec.X;
        height = (int)vec.Y;
        if (centerX || centerY) {
            origin = new Vector2(centerX ? width / 2F : 0, centerY ? height / 2F : 0);
        } else {
            origin = Vector2.Zero;
        }
        GenRect();
    }

    public void UpdateTextDirect(string text) {
        Text = text;
    }

    private void GenRect() {
        rect = new Rectangle(GetX(), GetY(), width, height);
    }

    public override Rectangle GetRect() {
        return rect;
    }

    public override string ToString() {
        return "ElementText{" + Text + "@" + X + "/" + Y + "}";
    }

}