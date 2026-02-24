using Haruka.MonoGameUtils.Input;
using Haruka.MonoGameUtils.UI.Graphics;
using Haruka.MonoGameUtils.UI.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Haruka.MonoGameUtils.UI.Elements;

public class ElementPopup : UIElement, IAlphaable {

    private float alpha;
    private readonly int height;
    private readonly int width;
    private readonly int borderSize = 5;

    public ElementPopup(string header, string value) : base(Screen.TopCenterSafeFrame.X - 350 / 2, Screen.TopCenterSafeFrame.Y) {
        height = (int)(Game.Skin.DefaultFont.MeasureString(header).Y + Game.Skin.DefaultFont.MeasureString(value).Y + borderSize * 2);
        width = Math.Max(350, (int)(Game.Skin.DefaultFont.MeasureString(value).Y + Game.Skin.DefaultFont.MeasureString(value).X + borderSize * 2));
        SetPosition(new Point(Screen.TopCenterSafeFrame.X - width / 2, Screen.TopCenterSafeFrame.Y));

        Children.Add(new ElementRectangle(GetX(), GetY(), width, height, Color.White) {
            BorderSize = borderSize
        });
        Children.Add(new ElementRectangle(GetX(), GetY(), width, height, Color.Gray, true));
        Children.Add(new ElementText(header, GetX() + width / 2, GetY() + borderSize, true));
        Children.Add(new ElementText(value, GetX() + width / 2, GetY() + borderSize + Game.Skin.DefaultFontHeight, true));

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

    public override int GetHeight() {
        return height;
    }

    public override Rectangle GetRect() {
        return new Rectangle(GetX(), GetY(), width, height);
    }

    public override int GetWidth() {
        return width;
    }

    public override void SetHeight(int h) {
        throw new NotImplementedException();
    }

    public override void SetWidth(int w) {
        throw new NotImplementedException();
    }

    protected override void DrawElement(GameTime gameTime, SpriteBatch spriteBatch) {
    }

    protected override void UpdateElement(ExtendedGame game, InputManager inputManager, Screen screen, GameTime gameTime) {
    }
}