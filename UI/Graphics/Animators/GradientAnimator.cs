using Microsoft.Xna.Framework;
using OAS.UI.Screens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAS.UI.Graphics.Animators {

    public class GradientAnimator : IAnimator {

        public bool Paused { get; set; }

        private IColorable obj;
        private Color from;
        private Color to;
        private double speed;
        private bool repeat;

        private bool forward = true;
        private double progress;

        public GradientAnimator(IColorable obj, Color from, Color to, double speed = 1000, bool repeat = true) {
            this.obj = obj;
            this.from = from;
            this.to = to;
            this.speed = speed;
            this.repeat = repeat;
        }

        public void Update(Screen screen, GameTime t) {
            if (!Paused) {
                if (forward) {
                    progress += t.ElapsedGameTime.TotalMilliseconds;
                    if (progress >= speed) {
                        progress = speed;
                        if (!repeat) {
                            Paused = true;
                        } else {
                            forward = false;
                        }
                    }
                } else {
                    progress -= t.ElapsedGameTime.TotalMilliseconds;
                    if (progress < 0) {
                        progress = 0;
                        if (!repeat) {
                            Paused = true;
                        } else {
                            forward = true;
                        }
                    }
                }
                obj.Color = InterpolateRGB(from, to, progress / speed);
            }
        }

        public void Restart() {
            Paused = false;
            progress = 0;
        }

        // https://stackoverflow.com/a/36469730
        public static Color InterpolateRGB(Color colorA, Color colorB, double bAmount) {
            double aAmount = 1.0f - bAmount;
            int red = (int)(colorA.R * aAmount + colorB.R * bAmount);
            int green = (int)(colorA.G * aAmount + colorB.G * bAmount);
            int blue = (int)(colorA.B * aAmount + colorB.B * bAmount);
            return new Color(red, green, blue);
        }
    }
}
