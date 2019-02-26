using Isometric.Interface;
using UnityEngine;
using Custom;

namespace Isometric.Items
{
    public class ItemGun : ItemTool
    {
        public ItemGun(string name, string textureName) : base(name, textureName)
        {

        }

        public override CursorType cursorType
        {
            get
            { return CursorType.Target; }
        }

        public override void OnUseItem(World world, Player player, RayTrace rayTrace)
        {
            Vector3 shootPosition = player.worldPosition + CustomMath.HorizontalRotate(new Vector3(0.7f, 1f, 0f), player.viewAngle);
            world.SpawnEntity(new Bullet(new Damage(player), (rayTrace.hitPosition - shootPosition).normalized * 25f), shootPosition);
        }
    }
}