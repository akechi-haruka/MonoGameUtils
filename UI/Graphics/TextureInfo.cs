using Microsoft.Xna.Framework.Graphics;

namespace Haruka.MonoGameUtils.UI.Graphics;

public class TextureInfo {

    public Texture2D Texture { get; private set; }
    public int Width { get; private set; }
    public int Height { get; private set; }

    public TextureInfo(Texture2D tex) {
        Texture = tex;
        Width = tex.Width;
        Height = tex.Height;
    }

}