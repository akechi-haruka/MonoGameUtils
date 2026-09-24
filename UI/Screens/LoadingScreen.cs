using Haruka.MonoGameUtils.UI.Elements;
using Haruka.MonoGameUtils.UI.Graphics.Animators;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Haruka.MonoGameUtils.UI.Screens;

public class LoadingScreen<T> : Screen {
    private readonly string text;
    private readonly Func<T> loadFunc;
    private readonly Action<T> doneFunc;
    private bool done;
    private bool doneInvoked;
    private T result;

    public LoadingScreen(Func<T> loadFunc, Action<T> doneFunc, string text = "NOW LOADING") {
        this.text = text;
        this.loadFunc = loadFunc;
        this.doneFunc = doneFunc;
        IgnoreOnScreenStack = true;
    }

    protected override void OnScreenOpened() {
        new Thread(RunDelegate) {
            Name = "Loading Delegate"
        }.Start();
    }

    public override void OnCreateScreenElements() {
        ElementText el = new ElementText(text, Game.Width / 2, Game.Height / 2, CenterFlags.CenterX | CenterFlags.CenterY);
        el.AddAnimator(new FlashAnimator(el, 1000));
        AddElement(el);
    }

    private void RunDelegate() {
        result = loadFunc.Invoke();
        done = true;
    }

    protected override void DrawScreen(GameTime gameTime, SpriteBatch spriteBatch) {
    }

    protected override void UpdateScreen(GameTime gameTime) {
        if (done && !doneInvoked) {
            doneFunc?.Invoke(result);
            doneInvoked = true;
        }
    }
}

public class LoadingScreen : Screen {
    private readonly string text;
    private readonly Action loadFunc;
    private readonly Action doneFunc;
    private bool done;
    private bool doneInvoked;

    public LoadingScreen(Action loadFunc, Action doneFunc, string text = "NOW LOADING") {
        this.text = text;
        this.loadFunc = loadFunc;
        this.doneFunc = doneFunc;
        IgnoreOnScreenStack = true;
    }

    protected override void OnScreenOpened() {
        new Thread(RunDelegate) {
            Name = "Loading Delegate"
        }.Start();
    }

    public override void OnCreateScreenElements() {
        ElementText el = new ElementText(text, Game.Width / 2, Game.Height / 2, CenterFlags.CenterX | CenterFlags.CenterY);
        el.AddAnimator(new FlashAnimator(el, 1000));
        AddElement(el);
    }

    private void RunDelegate() {
        loadFunc.Invoke();
        done = true;
    }

    protected override void DrawScreen(GameTime gameTime, SpriteBatch spriteBatch) {
    }

    protected override void UpdateScreen(GameTime gameTime) {
        if (done && !doneInvoked) {
            doneFunc?.Invoke();
            doneInvoked = true;
        }
    }
}