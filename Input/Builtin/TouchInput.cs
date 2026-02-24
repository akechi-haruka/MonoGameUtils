using Haruka.MonoGameUtils.Input.Api;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;

namespace Haruka.MonoGameUtils.Input.Builtin;

public class TouchInput : ICursorAPI {

    private const int MIN_DISTANCE = 200;

    protected TouchCollection Prev;
    protected TouchCollection Current;
    protected Vector2? DragStart;

    public virtual void EarlyUpdate(GameTime time) {
        Current = TouchPanel.GetState();
        if (Current.Count > 0) {
            if (Current[0].State == TouchLocationState.Pressed) {
                DragStart = Current[0].Position;
            } else if (Current[0].State == TouchLocationState.Released) {
                DragStart = null;
            }
        } 
    }

    public Point[] GetPoints() {
        Point[] arr = new Point[Current.Count];
        for (int i = 0; i < Current.Count; i++) {
            arr[i] = Current[i].Position.ToPoint();
        }
        return arr;
    }

    public TouchCollection GetTouchCollection() {
        return Current;
    }

    public int GetX() {
        return (int)(Current.Count > 0 ? Current[0].Position.X : (Prev.Count > 0 ? Prev[0].Position.X : 0));
    }

    public int GetY() {
        return (int)(Current.Count > 0 ? Current[0].Position.Y : (Prev.Count > 0 ? Prev[0].Position.Y : 0));
    }

    public bool HasPositionJustChanged() {
        return false; // todo
    }

    public bool IsDragging() {
        return Current.Count > 0 && Current[0].State == TouchLocationState.Moved;
    }

    public bool IsJustPressed() {
        return Prev.Count == 0 && Current.Count > 0;
    }

    public bool IsJustReleased() {
        return Prev.Count > 0 && Current.Count == 0;
    }

    public bool IsPressed() {
        return Current.Count > 0;
    }

    public void LateUpdate(GameTime time) {
        Prev = Current;
    }

    public Vector2 GetDragDistance() {
        if (IsDragging()) {
            return Prev[0].Position - Current[0].Position;
        }
        return Vector2.Zero;
    }

    public bool IsInClickOrigin() {
        return true;// return current.Count > 0 && dragStart != null && (current[0].Position - dragStart.Value).Length() < MIN_DISTANCE; todo buggy af
    }

    public void Initialize() {
        
    }

    public void Start() {
        if (!TouchPanel.GetCapabilities().IsConnected) {
            InputManager.InputLog.LogError("No touch panel found!");
            throw new InputException("No touch panel found!");
        }
        InputManager.InputLog.LogInformation(TouchPanel.GetCapabilities().MaximumTouchCount + " maximum touches");
    }

    public void Stop() {
    }
}