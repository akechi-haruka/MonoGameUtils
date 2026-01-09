using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Haruka.MonoGameUtils.UI.Screens;

public interface IRenderable<in T> {

    public void Draw(ExtendedGame game, SpriteBatch s, GameTime t, float xOffset, float yOffset, T owner, bool selected, int maxOptionNameWidth);

}