using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAS.Input {

    public class MouseInput : ICursorAPI {

        private const int MIN_DISTANCE = 20;

        private MouseState prev;
        private MouseState current;
        private bool drag;
        private Point? dragStart;

        public MouseInput() {
            prev = default;
        }

        public void EarlyUpdate(GameTime time) {
            current = Mouse.GetState();
            if (IsPressed()) {
                if (HasPositionJustChanged()) {
                    drag = true;
                    if (dragStart == null) {
                        dragStart = current.Position;
                    }
                }
            } else {
                drag = false;
                dragStart = null;
            }
        }

        public Vector2 GetDragDistance() {
            if (IsDragging()) {
                Point p = prev.Position - current.Position;
                return new Vector2(p.X, p.Y);
            }
            return Vector2.Zero;
        }

        public Point[] GetPoints() {
            return new Point[] { current.Position };
        }

        public int GetX() {
            return current.X;
        }

        public int GetY() {
            return current.Y;
        }

        public bool HasPositionJustChanged() {
            return prev.Position != current.Position;
        }

        public void Initialize(bool isErrorDisp) {
        }

        public bool IsDragging() {
            return drag;
        }

        public bool IsJustPressed() {
            return current.LeftButton == ButtonState.Pressed && prev.LeftButton == ButtonState.Released;
        }

        public bool IsJustReleased() {
            return current.LeftButton == ButtonState.Released && prev.LeftButton == ButtonState.Pressed;
        }

        public bool IsPressed() {
            return prev.LeftButton == ButtonState.Pressed;
        }

        public void LateUpdate(GameTime time) {
            prev = current;
        }

        public bool IsInClickOrigin() {
            if (dragStart == null) {
                return true;
            }
            Vector2 distance = (current.Position - dragStart.Value).ToVector2();
            return distance.Length() < MIN_DISTANCE;
        }

        public Exception GetError() {
            return null;
        }

        public DateTime? GetErrorTime() {
            return null;
        }

        public void ResetError() {
        }
    }
}
