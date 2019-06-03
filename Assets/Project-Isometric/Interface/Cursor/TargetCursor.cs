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

        private TargetInspector _targetInspector;

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

            _targetInspector = new TargetInspector(menu);
            _targetInspector.position = new Vector2(0f, 32f);

            AddElement(_targetInspector);
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

                _targetInspector.InspectTarget(_currentTarget);
            }

            if (_currentTarget != null)
            {
                _targetScreenPosition = screenPositionA + nearestTarget.boundRect.position;
                _targetSize = nearestTarget.boundRect.size;

                if (Input.GetKey(KeyCode.Mouse0))
                {
                    player.UseItem(_currentTarget.worldPosition, Input.GetKeyDown(KeyCode.Mouse0));
                }
            }

            else
            {
                _targetScreenPosition = cursorPosition;
                _targetSize = new Vector2(12f, 12f);

                if (Input.GetKey(KeyCode.Mouse0))
                {
                    RayTrace rayTrace = camera.GetRayAtScreenPosition(cursorPosition - camera.worldContainer.GetPosition());

                    Vector3 targetPosition = new Vector3(rayTrace.hitPosition.x, player.worldPosition.y, rayTrace.hitPosition.z);

                    if (rayTrace.hit)
                        player.UseItem(targetPosition, Input.GetKeyDown(KeyCode.Mouse0));
                }
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

    public class TargetInspector : InterfaceObject
    {
        private ShadowedLabel _inspectLabel;

        private FSprite _bar;
        private FSprite _barCase;

        private EntityCreature _target;

        private float _lastLength;

        public TargetInspector(MenuFlow menu) : base(menu)
        {
            _inspectLabel = new ShadowedLabel(menu, string.Empty);
            _inspectLabel.position = new Vector2(0f, 10f);

            _bar = new FSprite("uipixel");
            _bar.scaleY = 4f;

            _barCase = new FSprite("targethealth");
        }

        public void InspectTarget(ITarget target)
        {
            EntityCreature creature = target as EntityCreature;

            if (creature != null)
            {
                _inspectLabel.text = string.Concat(creature.name);

                _lastLength = creature.health / creature.maxHealth;
                
                AddElement(_inspectLabel);
                AddElement(_bar);
                AddElement(_barCase);
            }
            
            else
            {
                RemoveElement(_inspectLabel);
                RemoveElement(_bar);
                RemoveElement(_barCase);
            }

            _target = creature;
        }

        public override void Update(float deltaTime)
        {
            if (_target != null)
            {
                _lastLength = Mathf.Lerp(_lastLength, _target.health / _target.maxHealth, deltaTime * 10f);

                _bar.x = _lastLength * 24f - 24f;
                _bar.scaleX = _lastLength * 48f;
                _bar.color = Color.Lerp(Color.red, new Color32(0xAC, 0xEF, 0x2A, 0xFF), _lastLength);
            }

            base.Update(deltaTime);
        }
    }
}