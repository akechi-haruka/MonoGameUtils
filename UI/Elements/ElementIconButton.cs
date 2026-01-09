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
    public class ElementIconButton : ElementBorderedRectangle, ITouchable {

        private const int DEFAULT_BTN_SIZE = 25;

        public event Action<ElementIconButton> Click;

        private GradientAnimator anim;
        private Texture2D texture;
        public Color TextureTint { get; set; } = Color.White;

        public ElementIconButton(Texture2D texture, int x, int y, Action<ElementIconButton> onClick = null, int ? width = null, int? height = null) : base(x, y, width.GetValueOrDefault(texture?.Width ?? DEFAULT_BTN_SIZE), height.GetValueOrDefault(texture?.Height ?? DEFAULT_BTN_SIZE), Color.Black, Color.White) {
            anim = new GradientAnimator(this, Color.Gray, Color.Black, 200, false) {
                Paused = true
            };
            this.texture = texture;
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
            if (texture != null) {
                spriteBatch.Draw(texture, GetRect(), TextureTint);
            }
        }

        public bool ShouldDoOriginCheck() {
            return true;
        }
    }
}
