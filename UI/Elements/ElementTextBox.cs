using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using OAS.Input;

using OAS.SystemData;
using OAS.UI.Graphics;
using OAS.UI.Graphics.Animators;
using OAS.UI.Resources;
using OAS.UI.Screens;
using OAS.Util.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAS.UI.Elements {
    
    public class ElementTextBox : UIElement {

        public enum InputStyle {
            FullText, ASCIIOnly, NumbersOnly
        }

        public bool AllowCancel { get; set; }

        private String value;
        private InputStyle style;
        private int width;
        private int strWidth;

        private ElementText theLine;
        private FlashAnimator fa;

        public event Action<String> OnConfirm;

        public ElementTextBox(int x, int y, int width, InputStyle style = InputStyle.FullText, String initial = null) : base(x, y) {
            this.width = width;
            this.style = style;
            value = initial ?? "";
            AddChild(new ElementRectangle(x, y, width, GetHeight(), Color.White));
            theLine = new ElementText("|", x, y);
            fa = new FlashAnimator(theLine, 500);
            theLine.AddAnimator(fa);
            AddChild(theLine);
            UpdateStringWidth();
            Log.WriteTraced("Created");
        }

        public override int GetHeight() {
            return (int)(game.Skin.DefaultFontHeight * 1.5F);
        }

        public override Rectangle GetRect() {
            return new Rectangle(GetX(), GetY(), width, GetHeight());
        }

        public override int GetWidth() {
            return width;
        }

        public override void SetHeight(int height) {
            throw new NotImplementedException();
        }

        public override void SetWidth(int width) {
            this.width = width;
            foreach (UIElement child in Children) {
                child.SetWidth(width);
            }
        }

        private void UpdateText(String str) {
            value = str;
            UpdateStringWidth();
        }

        private void UpdateStringWidth() {
            strWidth = (int)game.Skin.DefaultFont.MeasureString(value).X;
            theLine.SetPosition(new Vector2(X + strWidth, theLine.Y));
        }

        public void Activate() {
            Log.WriteTraced("Activated");
            Program.Main.Window.TextInput += Window_TextInput;
            // soft keyboard subscribe
        }

        public void Deactivate() {
            Log.WriteTraced("Deactivated");
            Program.Main.Window.TextInput -= Window_TextInput;
            // soft keyboard subscribe
        }

        public void Submit(String result) {
            Log.WriteTraced("TextBox result: " + result);
            Deactivate();
            OnConfirm?.Invoke(result);
        }

        private void Window_TextInput(object sender, TextInputEventArgs e) {
            //Log.WriteTraced("Text: " + e.Character + "/" + e.Key);
            if (e.Key == Keys.Back) {
                if (value.Length > 0) {
                    value = value.Substring(0, value.Length - 1);
                    UpdateText(value);
                }
            } else if (e.Key == Keys.Enter) {
                Submit(value);
            } else if (e.Key == Keys.Escape && AllowCancel) {
                Submit(null);
            } else if (!Char.IsControl(e.Character)) {
                value += e.Character;
                UpdateText(value);
            }
            fa.Restart();
        }

        protected override void DrawElement(GameTime gameTime, SpriteBatch spriteBatch) {
            spriteBatch.DrawString(value, X, Y);
        }

        protected override void UpdateElement(Program game, InputManager inputManager, Screen screen, GameTime gameTime) {
            if (inputManager.IsAnyJustPressed(Inputs.P1Cancel) && AllowCancel) {
                Submit(null);
            } else if (inputManager.IsAnyJustPressed(Inputs.INT_KBD_CTRL)){
                if (inputManager.IsAnyJustPressed(Inputs.INT_KBD_X)) {
                    value = "";
                    UpdateText(value);
                } else if (inputManager.IsAnyJustPressed(Inputs.INT_KBD_C)) {
                    Clipboard.Write(value);
                } else if (inputManager.IsAnyJustPressed(Inputs.INT_KBD_V)) {
                    value += Clipboard.Read();
                    UpdateText(value);
                }
            }
        }
    }
}
