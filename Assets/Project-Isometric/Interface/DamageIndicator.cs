using UnityEngine;
using System.Collections;

namespace Isometric.Interface
{
    public class DamageIndicator : InterfaceObject
    {
        private WorldCamera _camera;
        private IPositionable _positionable;

        private FLabel _label;
        private FLabel _labelShadow;

        private float _time;

        public DamageIndicator(WorldCamera camera, IPositionable positionable, Damage damage, MenuFlow menu) : base(menu)
        {
            _camera = camera;
            _positionable = positionable;

            string text = ((int)damage.amount).ToString();
            _label = new FLabel("font", text);
            _label.color = Color.red;
            _label.scale = 2f;

            _labelShadow = new FLabel("font", text);
            _labelShadow.scale = 2f;
            _labelShadow.color = Color.black;

            AddElement(_labelShadow);
            AddElement(_label);

            _time = 0f;
        }

        public override void Update(float deltaTime)
        {
            _time += deltaTime;

            if (_time > 5f)
                RemoveSelf();

            Vector2 position = _camera.GetScreenPosition(_positionable.worldPosition) + _camera.worldContainer.GetPosition() + new Vector2(0f, Mathf.Sqrt(_time * 4f) * 16f);

            _label.SetPosition(position);
            _label.alpha = Mathf.Lerp(1f, 0f, _time);

            _labelShadow.SetPosition(position + Vector2.down);
            _labelShadow.alpha = Mathf.Lerp(1f, 0f, _time);

            base.Update(deltaTime);
        }
    }
}