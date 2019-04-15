using System;
using UnityEngine;
using Isometric.Items;

namespace Isometric.Interface
{
    public class HealthBar : InterfaceObject
    {
        private EntityCreature owner;

        private EntityPart backGround;
        private EntityPart foreGround;

        public HealthBar(EntityCreature owner, MenuFlow menu) : base(menu)
        {
            this.owner = owner;

            FAtlasElement element = Futile.atlasManager.GetElementWithName("uipixel");

            backGround = new EntityPart(owner, element);
            foreGround = new EntityPart(owner, element);

            backGround.scale = new Vector2(24f, 6f);
            backGround.color = Color.black;

            SetHealthValue(owner.health);
        }

        public void Show()
        {
            owner.world.AddCosmeticDrawble(backGround);
            owner.world.AddCosmeticDrawble(foreGround);
        }

        public void Hide()
        {
            backGround.Erase();
            foreGround.Erase();
        }

        public void SetHealthValue(float value)
        {
            foreGround.positionOffset = new Vector2(Mathf.Lerp(-11f, 0f, owner.health / owner.maxHealth), 0f);
            foreGround.scale = new Vector2(22f * owner.health / owner.maxHealth, 4f);
            foreGround.color = Color.Lerp(Color.red, new Color32(0xAC, 0xEF, 0x2A, 0xFF), owner.health / owner.maxHealth);
        }

        public void Update(float deltaTime)
        {
            backGround.worldPosition = owner.worldPosition + Vector3.up * 3f;
            foreGround.worldPosition = owner.worldPosition + Vector3.up * 3f;
        }
    }
}
