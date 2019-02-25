using UnityEngine;
using System.Collections.Generic;
using Custom;

namespace Isometric.Interface
{
    public class TargetCursor : WorldCursor
    {
        private List<ITarget> _targets;

        private FSprite[] _sprites;
        
        private ITarget _currentTarget;

        private Vector2 _targetScreenPosition;
        private Vector2 _lastScreenPosition;

        private float _lastTime;

        public TargetCursor(PlayerInterface menu) : base(menu)
        {
            _targets = new List<ITarget>();

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

        public void AddTarget(ITarget target)
        {
            _targets.Add(target);
        }

        public override void CursorUpdate(World world, Player player, Vector2 cursorPosition)
        {
            WorldCamera camera = world.worldCamera;

            if (_targets.Count > 0)
            {
                Vector2 screenPositionA, screenPositionB, mousePosition;
                ITarget nearestTarget = _targets[0];

                screenPositionA = camera.GetScreenPosition(nearestTarget.worldPosition) + camera.worldContainer.GetPosition();

                for (int index = 1; index < _targets.Count; index++)
                {
                    screenPositionB = camera.GetScreenPosition(_targets[index].worldPosition) + camera.worldContainer.GetPosition();

                    mousePosition = Menu.mousePosition;

                    if ((screenPositionA - mousePosition).sqrMagnitude > (screenPositionB - mousePosition).sqrMagnitude)
                    {
                        nearestTarget = _targets[index];
                        screenPositionA = screenPositionB;
                    }
                }

                _targetScreenPosition = screenPositionA;

                if (_currentTarget != nearestTarget)
                {
                    _lastScreenPosition = position;
                    _currentTarget = nearestTarget;
                    _lastTime = menu.time;
                }
            }
        }

        public override void Update(float deltaTime)
        {
            position = Vector2.Lerp(_lastScreenPosition, _targetScreenPosition, CustomMath.Curve((menu.time - _lastTime) * 2f, -5f)) + _currentTarget.boundRect.position;
            size = Vector2.Lerp(size, _currentTarget.boundRect.size, deltaTime * 10f);

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