using Haruka.MonoGameUtils.Input;
using Haruka.MonoGameUtils.UI.Graphics;
using Haruka.MonoGameUtils.UI.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Haruka.MonoGameUtils.UI.Elements;

public class ElementPopup : UIElement, IAlphaable {

    private float alpha;
    private readonly int borderSize = 5;

    public ElementPopup(string header, string value) : base(Screen.TopCenterSafeFrame.X - 350 / 2, Screen.TopCenterSafeFrame.Y) {
        Height = (int)(Game.Skin.DefaultFont.MeasureString(header).Y + Game.Skin.DefaultFont.MeasureString(value).Y + borderSize * 2);
        Width = Math.Max(350, (int)(Game.Skin.DefaultFont.MeasureString(value).Y + Game.Skin.DefaultFont.MeasureString(value).X + borderSize * 2));
        Position = new Vector2(Screen.TopCenterSafeFrame.X - Width / 2F, Screen.TopCenterSafeFrame.Y);  
        Children.Add(new ElementRectangle(X, Y, Width, Height, Color.White) {
            BorderSize = borderSize
        });
        Children.Add(new ElementRectangle(X, Y, Width, Height, Color.Gray, true));
        Children.Add(new ElementText(header, X + Width / 2, Y + borderSize, CenterFlags.CenterX));
        Children.Add(new ElementText(value, X + Width / 2, Y + borderSize + Game.Skin.DefaultFontHeight, CenterFlags.CenterX));

        alpha = 1.0F;
        DestroyWhenInvisible = true;
    }

    public float Alpha {
        get { return alpha; }
        set { PropagateAlpha(value); }
    }

    private void PropagateAlpha(float a) {
        alpha = a;
        foreach (UIElement child in Children) {
            if (child is IAlphaable alphaable) {
                alphaable.Alpha = a;
            }
        }
    }

    protected override void DrawElement(GameTime gameTime, SpriteBatch spriteBatch) {
    }

    protected override void UpdateElement(ExtendedGame game, InputManager inputManager, Screen screen, GameTime gameTime) {
    }
}