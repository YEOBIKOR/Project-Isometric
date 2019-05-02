using UnityEngine;
using Isometric.Interface;

namespace Isometric.Items
{
    public class ItemPickaxe : ItemTool
    {
        public ItemPickaxe(string name, string textureName) : base(name, textureName)
        {

        }

        public override void OnUseItem(World world, Player player, ItemContainer itemContainer, Vector3Int tilePosition)
        {
            world.DestroyBlock(tilePosition);
        }

        public override CursorType cursorType
        {
            get
            { return CursorType.Destruct; }
        }
    }
}
