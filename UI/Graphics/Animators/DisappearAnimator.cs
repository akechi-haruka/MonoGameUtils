using Haruka.MonoGameUtils.UI.Elements;
using Haruka.MonoGameUtils.UI.Screens;
using Microsoft.Xna.Framework;

namespace Haruka.MonoGameUtils.UI.Graphics.Animators;

public class DisappearAnimator : IAnimator {

    private readonly UIElement element;
    private readonly int delay;

    private double time;

    public DisappearAnimator(UIElement element, int delay) {
        this.element = element;
        this.delay = delay;
    }

    public bool Paused { get; set; }

    public void Update(Screen screen, GameTime gameTime) {
        if (!Paused) {
            time += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (time > delay) {
                element.Visible = false;
                Paused = true;
            }
        }
    }

}