using Haruka.MonoGameUtils.Input;
using Haruka.MonoGameUtils.UI.Graphics;
using Haruka.MonoGameUtils.UI.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Haruka.MonoGameUtils.UI.Elements;

public class ElementText : UIElement, IColorable, IAlphaable {

    public SpriteFont Font {
        get;
        set;
    }

    public Color Color {
        get; 
        set;
    } = ExtendedGame.Instance.Skin.UnselectedItemColor;
    public float Alpha {
        get {
            return Color.A / 255F;
        }
        set {
            Color = new Color(Color, (int)(value * 255));
        }
    }
    public string Text { get; private set; }
    public CenterFlags CenterFlags { get; set; }
    
    private Vector2 origin;

    public ElementText(string text, Point point, CenterFlags center = CenterFlags.NoCenter) : this(text, point.X, point.Y, center) {
    }

    public ElementText(string text, int x, int y, CenterFlags center = CenterFlags.NoCenter, SpriteFont font = null) : base(x, y) {
        CenterFlags = center;
        Font = (font ?? Game.Skin.DefaultFont) ?? Game.Skin.FallbackFont;
        UpdateText(text);
    }

    protected override void DrawElement(GameTime gameTime, SpriteBatch spriteBatch) {
        if (Text != null) {
            spriteBatch.DrawString(Font, Text, Position, Color, 0, origin, 1, SpriteEffects.None, 0);
        }
    }

    protected override void UpdateElement(ExtendedGame game, InputManager inputManager, Screen screen, GameTime gameTime) {
    }

    public void UpdateText(string text) {
        if (text == Text) {
            return;
        }
        Text = text;
        Vector2 vec = text != null ? Font.MeasureString(text) : Vector2.Zero;
        Width = (int)vec.X;
        Height = (int)vec.Y;
        origin = new Vector2((CenterFlags & CenterFlags.CenterX) != 0 ? Width / 2F : 0, (CenterFlags & CenterFlags.CenterY) != 0 ? Height / 2F : 0);
    }

    public void UpdateTextDirect(string text) {
        Text = text;
    }

    public override string ToString() {
        return "ElementText{" + Text + "@" + X + "/" + Y + "}";
    }

}