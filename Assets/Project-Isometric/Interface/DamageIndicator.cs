using UnityEngine;
using System.Collections;

namespace Isometric.Interface
{
    public class DamageIndicator : InterfaceObject
    {
        private WorldCamera _camera;
        private IPositionable _positionable;

        private ShadowedLabel _label;

        private float _time;

        private Color[] _colors;

        public DamageIndicator(WorldCamera camera, IPositionable positionable, Damage damage, MenuFlow menu) : base(menu)
        {
            _camera = camera;
            _positionable = positionable;

            string text = ((int)damage.amount).ToString();
            _label = new ShadowedLabel(menu, text);
            _label.scale = Vector2.one * 3f;

            AddElement(_label);

            _time = 0f;

            _colors = new Color[] { Color.white, Color.red, Color.yellow };
        }

        public override void Update(float deltaTime)
        {
            _time += deltaTime;

            if (_time > 5f)
                RemoveSelf();

            Vector2 targetScreenPosition = _camera.GetScreenPosition(_positionable.worldPosition) + _camera.worldContainer.GetPosition();
            Vector2 offset = new Vector2(0f, Mathf.Sqrt(_time * 4f) * 16f);
            Vector2 shakeoffset = Random.insideUnitCircle * Mathf.Max((0.5f - _time) * 10f, 0f);

            Vector2 position = targetScreenPosition + offset + shakeoffset;

            _label.position = position;
            _label.alpha = Mathf.Clamp01(1.5f - _time);

            if (_time < 0.5f)
                _label.color = _colors[Random.Range(0, 3)];
            else
                _label.color = _colors[0];

            base.Update(deltaTime);
        }
    }
}