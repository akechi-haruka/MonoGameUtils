using Haruka.MonoGameUtils.Input;
using Haruka.MonoGameUtils.UI.Graphics;
using Haruka.MonoGameUtils.UI.Graphics.Animators;
using Haruka.MonoGameUtils.UI.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Haruka.MonoGameUtils.UI.Elements;

public class ElementRectangle : UIElement, IColorable, IAlphaable {
    public bool Fill { get; }
    public Color Color { get; set; }
    public float Alpha {
        get {
            return Color.A / 255F;
        }
        set {
            Color = new Color(Color, (int)(value * 255));
        }
    }
    public int BorderSize { get; set; } = 1;

    public ElementRectangle(int x, int y, int width, int height, Color col, bool fill = false, params IAnimator[] animators) : base(x, y) {
        Width = width;
        Height = height;
        Color = col;
        Fill = fill;
        foreach (IAnimator animator in animators) {
            AddAnimator(animator);
        }
    }

    protected override void DrawElement(GameTime gameTime, SpriteBatch spriteBatch) {
        if (Fill) {
            spriteBatch.FillRectangle(Rectangle, Color);
        } else {
            spriteBatch.DrawRectangle(Rectangle, Color, BorderSize);
        }
    }

    protected override void UpdateElement(ExtendedGame game, InputManager inputManager, Screen screen, GameTime gameTime) {
    }
}