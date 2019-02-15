using UnityEngine;
using System.Collections;

namespace Isometric.Interface
{
    public class DamageIndicator : InterfaceObject
    {
        private Damage _damage;

        private FLabel _label;

        private float _time;

        public DamageIndicator(Menu menu, Damage damage) : base(menu)
        {
            _damage = damage;

            _label = new FLabel("font", damage.amount.ToString());
            _label.scale = 1.2f;

            AddElement(_label);

            _time = 0f;
        }

        public override void Update(float deltaTime)
        {
            _time += deltaTime;

            _label.SetPosition(0f, Mathf.Sqrt(_time * 4f) * 16f);
            _label.alpha = Mathf.Lerp(1f, 0f, _time);

            base.Update(deltaTime);
        }
    }
}