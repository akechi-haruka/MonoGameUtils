using Haruka.MonoGameUtils.Input;
using Haruka.MonoGameUtils.UI.Graphics;
using Haruka.MonoGameUtils.UI.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Haruka.MonoGameUtils.UI.Elements;

public class ElementLoadingBar : UIElement {

    private Rectangle rect;
    public bool IsCentered { get; }
    public Color Fill { get; set; }
    public Color Border { get; set; }
    public float Progress { get; set; }

    public ElementLoadingBar(int x, int y, int w, int h, bool center = false, Color? fill = null, Color? border = null) : this(new Point(x, y), w, h, center, fill, border) { }

    public ElementLoadingBar(Point pos, int w, int h, bool center = false, Color? fill = null, Color? border = null) : base(pos.X, pos.Y) {
        rect = new Rectangle(pos, new Point(w, h));
        IsCentered = center;
        Fill = fill.GetValueOrDefault(Color.Green);
        Border = border.GetValueOrDefault(Color.White);
    }

    protected override void DrawElement(GameTime gameTime, SpriteBatch spriteBatch) {
        if (IsCentered) {
            spriteBatch.DrawBarCentered(rect.X, rect.Y, rect.Width, rect.Height, Progress, Fill, Border);
        } else {
            spriteBatch.DrawBar(rect.X, rect.Y, rect.Width, rect.Height, Progress, Fill, Border);
        }
    }

    public override int GetHeight() {
        return rect.Height;
    }

    public override Rectangle GetRect() {
        return rect;
    }

    public override int GetWidth() {
        return rect.Width;
    }

    public override void SetHeight(int height) {
        rect.Height = height;
    }

    public override void SetWidth(int w) {
        rect.Width = w;
    }

    protected override void UpdateElement(ExtendedGame game, InputManager inputManager, Screen screen, GameTime gameTime) {
    }
}