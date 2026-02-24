using Haruka.MonoGameUtils.Input;
using Haruka.MonoGameUtils.UI.Graphics.Animators;
using Haruka.MonoGameUtils.UI.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Haruka.MonoGameUtils.UI.Elements;

public abstract class UIElement {
    public Vector2 Position { get; private set; }

    public float X {
        get { return Position.X; }
    }

    public float Y {
        get { return Position.Y; }
    }

    public bool Visible { get; set; }
    public bool DestroyWhenInvisible { get; set; }
    protected List<IAnimator> Animators { get; set; } = new List<IAnimator>();
    protected List<UIElement> Children { get; set; } = new List<UIElement>();
    public bool BlockUpdatePropagation { get; set; }

    protected readonly ExtendedGame Game;

    protected UIElement(int x, int y) {
        Game = ExtendedGame.Instance;
        Position = new Vector2(x, y);
        Visible = true;
    }

    public abstract int GetWidth();

    public abstract int GetHeight();

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

    public abstract void SetWidth(int w);

    public abstract void SetHeight(int height);

    public abstract Rectangle GetRect();

    public int GetX() {
        return (int)X;
    }

    public int GetY() {
        return (int)Y;
    }

    public virtual void SetPosition(Vector2 position) {
        Position = position;
    }

    public void SetPosition(Point position, int xoff = 0, int yoff = 0) {
        SetPosition(new Vector2(position.X + xoff, position.Y + yoff));
    }

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