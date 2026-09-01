using Haruka.MonoGameUtils.Input;
using Haruka.MonoGameUtils.UI.Graphics.Animators;
using Haruka.MonoGameUtils.UI.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Haruka.MonoGameUtils.UI.Elements;

public abstract class UIElement {
    public Vector2 Position { get; set; }

    public int X {
        get { return (int)Position.X; }
    }

    public int Y {
        get { return (int)Position.Y; }
    }

    public Rectangle Rectangle {
        get {
            return new Rectangle(X, Y, Width, Height);
        }
    }
    
    public int Width { get; set; }
    public int Height { get; set; }

    public bool Visible { get; set; }
    public bool DestroyWhenInvisible { get; set; }
    protected List<IAnimator> Animators { get; } = new List<IAnimator>();
    protected List<UIElement> Children { get; } = new List<UIElement>();
    public bool BlockUpdatePropagation { get; set; }

    protected readonly ExtendedGame Game;

    protected UIElement(int x, int y) {
        Game = ExtendedGame.Instance;
        Position = new Vector2(x, y);
        Visible = true;
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch) {
        DrawElement(gameTime, spriteBatch);
        foreach (UIElement child in Children) {
            if (child.Visible) {
                child.Draw(gameTime, spriteBatch);
            }
        }
    }

    protected abstract void DrawElement(GameTime gameTime, SpriteBatch spriteBatch);

    public void Update(Screen screen, GameTime gameTime) {
        foreach (IAnimator animator in Animators) {
            animator.Update(screen, gameTime);
        }

        UpdateElement(Game, Game.InputManager, screen, gameTime);
        foreach (UIElement child in Children) {
            child.Update(screen, gameTime);
        }
    }

    protected abstract void UpdateElement(ExtendedGame game, InputManager inputManager, Screen screen, GameTime gameTime);

    public void AddAnimator(IAnimator animator) {
        Animators.Add(animator);
    }

    public void RemoveAnimator(IAnimator animator) {
        Animators.Remove(animator);
    }

    public void RemoveAllAnimators() {
        Animators.Clear();
    }

    protected void AddChild(UIElement child) {
        Children.Add(child);
    }
}