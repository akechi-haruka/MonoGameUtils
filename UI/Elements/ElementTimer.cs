using Haruka.MonoGameUtils.Input;
using Haruka.MonoGameUtils.UI.Graphics;
using Haruka.MonoGameUtils.UI.Graphics.Animators;
using Haruka.MonoGameUtils.UI.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Haruka.MonoGameUtils.UI.Elements;

public class ElementTimer : ElementBorderedRectangle {

    private const int WIDTH = 320;

    private TimeSpan time;
    private readonly TimeSpan lowTime = TimeSpan.FromSeconds(10);
    private readonly TimeSpan soundTime = TimeSpan.FromSeconds(5);
    private readonly Action onExpire;

    public bool Paused { get; set; }

    public ElementTimer(int time, Action onExpire) : base(Screen.TopRight.X - WIDTH, Screen.TopRight.Y + 60, WIDTH, ExtendedGame.Instance.Skin.DefaultFontHeight, Color.Black, Color.White) {
        this.time = TimeSpan.FromSeconds(time);
        this.onExpire = onExpire;
    }

    protected override void DrawElement(GameTime gameTime, SpriteBatch spriteBatch) {
        base.DrawElement(gameTime, spriteBatch);
        spriteBatch.DrawString("TIME REMAIN: " + ((int)time.TotalSeconds).ToString("D2"), X+10, Y);
    }

    protected override void UpdateElement(ExtendedGame game, InputManager inputManager, Screen screen, GameTime gameTime) {
        base.UpdateElement(game, inputManager, screen, gameTime);
        if (!Paused) {
            int ps = time.Seconds;
            time -= gameTime.ElapsedGameTime;
            if (time.Seconds != ps) {
                if (time <= lowTime && Animators.Count == 0) {
                    AddAnimator(new GradientAnimator(this, Color.Black, Color.Red, 500));
                }
                if (time <= soundTime) {
                    game.PlaySound("Sound/TimerLowTick");
                }
                if (time <= TimeSpan.Zero) {
                    Paused = true;
                    game.QueueOnLogicThread(onExpire);
                }
            }
        }
    }

    internal void SetPaused(bool v) {
        Paused = v;
    }
}