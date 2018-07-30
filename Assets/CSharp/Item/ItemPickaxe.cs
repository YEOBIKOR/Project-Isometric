using System;

namespace ISO.Items
{
    public class ItemPickaxe : ItemTool
    {
        public ItemPickaxe() : base()
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
