using UnityEngine;
using Isometric.Interface;
using Custom;

namespace Isometric.Items
{
    public class ItemGranadeLauncher : ItemTool
    {
        public ItemGranadeLauncher(string name, string textureName) : base(name, textureName)
        {

        }

        public override void OnUseItem(World world, Player player, RayTrace rayTrace)
        {
            Vector3 shootPosition = player.worldPosition + CustomMath.HorizontalRotate(new Vector3(0.7f, 1f, 0f), player.viewAngle);
            Vector3 targetPosition = rayTrace.hitPosition + Vector3.up + Random.insideUnitSphere;
            Vector3 bulletVelocity = (targetPosition - shootPosition).normalized * 16f;

            world.SpawnEntity(new Granade(player, new Damage(player, Random.Range(10f, 20f)), bulletVelocity), shootPosition);
            world.worldCamera.ShakeCamera(4f);
        }

        public override float useCoolTime
        {
            get
            { return 1f; }
        }

        public override bool repeatableUse
        {
            get
            { return false; }
        }

        public override CursorType cursorType
        {
            get
            { return CursorType.Target; }
        }
    }
}