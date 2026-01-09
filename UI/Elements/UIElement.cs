using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OAS.Input;
using OAS.UI.Graphics.Animators;
using OAS.UI.Screens;
using System.Collections.Generic;

namespace OAS.UI.Elements {

    public abstract class UIElement {

        public Vector2 Position { get; private set; }
        public float X => Position.X;
        public float Y => Position.Y;
        public bool Visible { get; set; }
        public bool DestroyWhenInvisible { get; set; }
        protected List<IAnimator> Animators { get; set; } = new List<IAnimator>();
        protected List<UIElement> Children { get; set; } = new List<UIElement>();
        protected Program game;

        protected UIElement(int x, int y) {
            game = Program.Main;
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
            UpdateElement(game, game.InputManager, screen, gameTime);
            foreach (UIElement child in Children) {
                child.Update(screen, gameTime);
            }
        }

        protected abstract void UpdateElement(Program game, InputManager inputManager, Screen screen, GameTime gameTime);

        public abstract void SetWidth(int width);

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
}
