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

        private Vector2 _targetSize;
        private Vector2 _lastSize;

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

            ITarget nearestTarget = null;

            Vector2 screenPositionA, screenPositionB;

            screenPositionA = new Vector2(1920f, 1080f);

            for (int index = 0; index < targets.Count; index++)
            {
                screenPositionB = camera.GetScreenPosition(targets[index].worldPosition) + camera.worldContainer.GetPosition();

                float sqrMagnitude = (screenPositionB - cursorPosition).sqrMagnitude;

                //if ((screenPositionB.x > Menu.screenWidth * 0.5f || screenPositionB.x < Menu.screenWidth * -0.5f) ||
                //    (screenPositionB.y > Menu.screenHeight * 0.5f || screenPositionB.y < Menu.screenHeight * -0.5f))
                if (sqrMagnitude > 4096f)
                {
                    continue;
                }

                else if ((screenPositionA - cursorPosition).sqrMagnitude > sqrMagnitude)
                {
                    nearestTarget = targets[index];
                    screenPositionA = screenPositionB;
                }
            }
            
            if (_currentTarget != nearestTarget)
            {
                _lastScreenPosition = position;
                _lastSize = size;

                _currentTarget = nearestTarget;
                _lastTime = menu.time;
            }

            if (_currentTarget != null)
            {
                _targetScreenPosition = screenPositionA + nearestTarget.boundRect.position;
                _targetSize = nearestTarget.boundRect.size;

                if (Input.GetKey(KeyCode.Mouse0))
                {
                    RayTrace rayTrace = new RayTrace(_currentTarget.worldPosition, Vector3.zero, Vector3Int.zero);
                    player.UseItem(rayTrace, Input.GetKeyDown(KeyCode.Mouse0));
                }
            }

            else
            {
                _targetScreenPosition = cursorPosition;
                _targetSize = new Vector2(12f, 12f);
            }
        }

        public override void Update(float deltaTime)
        {
            float t = CustomMath.Curve((menu.time - _lastTime) * 2f, -5f);

            position = Vector2.Lerp(_lastScreenPosition, _targetScreenPosition, t);
            size = Vector2.Lerp(_lastSize, _targetSize, t);

            Vector2 halfSize = size * 0.5f;

            float sin = Mathf.Sin(menu.time * Mathf.PI * 2f);

            _sprites[0].SetPosition(Vector2.Lerp(Vector2.zero, MenuFlow.mousePosition - position, 0.3f));
            _sprites[1].SetPosition(-halfSize.x - sin, halfSize.y + sin);
            _sprites[2].SetPosition(halfSize.x + sin, halfSize.y + sin);
            _sprites[3].SetPosition(-halfSize.x - sin, -halfSize.y - sin);
            _sprites[4].SetPosition(halfSize.x + sin, -halfSize.y - sin);

            base.Update(deltaTime);
        }
    }
}