using UnityEngine;
using System.Collections;

namespace Isometric.Interface
{
    public class DamageIndicator : InterfaceObject
    {
        private Damage _damage;

        private FLabel _label;
        private FLabel _labelShadow;

        private float _time;

        public DamageIndicator(MenuFlow menu, Damage damage) : base(menu)
        {
            _damage = damage;

            string text = ((int)damage.amount).ToString();
            _label = new FLabel("font", text);
            _label.scale = 1.2f;

            _labelShadow = new FLabel("font", text);
            _labelShadow.scale = 1.2f;
            _labelShadow.color = Color.black;

            AddElement(_labelShadow);
            AddElement(_label);

            _time = 0f;
        }

        public override void Update(float deltaTime)
        {
            _time += deltaTime;

            if (_time > 3f)
                RemoveSelf();

            Vector2 position = new Vector2(0f, Mathf.Sqrt(_time * 4f) * 16f);

            _label.SetPosition(position);
            _label.alpha = Mathf.Lerp(1f, 0f, _time);

            _labelShadow.SetPosition(position + Vector2.down);
            _labelShadow.alpha = Mathf.Lerp(1f, 0f, _time);

            base.Update(deltaTime);
        }
    }
}