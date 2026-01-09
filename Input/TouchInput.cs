using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;
using OAS.Error;
using OAS.Lib.Touch;
using OAS.Util.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAS.Input {
    public class TouchInput : ICursorAPI {

        public const String TAG = nameof(TouchInput);
        private const int MIN_DISTANCE = 200;

        protected TouchCollection prev;
        protected TouchCollection current;
        protected Vector2? dragStart;

        public TouchInput() {
            prev = default;
        }

        public virtual void Initialize(bool isErrorDisp) {
            if (!TouchPanel.GetCapabilities().IsConnected) {
                Log.WriteError("No touch panel found!", TAG);
                if (!isErrorDisp) {
                    throw new OASException(ErrorDictionary.Get(2000, 1008));
                }
            }
            Log.Write(TouchPanel.GetCapabilities().MaximumTouchCount + " maximum touches", TAG);
        }

        public virtual void EarlyUpdate(GameTime time) {
            current = TouchPanel.GetState();
            if (current.Count > 0) {
                if (current[0].State == TouchLocationState.Pressed) {
                    dragStart = current[0].Position;
                } else if (current[0].State == TouchLocationState.Released) {
                    dragStart = null;
                }
            } 
        }

        public Point[] GetPoints() {
            Point[] arr = new Point[current.Count];
            for (int i = 0; i < current.Count; i++) {
                arr[i] = current[i].Position.ToPoint();
            }
            return arr;
        }

        public TouchCollection GetTouchCollection() {
            return current;
        }

        public int GetX() {
            return (int)(current.Count > 0 ? current[0].Position.X : (prev.Count > 0 ? prev[0].Position.X : 0));
        }

        public int GetY() {
            return (int)(current.Count > 0 ? current[0].Position.Y : (prev.Count > 0 ? prev[0].Position.Y : 0));
        }

        public bool HasPositionJustChanged() {
            return false; // todo
        }

        public bool IsDragging() {
            return current.Count > 0 && current[0].State == TouchLocationState.Moved;
        }

        public bool IsJustPressed() {
            return prev.Count == 0 && current.Count > 0;
        }

        public bool IsJustReleased() {
            return prev.Count > 0 && current.Count == 0;
        }

        public bool IsPressed() {
            return current.Count > 0;
        }

        public void LateUpdate(GameTime time) {
            prev = current;
        }

        public Vector2 GetDragDistance() {
            if (IsDragging()) {
                return prev[0].Position - current[0].Position;
            }
            return Vector2.Zero;
        }

        public bool IsInClickOrigin() {
            return true;// return current.Count > 0 && dragStart != null && (current[0].Position - dragStart.Value).Length() < MIN_DISTANCE; todo buggy af
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
