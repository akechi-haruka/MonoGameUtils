using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OAS.UI.Elements;
using OAS.UI.Resources;
using OAS.UI.Screens;

namespace OAS.Screens {
    public class LoadingScreen : Screen {

        private string text;
        private Action loadFunc;
        private Action doneFunc;
        private bool done;
        
        public LoadingScreen(Action loadFunc, Action doneFunc, String text = "NOW LOADING") {
            this.text = text;
            this.loadFunc = loadFunc;
            this.doneFunc = doneFunc;
            IgnoreOnScreenStack = true;
        }

        protected override void OnScreenOpened() {
            OnGameResized();
            new Thread(RunDelegate) {
                Name = "Loading Delegate"
            }.Start();
        }

        public override void OnGameResized() {
            base.OnGameResized();
            RemoveAllElements();
            AddElement(new ElementText(text, game.Width / 2, game.Height / 2, true, true));
        }

        private void RunDelegate() {
            loadFunc.Invoke();
            done = true;
        }

        protected override void DrawScreen(GameTime gameTime, SpriteBatch spriteBatch) {

        }

        protected override void UpdateScreen(GameTime gameTime) {
            if (done) {
                doneFunc?.Invoke();
            }
        }
    }
}
