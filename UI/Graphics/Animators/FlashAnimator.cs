using Microsoft.Xna.Framework;
using OAS.UI.Elements;
using OAS.UI.Screens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAS.UI.Graphics.Animators {
    public class FlashAnimator : IAnimator {

        private readonly UIElement element;
        private int delay;

        private double time;

        public FlashAnimator(UIElement element, int delay) {
            this.element = element;
            this.delay = delay;
        }

        public bool Paused { get; set; }

        public void Restart() {
            if (!Paused) {
                element.Visible = true;
                time = 0;
            }
        }

        public void Update(Screen screen, GameTime gameTime) {
            if (!Paused) {
                time += gameTime.ElapsedGameTime.TotalMilliseconds;
                if (time > delay) {
                    element.Visible = !element.Visible;
                    time = 0;
                }
            }
        }

    }
}
