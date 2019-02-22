using System;

namespace Isometric.Items
{
    public class ItemPickaxe : ItemTool
    {
        public ItemPickaxe(string name, string textureName) : base(name, textureName)
        {

        }

        public override void OnUseItem(Player player, RayTrace rayTrace)
        {
            base.OnUseItem(player, rayTrace);

            player.world.DestroyBlock(rayTrace.hitTilePosition);
        }
    }
}
