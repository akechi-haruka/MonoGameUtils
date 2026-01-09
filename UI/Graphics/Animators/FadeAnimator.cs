using Microsoft.Xna.Framework;
using OAS.UI.Elements;
using OAS.UI.Screens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAS.UI.Graphics.Animators {
    public class FadeAnimator : IAnimator {

        private readonly UIElement element;
        private readonly IAlphaable alpha;
        private int delay;
        private int length;

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
}
