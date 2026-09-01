using Haruka.MonoGameUtils.Input;
using Haruka.MonoGameUtils.UI.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Haruka.MonoGameUtils.UI.Elements;

public class FPSViewerElement : UIElement {

    public const int W = 120;
    public const int H = 80;

    private readonly ElementText text;

    public FPSViewerElement() : base(0, 0) {
        Children.Add(new ElementRectangle(X, Y, W, H, Color.Gray, true));
        Children.Add(new ElementRectangle(X, Y, W, H, Color.White) {
            BorderSize = 5
        });
        text = new ElementText("FPS: ---\nFT: ---", 10, 10) {
            Font = Game.Skin.DefaultFontSmall,
            Color = Color.White
        };
        Children.Add(text);
    }

    protected override void DrawElement(GameTime gameTime, SpriteBatch spriteBatch) {
    }

    protected override void UpdateElement(ExtendedGame game, InputManager inputManager, Screen screen, GameTime gameTime) {
        double ft = 1000D / game.Framerate;

        text.UpdateTextDirect("FPS: " + game.Framerate.ToString("N1") + "\nT: " + (ft < 1000 ? ft.ToString("N1") : "---")+"ms\nLS: " + game.LastSecond.Second);
    }
}