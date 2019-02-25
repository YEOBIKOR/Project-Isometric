﻿using UnityEngine;
using System.Collections;
using Isometric.Items;

namespace Isometric.Interface
{
    public class ConstructCursor : WorldCursor
    {
        private WorldCamera _worldCamera;

        private FSprite _previewSprite;

        public ConstructCursor(PlayerInterface menu, WorldCamera worldCamera) : base(menu)
        {
            _worldCamera = worldCamera;

            _previewSprite = new FSprite(Item.GetItemByID(0).element);
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

        public override void CursorUpdate(World world, Player player, Vector2 cursorPosition)
        {
            WorldCamera camera = world.worldCamera;

            if (!camera.turning)
            {
                RayTrace rayTrace = camera.GetRayAtScreenPosition(Menu.mousePosition - camera.worldContainer.GetPosition());

                if (rayTrace.hit)
                {
                    bool available = false;
                    if (player.pickItemStack != null)
                        available = player.pickItemStack.item is ItemBlock || player.pickItemStack.item is ItemPickaxe;
                    bool inRange = (rayTrace.hitPosition - player.worldPosition).sqrMagnitude < 25f;

                    if (Input.GetKey(KeyCode.Mouse0) && (inRange || !available))
                        player.UseItem(rayTrace, Input.GetKeyDown(KeyCode.Mouse0));

                    SetConstructionGuide(camera, player, rayTrace.hitTilePosition + Vector3.one * 0.5f, rayTrace.hitDirection, inRange);
                    _previewSprite.isVisible = true;
                }
            }
        }

        public void SetConstructionGuide(WorldCamera worldCamera, Player player, Vector3 worldPosition, Vector3 hitDirection, bool inRange)
        {
            _previewSprite.SetPosition(worldCamera.GetScreenPosition(worldPosition + hitDirection));
            _previewSprite.sortZ = worldCamera.GetSortZ(worldPosition) + 0.1f;

            _previewSprite.color = inRange ? Color.cyan : Color.red;
            _previewSprite.alpha = Mathf.Sin(menu.time * Mathf.PI) * 0.25f + 0.5f;

            _previewSprite.element = player.pickItemStack.item.element;
        }
    }
}