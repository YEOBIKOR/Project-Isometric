using UnityEngine;
using System.Collections;

namespace Isometric.Interface
{
    public abstract class WorldCursor : InterfaceObject
    {
        public WorldCursor(PlayerInterface menu) : base(menu)
        {
            
        }

        public abstract void CursorUpdate(World world, Player player, Vector2 cursorPosition);
    }
}