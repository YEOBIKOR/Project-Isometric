using UnityEngine;

namespace Isometric.Interface
{
    public class ChatBubble : InterfaceObject
    {
        private WorldCamera _camera;

        private IPositionable _behaviour;
        private string _text;
        private float _duration;

        private float _time;

        private FLabel _label;
        private RoundedRect _rect;

        const float SpeechSpeed = 24f;

        public ChatBubble(WorldCamera camera, IPositionable behaviour, string text, MenuFlow menu) : base(menu)
        {
            _camera = camera;

            _behaviour = behaviour;
            _text = text;
            _duration = 10f;

            _rect = new RoundedRect(menu, true);
            _label = new FLabel("font", string.Empty);

            AddElement(_rect);
            AddElement(_label);
        }

        public override void Update(float deltaTime)
        {
            _time = _time + deltaTime;

            position = _camera.GetScreenPosition(_behaviour.worldPosition) + _camera.worldContainer.GetPosition() + new Vector2(0f, 48f); // + Mathf.Sin(_time * 12f) * 0.5f);

            if (_time * SpeechSpeed < _text.Length + 1)
                _label.text = _text.Substring(0, (int)(_time * SpeechSpeed));

            _rect.size = _label.textRect.size;

            if (_time > _duration)
            {
                float factor = Mathf.Clamp01(_time - _duration);

                if (factor < 1f)
                    container.alpha = Mathf.Clamp01(1f - factor);
                else
                    RemoveSelf();
            }

            base.Update(deltaTime);
        }
    }
}
