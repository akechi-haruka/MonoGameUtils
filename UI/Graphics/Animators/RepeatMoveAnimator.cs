using Haruka.MonoGameUtils.UI.Elements;
using Haruka.MonoGameUtils.UI.Screens;
using Microsoft.Xna.Framework;

namespace Haruka.MonoGameUtils.UI.Graphics.Animators;

public class RepeatMoveAnimator : IAnimator {

    public bool Paused { get; set; }

    private readonly int orig_x;
    private readonly int orig_y;
    private readonly UIElement obj;
    private readonly int offset_x;
    private readonly int offset_y;
    private readonly double speed;

    private double progress;

    public RepeatMoveAnimator(UIElement obj, int offset_x, int offset_y, int speed = 1000) {
        this.obj = obj;
        orig_x = obj.GetX();
        orig_y = obj.GetY();
        this.offset_x = offset_x;
        this.offset_y = offset_y;
        this.speed = speed;
    }

    public void Update(Screen screen, GameTime t) {
        if (!Paused) {
            progress += t.ElapsedGameTime.TotalMilliseconds;
            if (progress > speed) {
                progress = 0;
            }
            Vector2 pos = obj.Position;
            pos.X = (float)(orig_x + (progress / speed) * offset_x);
            pos.Y = (float)(orig_y + (progress / speed) * offset_y);
            obj.SetPosition(pos);
        }
    }

    public void Restart() {
        Paused = false;
        progress = 0;
    }
}