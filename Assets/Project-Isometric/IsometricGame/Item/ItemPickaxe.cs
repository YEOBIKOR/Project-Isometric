using System;

namespace Isometric.Items
{
    public class ItemPickaxe : ItemTool
    {
        public ItemPickaxe(string name) : base(name)
        {

        }

        public override void OnUseItem(Player player, RayTrace rayTrace)
        {
            base.OnUseItem(player, rayTrace);

            player.world.DestroyBlock(rayTrace.hitTilePosition);
        }

        public override FAtlasElement element
        {
            get
            { return Futile.atlasManager.GetElementWithName("items/pickaxe"); }
        }
    }
}
