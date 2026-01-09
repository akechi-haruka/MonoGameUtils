using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OAS.Input;
using OAS.UI.Resources;
using OAS.UI.Screens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAS.UI.Elements {
    public class FPSViewerElement : UIElement {

        public const int W = 120;
        public const int H = 80;

        private ElementText text;

        public FPSViewerElement() : base(0, 0) {
            Children.Add(new ElementRectangle(GetX(), GetY(), W, H, Color.Gray, true));
            Children.Add(new ElementRectangle(GetX(), GetY(), W, H, Color.White) {
                BorderSize = 5
            });
            text = new ElementText("FPS: ---\nFT: ---", 10, 10) {
                Font = game.Skin.CJKFontSmall,
                Color = Color.White
            };
            Children.Add(text);
        }

        public override int GetHeight() {
            return H;
        }

        public override Rectangle GetRect() {
            throw new NotImplementedException();
        }

        public override int GetWidth() {
            return W;
        }

        public override void SetHeight(int height) {
            throw new NotImplementedException();
        }

        public override void SetWidth(int width) {
            throw new NotImplementedException();
        }

        protected override void DrawElement(GameTime gameTime, SpriteBatch spriteBatch) {
        }

        protected override void UpdateElement(Program game, InputManager inputManager, Screen screen, GameTime gameTime) {
            double ft = 1000D / game.Framerate;

            text.UpdateTextDirect("FPS: " + game.Framerate.ToString("N1") + "\nT: " + (ft < 1000 ? ft.ToString("N1") : "---")+"ms\nLS: " + game.lastSecond.Second);
        }
    }
}
