using Haruka.MonoGameUtils.UI.Elements;
using Haruka.MonoGameUtils.UI.Screens;
using Microsoft.Xna.Framework;

namespace Haruka.MonoGameUtils.UI.Graphics.Animators;

public class FadeAnimator : IAnimator {

    private readonly UIElement element;
    private readonly IAlphaable alpha;
    private readonly int delay;
    private readonly int length;

    private double time;

    public FadeAnimator(UIElement element, IAlphaable alpha, int delay, int length) {
        this.element = element;
        this.alpha = alpha;
        this.delay = delay;
        this.length = length;
    }

    public bool Paused { get; set; }

    public void Update(Screen screen, GameTime gameTime) {
        if (!Paused) {
            time += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (time > delay) {
                float alpha = 1F - (float)((time - delay) / length);
                if (alpha > 0) {
                    this.alpha.Alpha = alpha;
                } else {
                    element.Visible = false;
                    Paused = true;
                }
            }
        }
    }

}