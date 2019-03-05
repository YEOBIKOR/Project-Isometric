using Isometric.Interface;
using UnityEngine;
using Custom;

namespace Isometric.Items
{
    public class ItemGun : ItemTool
    {
        private AudioClip _shotAudio;

        public ItemGun(string name, string textureName) : base(name, textureName)
        {
            _shotAudio = Resources.Load<AudioClip>("SoundEffects/GunShot");
        }

        public override void OnUseItem(World world, Player player, RayTrace rayTrace)
        {
            Vector3 shootPosition = player.worldPosition + CustomMath.HorizontalRotate(new Vector3(0.7f, 1f, 0f), player.viewAngle);
            Vector3 targetPosition = rayTrace.hitPosition + Vector3.up + Random.insideUnitSphere;
            Vector3 bulletVelocity = (targetPosition - shootPosition).normalized * 16f;

            world.SpawnEntity(new Bullet(player, new Damage(player, Random.Range(10f, 20f)), bulletVelocity), shootPosition);
            world.worldCamera.worldMicroPhone.PlaySound(_shotAudio, new FixedPosition(player.worldPosition));
            world.worldCamera.ShakeCamera(4f);
        }

        public override float useCoolTime
        {
            get
            { return 0.08f; }
        }

        public override bool repeatableUse
        {
            get
            { return true; }
        }

        public override CursorType cursorType
        {
            get
            { return CursorType.Target; }
        }
    }
}