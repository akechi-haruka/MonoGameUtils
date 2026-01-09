using Microsoft.Xna.Framework;
using OAS.UI.Elements;
using OAS.UI.Screens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAS.UI.Graphics.Animators {
    public class RotateAnimator : IAnimator {

        private readonly Element2D element;

        private int speed;

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
}
