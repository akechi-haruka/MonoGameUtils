using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OAS.Input;
using OAS.UI.Graphics;
using OAS.UI.Screens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAS.UI.Elements {
    public class ElementLoadingBar : UIElement {

        private Rectangle rect;
        public bool IsCentered { get; private set; }
        public Color Fill { get; set; }
        public Color Border { get; set; }
        public float Progress { get; set; }

        public ElementLoadingBar(int x, int y, int w, int h, bool center = false, Color? fill = null, Color? border = null) : this(new Point(x, y), w, h, center, fill, border) { }

        public ElementLoadingBar(Point pos, int w, int h, bool center = false, Color? fill = null, Color? border = null) : base(pos.X, pos.Y) {
            this.rect = new Rectangle(pos, new Point(w, h));
            IsCentered = center;
            Fill = fill.GetValueOrDefault(Color.Green);
            Border = border.GetValueOrDefault(Color.White);
        }

        protected override void DrawElement(GameTime gameTime, SpriteBatch spriteBatch) {
            if (IsCentered) {
                spriteBatch.DrawBarCentered(rect.X, rect.Y, rect.Width, rect.Height, Progress, Fill, Border);
            } else {
                spriteBatch.DrawBar(rect.X, rect.Y, rect.Width, rect.Height, Progress, Fill, Border);
            }
        }

        public override int GetHeight() {
            return rect.Height;
        }

        public override Rectangle GetRect() {
            return rect;
        }

        public override int GetWidth() {
            return rect.Width;
        }

        public override void SetHeight(int height) {
            rect.Height = height;
        }

        public override void SetWidth(int width) {
            rect.Width = width;
        }

        protected override void UpdateElement(Program game, InputManager inputManager, Screen screen, GameTime gameTime) {
        }
    }
}
