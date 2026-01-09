using Haruka.MonoGameUtils.Input;
using Haruka.MonoGameUtils.UI.Graphics;
using Haruka.MonoGameUtils.UI.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Haruka.MonoGameUtils.UI.Elements;

public class Element2D : UIElement, IColorable {

    public static readonly float ROTATION_CONST = MathHelper.ToRadians(360);

    protected int Width { get; private set; }
    protected int Height { get; private set; }
    public Texture2D Texture { get; set; }
    public float Rotation { get; set; }
    public Vector2 Origin { get; protected set; }
    public Color Tint { get; set; } = Color.White;
    public Color Color {
        get { return Tint; }
        set { Tint = value; }
    }

    private Rectangle rect;
    private bool centerX;
    private bool centerY;

    public Element2D(Texture2D tex, int x, int y) : this(tex, x, y, tex.Width, tex.Height) {
    }

    public Element2D(Texture2D tex, Vector2 position) : this(tex, (int)position.X, (int)position.Y, tex.Width, tex.Height) {
    }

    public Element2D(Texture2D tex, int x, int y, int width, int height) : base(x, y) {
        Texture = tex;
        Width = width;
        Height = height;
        UpdateRect();
    }

    public override int GetWidth() {
        return Width;
    }

    public override int GetHeight() {
        return Height;
    }

    protected override void DrawElement(GameTime gameTime, SpriteBatch spriteBatch) {
        spriteBatch.Draw(Texture ?? Game.Skin.NoTexture, rect, null, Tint, Rotation, Origin, SpriteEffects.None, 0);
    }

    protected override void UpdateElement(ExtendedGame game, InputManager inputManager, Screen screen, GameTime gameTime) {
    }

    public override void SetWidth(int w) {
        Width = w;
        UpdateRect();
    }

    public override void SetHeight(int height) {
        Height = height;
        UpdateRect();
    }

    private void UpdateRect() {
        rect = new Rectangle((int)X, (int)Y, Width, Height);
        Origin = new Vector2(centerX ? Width / 2F : 0, centerY ? Height / 2F : 0);
    }

    public void SetDrawCentered(bool x, bool y) {
        centerX = x;
        centerY = y;
        UpdateRect();
    }

    public void SetRotationDegrees(int deg) {
        Rotation = MathHelper.ToRadians(deg) % ROTATION_CONST;
    }

    public override Rectangle GetRect() {
        return rect;
    }

    public override string ToString() {
        return GetType() + "{" + Texture?.Name + "@" + X + "/" + Y + "}";
    }

    public override void SetPosition(Vector2 position) {
        base.SetPosition(position);
        UpdateRect();
    }

    public void Stretch(Vector2 vector2) {
        SetWidth((int)vector2.X);
        SetHeight((int)vector2.Y);
    }
}