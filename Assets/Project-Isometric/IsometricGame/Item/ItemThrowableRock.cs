using System;
using UnityEngine;
using Custom;

namespace Isometric.Items
{
    public class ItemThrowableRock : ItemTool
    {
        public ItemThrowableRock(string name, string textureName) : base(name, textureName)
        {

        }

        public override void OnUseItem(World world, Player player, ItemContainer itemContainer, Vector3 targetPosition)
        {
            ThrowableRock rock = new ThrowableRock(player);

            Vector3 playerShotPosition = player.worldPosition + Vector3.up + (targetPosition - player.worldPosition).normalized * 0.5f;
            Vector3 shotDirection = (targetPosition + Vector3.up - playerShotPosition).normalized;

            rock.velocity = player.velocity + shotDirection * 30f;
            world.SpawnEntity(rock, playerShotPosition);
        }

        public override float useCoolTime
        {
            get
            {
                return 0.3f;
            }
        }
    }
}