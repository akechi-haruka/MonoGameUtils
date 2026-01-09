using Microsoft.Xna.Framework;

namespace Haruka.MonoGameUtils.Input;

public class SoftInput : ButtonInputAPI {

    private List<Key> prev;
    private List<Key> now;
    private List<Key> queue;

    public override void Bind(Key key, string[] bindings) {
    }

    public override void EarlyKeyUpdate(GameTime time) {
        lock (queue) {
            now.AddRange(queue);
            queue.Clear();
        }
    }

    public override void Initialize() {
        prev = new List<Key>();
        now = new List<Key>();
        queue = new List<Key>();
    }

    public override bool IsJustPressed(Key key) {
        return now.Contains(key) && !prev.Contains(key);
    }

    public override bool IsPressed(Key key) {
        return now.Contains(key);
    }

    public override bool IsReleased(Key key) {
        return !now.Contains(key);
    }

    public override void LateUpdate(GameTime time) {
        lock (queue) {
            prev.Clear();
            prev.AddRange(now);
            now.Clear();
        }
    }

    public void EnqueueButtonPress(Key key) {
        lock (queue) {
            queue.Add(key);
        }
    }

    public override Exception GetError() {
        return null;
    }

    public override DateTime? GetErrorTime() {
        return null;
    }

    public override void ResetError() {
    }
}