using Haruka.MonoGameUtils.UI.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Haruka.MonoGameUtils.UI.Screens;

public class LoadingScreen : Screen {

    private readonly string text;
    private readonly Action loadFunc;
    private readonly Action doneFunc;
    private bool done;
        
    public LoadingScreen(Action loadFunc, Action doneFunc, string text = "NOW LOADING") {
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
        AddElement(new ElementText(text, Game.Width / 2, Game.Height / 2, true, true));
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