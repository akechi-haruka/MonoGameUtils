using Haruka.MonoGameUtils.Input;
using Haruka.MonoGameUtils.UI.Graphics;
using Haruka.MonoGameUtils.UI.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Haruka.MonoGameUtils.UI.Elements;

public class ElementLoadingBar : UIElement {

    public bool IsCentered { get; }
    public Color Fill { get; set; }
    public Color Border { get; set; }
    public float Progress { get; set; }

    public ElementLoadingBar(int x, int y, int w, int h, bool center = false, Color? fill = null, Color? border = null) : this(new Point(x, y), w, h, center, fill, border) { }

    public ElementLoadingBar(Point pos, int w, int h, bool center = false, Color? fill = null, Color? border = null) : base(pos.X, pos.Y) {
        Width = w;
        Height = h;
        IsCentered = center;
        Fill = fill.GetValueOrDefault(Color.Green);
        Border = border.GetValueOrDefault(Color.White);
    }

    protected override void DrawElement(GameTime gameTime, SpriteBatch spriteBatch) {
        Rectangle rect = Rectangle;
        if (IsCentered) {
            spriteBatch.DrawBarCentered(rect.X, rect.Y, rect.Width, rect.Height, Progress, Fill, Border);
        } else {
            spriteBatch.DrawBar(rect.X, rect.Y, rect.Width, rect.Height, Progress, Fill, Border);
        }
    }

    protected override void UpdateElement(ExtendedGame game, InputManager inputManager, Screen screen, GameTime gameTime) {
    }
}