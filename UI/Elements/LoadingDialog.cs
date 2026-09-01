using Haruka.MonoGameUtils.Input;
using Haruka.MonoGameUtils.UI.Graphics.Animators;
using Haruka.MonoGameUtils.UI.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Haruka.MonoGameUtils.UI.Elements;

public class LoadingDialog : Dialog {

    Element2D spinner;

    public LoadingDialog(string message, int width = 700, int height = 300) : base(message, null, width, height, true) {
    }

    protected override void UpdateElement(ExtendedGame game, InputManager inputManager, Screen screen, GameTime gameTime) {
        if (spinner == null) {
            Texture2D tex = game.LoadTexture("LoadingSpinner");
            spinner = new Element2D(tex, (int)(X + Width / 2F), (int)(Y + Height / 2F), CenterFlags.CenterX | CenterFlags.CenterY);
            spinner.AddAnimator(new RotateAnimator(spinner, 2));
            Children.Add(spinner);
        }
    }
}