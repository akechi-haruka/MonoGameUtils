using System.Diagnostics;
using Haruka.Common;
using Haruka.MonoGameUtils.Input;
using Haruka.MonoGameUtils.UI.Elements;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Haruka.MonoGameUtils.UI.Screens;

public abstract class Screen {
    public const int SAFE_FRAME_SIZE = 20;
    public static Point TopLeft;
    public static Point TopCenter;
    public static Point TopRight;
    public static Point MiddleLeft;
    public static Point MiddleCenter;
    public static Point MiddleRight;
    public static Point BottomLeft;
    public static Point BottomMiddle;
    public static Point BottomRight;
    public static Point TopSafeFrame;
    public static Point TopCenterSafeFrame;
    public static Point BottomCenterSafeFrame;
    public static Point BottomCenterSafeFrameT;
    public static Point BottomLeftSafeFrameT;
    public static Point BottomRightSafeFrameT;
    public static Point BottomSafeFrame;
    public static Rectangle ScreenRect;

    protected readonly ExtendedGame Game;
    public Screen PreviousScreen { get; protected set; }
    protected InputManager InputManager { get; private set; }
    public bool IgnoreOnScreenStack { get; protected set; }

    private readonly List<UIElement> elements;

    public Keys LastKeyboardKey = Keys.None;

    protected Screen() {
        Game = ExtendedGame.Instance;
        InputManager = Game.InputManager;
        elements = new List<UIElement>();
    }

    internal void OpenScreen(Screen prev) {
        PreviousScreen = prev;
        ResetScreenElements();
        OnScreenOpened();
    }

    internal void CloseScreen() {
        OnScreenClosed();
    }

    protected virtual void OnScreenOpened() { }

    protected virtual void OnScreenClosed() { }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch) {
        foreach (UIElement e in elements.Where(e => e.Visible)) {
            e.Draw(gameTime, spriteBatch);
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
                    Log.Main.LogError(e + " has changed visible elements on the screen!!");
                }
                if (e.DestroyWhenInvisible && !e.Visible) {
                    elements.RemoveAt(i);
                }
                if (e is ITouchable it) {
                    if (Game.InputManager.IsJustClickReleased(e, it.ShouldDoOriginCheck())){
                        it.OnTouch(Game.InputManager.GetTouchX(), Game.InputManager.GetTouchY());
                    }
                }

                if (e.BlockUpdatePropagation) {
                    return;
                }
            }
        }

        UpdateScreen(gameTime);
    }

    protected abstract void UpdateScreen(GameTime gameTime);

    public UIElement AddElement(UIElement e) {
        ArgumentNullException.ThrowIfNull(e);
        CheckUiThread();
        lock (elements) {
            if (elements.Contains(e)) {
                Log.Main.LogWarning("Attempted to add UIElement " + e + " twice!");
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
        ResetScreenElements();
    }

    public virtual void OnCreateScreenElements() {

    }

    public virtual void OnKeyboardTypeEvent(TextInputEventArgs e) {
        LastKeyboardKey = e.Key;
    }

    public void RemoveAllElements() {
        lock (elements) {
            elements.Clear();
        }
    }

    public UIElement GetElementAt(Point p, UiElementSearchOrder order = UiElementSearchOrder.Topmost) {
        UIElement el = null;
        lock (elements) {
            foreach (UIElement e in elements.Where(e => e.Rectangle.Contains(p))) {
                if (order == UiElementSearchOrder.Topmost) {
                    el = e;
                } else if (order == UiElementSearchOrder.First) {
                    return e;
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
        return default;
    }

    private void CheckUiThread() {

        if (!(Game.DrawScreen == null || Game.CurrentScreen.GetType() == typeof(LoadingScreen)) && Game.LogicThread != Thread.CurrentThread) {
            Log.Main.LogError("!!! ATTEMPTING TO ADD ELEMENT FROM NON-LOGIC THREAD OUTSIDE OF A LOADING SCREEN! THIS IS A HUGE CRASH RISK !!!");
            Log.Main.LogError("Current screen is: " + Game.CurrentScreen?.GetType());
            Log.Main.LogError("Current thread is: " + Thread.CurrentThread.Name + "\n" + new StackTrace());
        }

    }

    internal void ResetScreenElements() {
        lock (elements) {
            RemoveAllElements();
            OnCreateScreenElements();
        }
    }
}

public enum UiElementSearchOrder {
    First, Topmost
}