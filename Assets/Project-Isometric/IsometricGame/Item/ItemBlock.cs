using System;
using UnityEngine;

namespace Isometric.Items
{
    public class ItemBlock : Item
    {
        private Block block;

        public ItemBlock(string name, string blockKey) : base(name)
        {
            this.block = Block.GetBlockByKey(blockKey);
        }

        public override void OnUseItem(Player player, RayTrace rayTrace)
        {
            base.OnUseItem(player, rayTrace);

            player.world.PlaceBlock(Vector3Int.FloorToInt(rayTrace.hitTilePosition + rayTrace.hitDirection), block);
        }

        public override FAtlasElement element
        {
            get
            { return block.sprite; }
        }
    }
}