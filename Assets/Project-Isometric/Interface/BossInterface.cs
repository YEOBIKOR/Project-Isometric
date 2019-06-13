using UnityEngine;

namespace Isometric.Interface
{
    public class BossInterface : MenuFlow
    {
        private EntityCreature _owner;

        private FSprite _bar;
        private FSprite _barCase;

        private FContainer _barContainer;

        private float _lastLength;

        public BossInterface(EntityCreature owner) : base()
        {
            _owner = owner;

            _bar = new FSprite("uipixel");
            _bar.scaleY = 8f;

            _barCase = new FSprite("bosshealth");

            _barContainer = new FContainer();
            _barContainer.SetPosition(0f, MenuFlow.screenHeight * 0.5f - 16f);

            _barContainer.AddChild(_bar);
            _barContainer.AddChild(_barCase);

            container.AddChild(_barContainer);
        }

        public override void Update(float deltaTime)
        {
            _lastLength = Mathf.Lerp(_lastLength, _owner.health / _owner.maxHealth, deltaTime * 10f);

            _bar.x = _lastLength * 160f - 160f;
            _bar.scaleX = _lastLength * 320f;
            _bar.color = Color.Lerp(Color.red, new Color32(0xAC, 0xEF, 0x2A, 0xFF), _lastLength);

            base.Update(deltaTime);
        }
    }
}