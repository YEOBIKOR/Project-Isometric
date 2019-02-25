using UnityEngine;
using Isometric.Interface;

namespace Isometric.Items
{
    public class ItemPickaxe : ItemTool
    {
        public ItemPickaxe(string name, string textureName) : base(name, textureName)
        {

        }

        public override void OnUseItem(World world, Player player, RayTrace rayTrace)
        {
            base.OnUseItem(world, player, rayTrace);

            world.DestroyBlock(Vector3Int.FloorToInt(rayTrace.hitTilePosition));
        }

        public override CursorType cursorType
        {
            get
            { return CursorType.Construct; }
        }
    }
}
