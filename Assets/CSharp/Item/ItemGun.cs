using System;
using UnityEngine;

namespace ISO.Items
{
    public class ItemGun : ItemTool
    {
        public ItemGun() : base()
        {

        }

        public override void OnUseItem(Player player, RayTrace rayTrace)
        {
            base.OnUseItem(player, rayTrace);

            player.world.SpawnEntity(new EntityBoss(), rayTrace.hitPosition + Vector3.up * 50f);
        }

        public override FAtlasElement element
        {
            get
            { return Futile.atlasManager.GetElementWithName("items/gunak47"); }
        }
    }
}