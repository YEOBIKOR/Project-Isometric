using UnityEngine;
using System.Collections.Generic;
using Custom;

namespace Isometric.Interface
{
    public class TargetCursor : WorldCursor
    {
        private FSprite[] _sprites;
        
        private ITarget _currentTarget;

        private Vector2 _targetScreenPosition;
        private Vector2 _lastScreenPosition;

        private float _lastTime;

        public TargetCursor(PlayerInterface menu) : base(menu)
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

        public override void CursorUpdate(World world, Player player, Vector2 cursorPosition)
        {
            WorldCamera camera = world.worldCamera;
            List<ITarget> targets = world.targets;

            if (targets.Count > 0)
            {
                Vector2 screenPositionA, screenPositionB, mousePosition;
                ITarget nearestTarget = targets[0];

                screenPositionA = camera.GetScreenPosition(nearestTarget.worldPosition) + camera.worldContainer.GetPosition();

                for (int index = 1; index < targets.Count; index++)
                {
                    screenPositionB = camera.GetScreenPosition(targets[index].worldPosition) + camera.worldContainer.GetPosition();

                    mousePosition = Menu.mousePosition;

                    if ((screenPositionA - mousePosition).sqrMagnitude > (screenPositionB - mousePosition).sqrMagnitude)
                    {
                        nearestTarget = targets[index];
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

                if (Input.GetKey(KeyCode.Mouse0) && _currentTarget != null)
                {
                    RayTrace rayTrace = new RayTrace(_currentTarget.worldPosition, Vector3.zero, Vector3Int.zero);
                    player.UseItem(rayTrace, Input.GetKeyDown(KeyCode.Mouse0));
                }
            }
        }

        public override void Update(float deltaTime)
        {
            position = Vector2.Lerp(_lastScreenPosition, _targetScreenPosition, CustomMath.Curve((menu.time - _lastTime) * 2f, -5f)) + _currentTarget.boundRect.position;
            size = Vector2.Lerp(size, _currentTarget.boundRect.size, deltaTime * 10f);

            Vector2 halfSize = size * 0.5f;

            float sin = Mathf.Sin(menu.time * Mathf.PI * 2f);

            _sprites[0].SetPosition(Vector2.Lerp(Vector2.zero, Menu.mousePosition - position, 0.3f));
            _sprites[1].SetPosition(-halfSize.x - sin, halfSize.y + sin);
            _sprites[2].SetPosition(halfSize.x + sin, halfSize.y + sin);
            _sprites[3].SetPosition(-halfSize.x - sin, -halfSize.y - sin);
            _sprites[4].SetPosition(halfSize.x + sin, -halfSize.y - sin);

            base.Update(deltaTime);
        }
    }
}