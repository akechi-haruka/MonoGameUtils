using Haruka.MonoGameUtils.UI.Graphics.Animators;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Haruka.MonoGameUtils.UI.Elements;

public class ElementIconButton : ElementBorderedRectangle, ITouchable {

    private const int DEFAULT_BTN_SIZE = 25;

    public event Action<ElementIconButton> Click;

    private readonly GradientAnimator anim;
    private readonly Texture2D texture;
    public Color TextureTint { get; set; } = Color.White;

    public ElementIconButton(Texture2D texture, int x, int y, Action<ElementIconButton> onClick = null, int ? width = null, int? height = null) : base(x, y, width.GetValueOrDefault(texture?.Width ?? DEFAULT_BTN_SIZE), height.GetValueOrDefault(texture?.Height ?? DEFAULT_BTN_SIZE), Color.Black, Color.White) {
        anim = new GradientAnimator(this, Color.Gray, Color.Black, 200, false) {
            Paused = true
        };
        this.texture = texture;
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
        if (texture != null) {
            spriteBatch.Draw(texture, Rectangle, TextureTint);
        }
    }

    public bool ShouldDoOriginCheck() {
        return true;
    }
}