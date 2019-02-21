using UnityEngine;
using System.Collections.Generic;
using Custom;

namespace Isometric.Interface
{
    public class CameraHUDMenu : Menu
    {
        private WorldCamera _camera;

        private List<ITarget> _targets;

        private TargetVisualizer _targetVisualizer;

        public CameraHUDMenu(WorldCamera camera) : base()
        {
            _camera = camera;

            _targets = new List<ITarget>();

            _targetVisualizer = new TargetVisualizer(this);
            AddElement(_targetVisualizer);
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
                    _lastScreenPosition = _targetVisualizer.position;
                    _currentTarget = nearestTarget;
                    _lastTime = time;
                }

                _targetVisualizer.position = Vector2.Lerp(_lastScreenPosition, screenPositionA, CustomMath.Curve((time - _lastTime) * 2f, -5f)) + _currentTarget.boundRect.position;
                _targetVisualizer.size = Vector2.Lerp(_targetVisualizer.size, _currentTarget.boundRect.size, deltaTime * 10f);
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

    public class TargetVisualizer : InterfaceObject
    {
        private FSprite[] _sprites;

        public TargetVisualizer(Menu menu) : base(menu)
        {
            _sprites = new FSprite[5];
            _sprites[0] = new FSprite("targetcenter");

            FAtlasElement corner = Futile.atlasManager.GetElementWithName("targetcorner");
            _sprites[1] = new FSprite(corner);
            _sprites[2] = new FSprite(corner);
            _sprites[2].scaleX = -1f;
            _sprites[3] = new FSprite(corner);
            _sprites[3].scaleY = -1f;
            _sprites[4] = new FSprite(corner);
            _sprites[4].scaleX = -1f;
            _sprites[4].scaleY = -1f;

            for (int index = 0; index < _sprites.Length; index++)
            {
                AddElement(_sprites[index]);
            }
        }

        public override void Update(float deltaTime)
        {
            Vector2 halfSize = size * 0.5f;

            float sin = Mathf.Sin(menu.time * Mathf.PI * 2f);

            _sprites[1].SetPosition(-halfSize.x - sin, halfSize.y + sin);
            _sprites[2].SetPosition(halfSize.x + sin, halfSize.y + sin);
            _sprites[3].SetPosition(-halfSize.x - sin, -halfSize.y - sin);
            _sprites[4].SetPosition(halfSize.x + sin, -halfSize.y - sin);

            base.Update(deltaTime);
        }
    }
}