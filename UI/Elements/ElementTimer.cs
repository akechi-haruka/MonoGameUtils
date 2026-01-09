using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OAS.Input;
using OAS.UI.Graphics;
using OAS.UI.Graphics.Animators;
using OAS.UI.Resources;
using OAS.UI.Screens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAS.UI.Elements {
    public class ElementTimer : ElementBorderedRectangle {

        private const int WIDTH = 320;

        private TimeSpan time;
        private TimeSpan lowTime = TimeSpan.FromSeconds(10);
        private TimeSpan soundTime = TimeSpan.FromSeconds(5);
        private Action onExpire;

        public bool Paused { get; set; }

        public ElementTimer(int time, Action onExpire) : base(Screen.TOP_RIGHT.X - WIDTH, Screen.TOP_RIGHT.Y + 60, WIDTH, Program.Main.Skin.DefaultFontHeight, Color.Black, Color.White) {
            this.time = TimeSpan.FromSeconds(time);
            this.onExpire = onExpire;
        }

        protected override void DrawElement(GameTime gameTime, SpriteBatch spriteBatch) {
            base.DrawElement(gameTime, spriteBatch);
            spriteBatch.DrawString("TIME REMAIN: " + ((int)time.TotalSeconds).ToString("D2"), X+10, Y);
        }

        protected override void UpdateElement(Program game, InputManager inputManager, Screen screen, GameTime gameTime) {
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
}
