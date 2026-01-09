using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OAS.UI.Graphics;
using OAS.UI.Graphics.Animators;
using OAS.UI.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAS.UI.Elements {
    public class ElementButton : ElementBorderedRectangle, ITouchable {

        public event Action<ElementButton> Click;

        private GradientAnimator anim;
        private String text;
        private Vector2 textSize;
        public bool OriginCheck { get; set; } = true;

        public ElementButton(int x, int y, int width, int height, String text = null, Action<ElementButton> onClick = null) : base(x, y, width, height, Program.Main.Skin.SystemBackgroundColor, Program.Main.Skin.SystemBorderColor) {
            anim = new GradientAnimator(this, Color.Gray, Program.Main.Skin.SystemBackgroundColor, 200, false) {
                Paused = true
            };
            this.text = text;
            if (text != null) {
                this.textSize = game.Skin.DefaultFont.MeasureString(text);
            }
            if (onClick != null) {
                Click += onClick;
            }
            AddAnimator(anim);
        }

        public void OnTouch(int x, int y) {
            anim.Restart();
            Click?.Invoke(this);
        }

        protected override void DrawElement(GameTime gameTime, SpriteBatch spriteBatch) {
            base.DrawElement(gameTime, spriteBatch);
            if (text != null) {
                spriteBatch.DrawString(text, GetX() + Width / 2 - textSize.X / 2, GetY() + Height / 2 - textSize.Y / 2, game.Skin.UnselectedItemColor);
            }
        }

        public bool ShouldDoOriginCheck() {
            return OriginCheck;
        }
    }
}
