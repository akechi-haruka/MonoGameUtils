using Haruka.MonoGameUtils.UI.Elements;
using Haruka.MonoGameUtils.UI.Screens;
using Microsoft.Xna.Framework;

namespace Haruka.MonoGameUtils.UI.Graphics.Animators;

public class RepeatMoveAnimator : IAnimator {

    public bool Paused { get; set; }

    private readonly int origX;
    private readonly int origY;
    private readonly UIElement obj;
    private readonly int offsetX;
    private readonly int offsetY;
    private readonly double speed;

    private double progress;

    public RepeatMoveAnimator(UIElement obj, int offsetX, int offsetY, int speed = 1000) {
        this.obj = obj;
        origX = obj.X;
        origY = obj.Y;
        this.offsetX = offsetX;
        this.offsetY = offsetY;
        this.speed = speed;
    }

    public void Update(Screen screen, GameTime t) {
        if (!Paused) {
            progress += t.ElapsedGameTime.TotalMilliseconds;
            if (progress > speed) {
                progress = 0;
            }
            Vector2 pos = obj.Position;
            pos.X = (float)(origX + (progress / speed) * offsetX);
            pos.Y = (float)(origY + (progress / speed) * offsetY);
            obj.Position = pos;
        }
    }

    public void Restart() {
        Paused = false;
        progress = 0;
    }
}