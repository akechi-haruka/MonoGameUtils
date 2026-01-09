using Haruka.MonoGameUtils.UI.Screens;
using Microsoft.Xna.Framework;

namespace Haruka.MonoGameUtils.UI.Graphics.Animators;

public interface IAnimator {

    public bool Paused { get; set; }

    public void Update(Screen screen, GameTime gameTime);

}