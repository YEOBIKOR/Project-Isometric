using System;
using UnityEngine;

namespace Isometric.Items
{
    public class ItemGun : ItemTool
    {
        public ItemGun(string name, string textureName) : base(name, textureName)
        {

        }

        public override void OnUseItem(Player player, RayTrace rayTrace)
        {
            base.OnUseItem(player, rayTrace);

            player.world.SpawnEntity(new EntityBoss(), rayTrace.hitPosition + Vector3.up * 50f);
        }
    }
}