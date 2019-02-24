using UnityEngine;
using System.Collections;

namespace Isometric.Interface
{
    public abstract class WorldCursor : InterfaceObject
    {
        public WorldCursor(PlayerInterface menu) : base(menu)
        {
            
        }

        public abstract void CursorButtonDown(Vector2 cursorPosition);

        public abstract void CursorButtonUpdate(Vector2 cursorPosition);

        public abstract void CursorButtonUp(Vector2 cursorPosition);
    }
}