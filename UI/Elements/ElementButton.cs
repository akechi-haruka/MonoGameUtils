using Haruka.MonoGameUtils.UI.Graphics;
using Haruka.MonoGameUtils.UI.Graphics.Animators;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Haruka.MonoGameUtils.UI.Elements;

public class ElementButton : ElementBorderedRectangle, ITouchable {

    public event Action<ElementButton> Click;

    private readonly GradientAnimator anim;
    private readonly string text;
    private readonly Vector2 textSize;
    public bool OriginCheck { get; set; } = true;

    public ElementButton(int x, int y, int width, int height, string text = null, Action<ElementButton> onClick = null) : base(x, y, width, height, ExtendedGame.Instance.Skin.SystemBackgroundColor, ExtendedGame.Instance.Skin.SystemBorderColor) {
        anim = new GradientAnimator(this, Color.Gray, ExtendedGame.Instance.Skin.SystemBackgroundColor, 200, false) {
            Paused = true
        };
        this.text = text;
        if (text != null) {
            textSize = Game.Skin.DefaultFont.MeasureString(text);
        }
        if (onClick != null) {
            Click += onClick;
        }
        AddAnimator(anim);
    }

    public void OnTouch(int x, int y) {
        anim.Restart();
        Click?.Invoke(this);
    }

    protected override void DrawElement(GameTime gameTime, SpriteBatch spriteBatch) {
        base.DrawElement(gameTime, spriteBatch);
        if (text != null) {
            spriteBatch.DrawString(text, GetX() + Width / 2 - textSize.X / 2, GetY() + Height / 2 - textSize.Y / 2, Game.Skin.UnselectedItemColor);
        }
    }

    public bool ShouldDoOriginCheck() {
        return OriginCheck;
    }
}