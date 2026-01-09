using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using OAS.Input;

using OAS.Screens;
using OAS.UI.Elements;
using OAS.Util;
using OAS.Util.Logging;

namespace OAS.UI.Screens {
    public abstract class Screen {

        public static int SAFE_FRAME_SIZE = 20;
        public static Point TOP_LEFT;
        public static Point TOP_CENTER;
        public static Point TOP_RIGHT;
        public static Point MIDDLE_LEFT;
        public static Point MIDDLE_CENTER;
        public static Point MIDDLE_RIGHT;
        public static Point BOTTOM_LEFT;
        public static Point BOTTOM_MIDDLE;
        public static Point BOTTOM_RIGHT;
        public static Point TOP_SAFE_FRAME;
        public static Point TOP_CENTER_SAFE_FRAME;
        public static Point BOTTOM_CENTER_SAFE_FRAME;
        public static Point BOTTOM_CENTER_SAFE_FRAME_T;
        public static Point BOTTOM_LEFT_SAFE_FRAME_T;
        public static Point BOTTOM_RIGHT_SAFE_FRAME_T;
        public static Point BOTTOM_SAFE_FRAME;
        public static Rectangle SCREEN;

        protected Program game;
        public Screen PreviousScreen { get; protected set; }
        protected InputManager InputManager { get; private set; }
        public bool IgnoreOnScreenStack { get; protected set; }

        private List<UIElement> elements;

        public Keys lastKeyboardKey = Keys.None;

        protected Screen() {
            game = Program.Main;
            InputManager = game.InputManager;
            elements = new List<UIElement>();
        }

        internal void OpenScreen(Screen prev) {
            PreviousScreen = prev;
            OnScreenOpened();
        }

        internal void CloseScreen() {
            OnScreenClosed();
        }

        protected virtual void OnScreenOpened() { }

        protected virtual void OnScreenClosed() { }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch) {
            foreach (UIElement e in elements) {
                if (e.Visible) {
                    e.Draw(gameTime, spriteBatch);
                }
            }
            DrawScreen(gameTime, spriteBatch);
        }

        protected abstract void DrawScreen(GameTime gameTime, SpriteBatch spriteBatch);

        public void Update(GameTime gameTime) {

            lock (elements) {
                for (int i = elements.Count - 1; i >= 0; i--) {
                    UIElement e = elements[i];
                    int presize = elements.Count;
                    e.Update(this, gameTime);
                    if (elements.Count != presize) {
                        Log.WriteError(e + " has changed visible elements on the screen!!", "Stability");
                        //break;
                    }
                    if (e.DestroyWhenInvisible && !e.Visible) {
                        elements.RemoveAt(i);
                    }
                    if (e is ITouchable it) {
                        if (game.InputManager.IsJustClickReleased(e, it.ShouldDoOriginCheck())){
                            it.OnTouch(game.InputManager.GetTouchX(), game.InputManager.GetTouchY());
                        }
                    }
                }
            }

            UpdateScreen(gameTime);
        }

        protected abstract void UpdateScreen(GameTime gameTime);

        public UIElement AddElement(UIElement e) {
            ArgumentNullException.ThrowIfNull(e);
            CheckUIThread();
            lock (elements) {
                if (elements.Contains(e)) {
                    Log.WriteError("Attempted to add UIElement " + e + " twice!", "Performance");
                } else {
                    elements.Add(e);
                }
            }
            return e;
        }

        public UIElement RemoveElement(UIElement e) {
            lock (elements) {
                elements.Remove(e);
            }
            return e;
        }

        public void RemoveElementsByType(Type e) {
            lock (elements) {
                elements.RemoveAll((el) => el.GetType() == e);
            }
        }

        public virtual void OnGameResized() {
        }

        public virtual void OnCreateScreenElements() {

        }

        public virtual void OnKeyboardTypeEvent(TextInputEventArgs e) {
            lastKeyboardKey = e.Key;
        }

        public void RemoveAllElements() {
            lock (elements) {
                elements.Clear();
            }
        }

        public UIElement GetElementAt(Point p, UIElementSearchOrder order = UIElementSearchOrder.TOPMOST) {
            UIElement el = null;
            lock (elements) {
                foreach (UIElement e in elements) {
                    if (e.GetRect().Contains(p)) {
                        if (order == UIElementSearchOrder.TOPMOST) {
                            el = e;
                        } else if (order == UIElementSearchOrder.FIRST) {
                            return e;
                        }
                    }
                }
            }
            return el;
        }

        public UIElement[] GetElements() {
            lock (elements) {
                return elements.ToArray();
            }
        }

        public T GetElement<T>() {
            lock (elements) {
                foreach (UIElement e in elements) {
                    if (e is T) {
                        return (T)(object)e;
                    }
                }
            }
            return default(T);
        }

        private void CheckUIThread() {

            if (!(game.DrawScreen == null || game.CurrentScreen.GetType() == typeof(LoadingScreen)) && game.LogicThread != Thread.CurrentThread) {
                Log.WriteError("!!! ATTEMPTING TO ADD ELEMENT FROM NON-LOGIC THREAD OUTSIDE OF A LOADING SCREEN! THIS IS A HUGE CRASH RISK !!!", "Stability");
                Log.WriteError("Current screen is: " + game.CurrentScreen?.GetType(), "Stability");
                Log.WriteError("Current thread is: " + Thread.CurrentThread.Name + "\n" + new StackTrace(), "Stability");
#if DEBUG
#pragma warning disable
                // Do not remove this, this is a permanent breakpoint for debug mode because this really should never happen.
                int breakpointdummy = 0;
#pragma warning restore
#endif
            }

        }

        internal void ResetScreenElements() {
            lock (elements) {
                RemoveAllElements();
                OnCreateScreenElements();
            }
        }
    }

    public enum UIElementSearchOrder {
        FIRST, TOPMOST
    }
}
