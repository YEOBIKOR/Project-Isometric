using Isometric.Interface;
using UnityEngine;
using Custom;

namespace Isometric.Items
{
    public class ItemGun : ItemTool
    {
        private AudioClip _shotAudio;

        private float _useCoolTime;
        private bool _repeatableUse;

        public ItemGun(string name, string textureName, float useCoolTime, bool repeatableUse) : base(name, textureName)
        {
            _useCoolTime = useCoolTime;
            _repeatableUse = repeatableUse;

            _shotAudio = Resources.Load<AudioClip>("SoundEffects/GunShot");
        }

        public override void OnUseItem(World world, Player player, ItemContainer itemContainer, Vector3 targetPosition)
        {
            Vector3 shootPosition = player.worldPosition + CustomMath.HorizontalRotate(new Vector3(1.5f, 1f, 0f), player.viewAngle);
            Vector3 bulletTargetPosition = targetPosition + Vector3.up + Random.insideUnitSphere;
            Vector3 bulletVelocity = (bulletTargetPosition - shootPosition).normalized * 24f;

            world.SpawnEntity(new Bullet(player, new Damage(player, Random.Range(10f, 20f)), bulletVelocity), shootPosition);
            world.worldMicrophone.PlaySound(_shotAudio, new FixedPosition(player.worldPosition));
            world.worldCamera.ShakeCamera(2f);
        }

        public override float useCoolTime
        {
            get
            { return _useCoolTime; }
        }

        public override bool repeatableUse
        {
            get
            { return _repeatableUse; }
        }

        public override CursorType cursorType
        {
            get
            { return CursorType.Target; }
        }
    }
}