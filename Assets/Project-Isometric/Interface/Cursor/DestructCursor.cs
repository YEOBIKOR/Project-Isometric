using UnityEngine;
using System.Collections;
using Isometric.Items;

namespace Isometric.Interface
{
    public class DestructCursor : WorldCursor
    {
        private WorldCamera _worldCamera;

        private float _progress;

        private FAtlasElement[] _destroyElements;

        private FSprite _previewSprite;

        private Tile _lastTile;

        public DestructCursor(PlayerInterface menu, WorldCamera worldCamera) : base(menu)
        {
            _worldCamera = worldCamera;

            _progress = 0f;

            _destroyElements = new FAtlasElement[8];

            for (int index = 0; index < _destroyElements.Length; index++)
            {
                _destroyElements[index] = Futile.atlasManager.GetElementWithName("destroy" + index);
            }

            _previewSprite = new FSprite(_destroyElements[0]);
        }

        public override void OnActivate()
        {
            base.OnActivate();

            _worldCamera.worldContainer.AddChild(_previewSprite);
        }

        public override void OnDeactivate()
        {
            _previewSprite.RemoveFromContainer();

            base.OnDeactivate();
        }

        public override void Update(float deltaTime)
        {
            if (Input.GetKey(KeyCode.Mouse0))
                _progress += deltaTime;
            else
                _progress = 0f;

            base.Update(deltaTime);
        }

        public override void CursorUpdate(World world, Player player, Vector2 cursorPosition)
        {
            WorldCamera camera = world.worldCamera;

            if (!camera.turning)
            {
                RayTrace rayTrace = camera.GetRayAtScreenPosition(cursorPosition - camera.worldContainer.GetPosition());

                Tile tile = rayTrace.GetTileIn(world);

                if (tile != _lastTile)
                {
                    _progress = 0f;
                    _lastTile = tile;
                }

                if (rayTrace.hit)
                {
                    bool inRange = (rayTrace.hitPosition - player.worldPosition).sqrMagnitude < 25f;

                    if (Input.GetKey(KeyCode.Mouse0) && inRange)
                    {
                        bool clicked = _progress >= 1f;

                        if (clicked)
                            _progress = 0f;

                        player.UseItem(rayTrace.hitTilePosition, clicked); // Input.GetKeyDown(KeyCode.Mouse0));
                    }

                    SetConstructionGuide(camera, player, rayTrace.hitTilePosition, inRange, _progress);
                    _previewSprite.isVisible = true;
                }
            }
        }

        public void SetConstructionGuide(WorldCamera worldCamera, Player player, Vector3Int tilePosition, bool inRange, float progress)
        {
            Vector3 worldPosition = tilePosition + Vector3.one * 0.5f;

            _previewSprite.SetPosition(worldCamera.GetScreenPosition(worldPosition));
            _previewSprite.sortZ = worldCamera.GetSortZ(worldPosition) + 0.1f;

            // _previewSprite.color = Color.Lerp(inRange ? Color.cyan : Color.red, Color.black, progress);
            // _previewSprite.alpha = Mathf.Sin(menu.time * Mathf.PI) * 0.25f + 0.5f;

            _previewSprite.alpha = progress > 0f ? 1f : 0f;
            _previewSprite.element = _destroyElements[Mathf.FloorToInt(Mathf.Clamp01(progress) * 7f)];    
        }
    }
}