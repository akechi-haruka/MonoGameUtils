using Haruka.MonoGameUtils.Input;
using Haruka.MonoGameUtils.UI.Graphics;
using Haruka.MonoGameUtils.UI.Screens;
using Microsoft.Xna.Framework;

namespace Haruka.MonoGameUtils.UI.Elements;

public class Dialog : ElementRectangle {
    
    private readonly List<ElementText> options = new List<ElementText>();
    private int selection;

    public Dialog(string message, string[] options = null, int width = 700, int height = 500, bool centerText = false) : base(ExtendedGame.Instance.Width / 2 - width / 2, ExtendedGame.Instance.Height / 2 - height / 2, width, height, ExtendedGame.Instance.Skin.SystemBackgroundColor, true) {
        
        Children.Add(new ElementRectangle(X, Y, width, height, Game.Skin.SystemBorderColor) {
            BorderSize = Game.Skin.DialogBorderSize
        });
        
        int messageHeight = options != null ? RenderUtils.CalculateTextLines(height, Game.Skin.DefaultFontHeight) - options.Length - 1 : height;
        Children.Add(new ElementText(RenderUtils.WrapText(Game.Skin.DefaultFont, message, width, messageHeight), X + Game.Skin.DialogBorderSize + (centerText ? Width / 2 : 0), Y + Game.Skin.DialogBorderSize, centerText ? CenterFlags.CenterY : CenterFlags.NoCenter));
        
        if (options != null) {
            for (int i = options.Length - 1; i >= 0; i--) {
                string option = options[i];
                ElementText row = new ElementText(RenderUtils.WrapText(Game.Skin.DefaultFont, option, width, 1), X + Game.Skin.DialogBorderSize, Y + Game.Skin.DialogBorderSize + height - (Game.Skin.DefaultFontHeight * (i + 1)));
                Children.Add(row);
                this.options.Add(row);
            }

            this.options.Reverse();
        }

        BlockUpdatePropagation = true;
    }

    public bool Open { get; private set; } = true;
    public event Action<int?> OnClose;
    public event Action OnTestButton;
    public int? Result { get; private set; }
    public bool UserClosable { get; set; } = true;

    protected override void UpdateElement(ExtendedGame game, InputManager inputManager, Screen screen, GameTime gameTime) {
        base.UpdateElement(game, inputManager, screen, gameTime);
        if (inputManager.IsAnyJustPressed(Inputs.TEST)) {
            if (OnTestButton != null) {
                OnTestButton.Invoke();
                return;
            }
        }

        if (UserClosable) {
            if (inputManager.IsAnyJustPressed(Inputs.TEST)) {
                Close(selection);
            }

            if (options != null) {
                if (options.Count > 0) {
                    if (inputManager.IsAnyJustPressed(Inputs.SERVICE)) {
                        selection--;
                        if (selection < 0) {
                            selection = options.Count - 1;
                        }
                    }

                    if (inputManager.IsAnyJustPressed(Inputs.UP)) {
                        selection++;
                        if (selection >= options.Count) {
                            selection = 0;
                        }
                    }

                    for (int i = 0; i < options.Count; i++) {
                        if (inputManager.IsJustClicked(options[i])) {
                            if (i == selection) {
                                Close(selection);
                            } else {
                                selection = i;
                            }
                        }
                    }
                }

                for (int i = 0; i < options.Count; i++) {
                    ElementText option = options[i];
                    option.Color = i == selection ? game.Skin.SelectedItemColor : game.Skin.UnselectedItemColor;
                }
            }
        }
    }

    public void SetText(string text) {
        int messageHeight = options != null ? RenderUtils.CalculateTextLines(Height, Game.Skin.DefaultFontHeight) - options.Count - 1 : Height;
        ((ElementText)Children[1]).UpdateText(RenderUtils.WrapText(Game.Skin.DefaultFont, text, Width, messageHeight));
    }

    public virtual void Close(int? result) {
        if (!Open) {
            return;
        }

        Open = false;
        Result = result;
        OnClose?.Invoke(result);
    }
}