using Haruka.MonoGameUtils.Input;
using Haruka.MonoGameUtils.UI.Graphics;
using Haruka.MonoGameUtils.UI.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Haruka.MonoGameUtils.UI.Elements;

public class Element2D : UIElement, IColorable {

    public static readonly float ROTATION_CONST = MathHelper.ToRadians(360);

    public Texture2D Texture { get; set; }
    public float Rotation { get; set; }
    public Color Tint { get; set; } = Color.White;
    public Color Color {
        get { return Tint; }
        set { Tint = value; }
    }

    public CenterFlags CenterFlags { get; set; }

    public Element2D(Texture2D tex, int x, int y, CenterFlags center = CenterFlags.NoCenter) : this(tex, x, y, tex.Width, tex.Height, center) {
    }

    public Element2D(Texture2D tex, Vector2 position, CenterFlags center = CenterFlags.NoCenter) : this(tex, (int)position.X, (int)position.Y, tex.Width, tex.Height, center) {
    }

    public Element2D(Texture2D tex, int x, int y, int width, int height, CenterFlags center = CenterFlags.NoCenter) : base(x, y) {
        Texture = tex;
        Width = width;
        Height = height;
        CenterFlags = center;
    }

    protected override void DrawElement(GameTime gameTime, SpriteBatch spriteBatch) {
        spriteBatch.Draw(Texture ?? Game.Skin.NoTexture, Rectangle, null, Tint, Rotation, new Vector2((CenterFlags & CenterFlags.CenterX) != 0 ? Width / 2F : 0, (CenterFlags & CenterFlags.CenterY) != 0 ? Height / 2F : 0), SpriteEffects.None, 0);
    }

    protected override void UpdateElement(ExtendedGame game, InputManager inputManager, Screen screen, GameTime gameTime) {
    }

    public void SetRotationDegrees(int deg) {
        Rotation = MathHelper.ToRadians(deg) % ROTATION_CONST;
    }

    public override string ToString() {
        return GetType() + "{" + Texture?.Name + "@" + X + "/" + Y + "}";
    }
}