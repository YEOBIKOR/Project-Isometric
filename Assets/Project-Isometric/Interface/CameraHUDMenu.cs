using UnityEngine;
using System.Collections.Generic;
using Custom;

namespace Isometric.Interface
{
    public class CameraHUDMenu : Menu
    {
        private WorldCamera _camera;

        private List<ITarget> _targets;

        private GeneralButton _rect;

        public CameraHUDMenu(WorldCamera camera) : base()
        {
            _camera = camera;

            _targets = new List<ITarget>();

            _rect = new GeneralButton(this, string.Empty, null);
            AddElement(_rect);
        }

        private ITarget _currentTarget;
        private Vector2 _lastScreenPosition;
        private float _lastTime;

        public override void Update(float deltaTime)
        {
            if (_targets.Count > 0)
            {
                Vector2 screenPositionA, screenPositionB, mousePosition;
                ITarget nearestTarget = _targets[0];
                
                screenPositionA = _camera.GetScreenPosition(nearestTarget.worldPosition) + _camera.worldContainer.GetPosition();

                for (int index = 1; index < _targets.Count; index++)
                {
                    screenPositionB = _camera.GetScreenPosition(_targets[index].worldPosition) + _camera.worldContainer.GetPosition();

                    mousePosition = Menu.mousePosition;

                    if ((screenPositionA - mousePosition).sqrMagnitude > (screenPositionB - mousePosition).sqrMagnitude)
                    {
                        nearestTarget = _targets[index];
                        screenPositionA = screenPositionB;
                    }
                }

                if (_currentTarget != nearestTarget)
                {
                    _lastScreenPosition = _rect.position;
                    _currentTarget = nearestTarget;
                    _lastTime = time;
                }

                _rect.position = Vector2.Lerp(_lastScreenPosition, screenPositionA, CustomMath.Curve((time - _lastTime) * 2f, -5f)) + _currentTarget.boundRect.position;
                _rect.size = Vector2.Lerp(_rect.size, _currentTarget.boundRect.size, deltaTime * 10f);
            }

            base.Update(deltaTime);
        }

        public void Speech(IPositionable behaviour, string text)
        {
            SpeechBubble bubble = new SpeechBubble(_camera, behaviour, text, this);
            AddElement(bubble);
        }

        public void AddTarget(ITarget target)
        {
            _targets.Add(target);
        }

        public void IndicateDamage(Damage damage, Vector3 position)
        {
            DamageIndicator indicator = new DamageIndicator(this, damage);
            indicator.position = _camera.GetScreenPosition(position) + _camera.worldContainer.GetPosition();

            AddElement(indicator);
        }
    }
}