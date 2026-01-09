using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAS.Input {
    public interface ICursorAPI : IInputAPI {
        public int GetX();

        public int GetY();

        public bool HasPositionJustChanged();

        public bool IsPressed();

        public bool IsDragging();

        public bool IsJustPressed();

        public bool IsJustReleased();

        public Point[] GetPoints();
        
        public Vector2 GetDragDistance();

        public bool IsInClickOrigin();

    }
}
