using System;
using UnityEngine;

namespace ISO.Items
{
    public class ItemBlock : Item
    {
        private Block block;

        public ItemBlock(int itemID, int stackSize) : base(itemID, stackSize)
        {
            this.block = new BlockSolid(itemID);
        }

        public override void OnUseItem(Player player, RayTrace rayTrace)
        {
            base.OnUseItem(player, rayTrace);

            player.world.PlaceBlock(Vector3Int.FloorToInt(rayTrace.hitTilePosition + rayTrace.hitDirection), new BlockSolid(itemID));
        }

        public override FAtlasElement element
        {
            get
            { return block.sprite; }
        }
    }
}