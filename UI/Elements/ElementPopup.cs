using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OAS.Input;
using OAS.UI.Graphics;
using OAS.UI.Resources;
using OAS.UI.Screens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAS.UI.Elements {
    public class ElementPopup : UIElement, IAlphaable {

        private string header;
        private string value;
        private float alpha;
        private int height;
        private int width;
        private int borderSize = 5;

        public ElementPopup(string header, string value) : base(Screen.TOP_CENTER_SAFE_FRAME.X - 350 / 2, Screen.TOP_CENTER_SAFE_FRAME.Y) {
            this.header = header;
            this.value = value;
            height = (int)(game.Skin.DefaultFont.MeasureString(header).Y + game.Skin.DefaultFont.MeasureString(value).Y + borderSize * 2);
            width = Math.Max(350, (int)(game.Skin.DefaultFont.MeasureString(value).Y + game.Skin.DefaultFont.MeasureString(value).X + borderSize * 2));
            SetPosition(new Point(Screen.TOP_CENTER_SAFE_FRAME.X - width / 2, Screen.TOP_CENTER_SAFE_FRAME.Y));

            Children.Add(new ElementRectangle(GetX(), GetY(), width, height, Color.White) {
                BorderSize = borderSize
            });
            Children.Add(new ElementRectangle(GetX(), GetY(), width, height, Color.Gray, true));
            Children.Add(new ElementText(header, GetX() + width / 2, GetY() + borderSize, true, false));
            Children.Add(new ElementText(value, GetX() + width / 2, GetY() + borderSize + game.Skin.DefaultFontHeight, true, false));

            alpha = 1.0F;
            DestroyWhenInvisible = true;
        }

        public float Alpha { get => alpha; set => PropagateAlpha(value); }

        private void PropagateAlpha(float alpha) {
            this.alpha = alpha;
            foreach (UIElement child in Children) {
                if (child is IAlphaable alphaable) {
                    alphaable.Alpha = alpha;
                }
            }
        }

        public override int GetHeight() {
            return height;
        }

        public override Rectangle GetRect() {
            return new Rectangle(GetX(), GetY(), width, height);
        }

        public override int GetWidth() {
            return width;
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
        }
    }
}
