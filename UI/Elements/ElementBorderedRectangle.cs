using Haruka.MonoGameUtils.Input;
using Haruka.MonoGameUtils.UI.Graphics;
using Haruka.MonoGameUtils.UI.Graphics.Animators;
using Haruka.MonoGameUtils.UI.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Haruka.MonoGameUtils.UI.Elements;

public class ElementBorderedRectangle : UIElement, IColorable, IAlphaable {
    public Rectangle Rectangle { get; private set; }

    public int Width { get; private set; }
    public int Height { get; private set; }
    public Color Fill { get; set; }
    public Color Border { get; set; }
    public float Alpha {
        get {
            return Fill.A / 255F;
        }
        set {
            Fill = new Color(Fill, (int)(value * 255));
        }
    }
    public Color Color {
        get {
            return Fill;
        }
        set {
            Fill = value;
        }
    }
    public int BorderSize { get; set; } = 1;

    public ElementBorderedRectangle(int x, int y, int width, int height, Color inner, Color border, int borderSize = 1, params IAnimator[] animators) : base(x, y) {
        Width = width;
        Height = height;
        Fill = inner;
        Border = border;
        BorderSize = borderSize;
        GenRect();
        foreach (IAnimator animator in animators) {
            AddAnimator(animator);
        }
    }

    private void GenRect() {
        Rectangle = new Rectangle((int)X, (int)Y, Width, Height);
    }

    protected override void DrawElement(GameTime gameTime, SpriteBatch spriteBatch) {
        spriteBatch.FillRectangle(Rectangle, Fill);
        spriteBatch.DrawRectangle(Rectangle, Border, BorderSize);
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

    public override void SetPosition(Vector2 position) {
        base.SetPosition(position);
        GenRect();
    }

    protected override void UpdateElement(ExtendedGame game, InputManager inputManager, Screen screen, GameTime gameTime) {
    }
}