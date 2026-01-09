using Haruka.MonoGameUtils.UI.Elements;
using Haruka.MonoGameUtils.UI.Screens;
using Microsoft.Xna.Framework;

namespace Haruka.MonoGameUtils.UI.Graphics.Animators;

public class RotateAnimator : IAnimator {

    private readonly Element2D element;

    private readonly int speed;

    public RotateAnimator(Element2D element, int speed) {
        this.element = element;
        this.speed = speed;
    }

    public bool Paused { get; set; }

    public void Update(Screen screen, GameTime gameTime) {
        if (!Paused) {
            element.Rotation += (float)(gameTime.ElapsedGameTime.TotalMilliseconds * speed / 360F);
        }
    }

}