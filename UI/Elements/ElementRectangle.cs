using Haruka.MonoGameUtils.Input;
using Haruka.MonoGameUtils.UI.Graphics;
using Haruka.MonoGameUtils.UI.Graphics.Animators;
using Haruka.MonoGameUtils.UI.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Haruka.MonoGameUtils.UI.Elements;

public class ElementRectangle : UIElement, IColorable, IAlphaable {
    public Rectangle Rectangle { get; private set; }

    public int Width { get; private set; }
    public int Height { get; private set; }
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
        GenRect();
        foreach (IAnimator animator in animators) {
            AddAnimator(animator);
        }
    }

    private void GenRect() {
        Rectangle = new Rectangle((int)X, (int)Y, Width, Height);
    }

    protected override void DrawElement(GameTime gameTime, SpriteBatch spriteBatch) {
        if (Fill) {
            spriteBatch.FillRectangle(Rectangle, Color);
        } else {
            spriteBatch.DrawRectangle(Rectangle, Color, BorderSize);
        }
    }

    public override int GetHeight() {
        return Height;
    }

    public override Rectangle GetRect() {
        return Rectangle;
    }

    public override int GetWidth() {
        return Width;
    }

    public override void SetHeight(int height) {
        Height = height;
        GenRect();
    }

    public override void SetWidth(int w) {
        Width = w;
        GenRect();
    }

    protected override void UpdateElement(ExtendedGame game, InputManager inputManager, Screen screen, GameTime gameTime) {
    }
}