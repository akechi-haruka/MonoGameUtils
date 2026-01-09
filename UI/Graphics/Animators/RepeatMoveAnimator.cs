using Microsoft.Xna.Framework;
using OAS.UI.Elements;
using OAS.UI.Screens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAS.UI.Graphics.Animators {

    public class RepeatMoveAnimator : IAnimator {

        public bool Paused { get; set; }

        private int orig_x;
        private int orig_y;
        private UIElement obj;
        private int offset_x;
        private int offset_y;
        private double speed;

        private double progress;

        public RepeatMoveAnimator(UIElement obj, int offset_x, int offset_y, int speed = 1000) {
            this.obj = obj;
            this.orig_x = obj.GetX();
            this.orig_y = obj.GetY();
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
}
