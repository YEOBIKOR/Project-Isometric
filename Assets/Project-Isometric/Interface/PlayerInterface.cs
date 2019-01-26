using UnityEngine;
using Isometric.Items;
using Custom;

namespace Isometric.Interface
{
    public class PlayerInterface : Menu
    {
        private Player _player;
        public Player player
        {
            get { return _player; }
        }
        public WorldCamera worldCamera
        {
            get { return _player.worldCamera; }
        }

        private ItemSelect itemSelect;
        private InventoryMenu inventoryMenu;

        private FSprite cursorSprite;
        private FAtlasElement[] guideSprites;

        public PlayerInterface(Player player) : base()
        {
            _player = player;

            itemSelect = new ItemSelect(this);
            inventoryMenu = new InventoryMenu(this);
            
            guideSprites = new FAtlasElement[3];
            guideSprites[0] = Futile.atlasManager.GetElementWithName("constructguidex");
            guideSprites[1] = Futile.atlasManager.GetElementWithName("constructguidey");
            guideSprites[2] = Futile.atlasManager.GetElementWithName("constructguidez");

            cursorSprite = new FSprite(guideSprites[0]);
        }

        public override void OnActivate()
        {
            base.OnActivate();

            worldCamera.worldContainer.AddChild(cursorSprite);
        }

        public override void Update(float deltaTime)
        {
            if (!inventoryMenu.activated)
            {
                if (Input.mouseScrollDelta.y != 0f && !itemSelect.activated)
                    AddSubLoopFlow(itemSelect);
                if (Input.GetKey(KeyCode.I))
                    AddSubLoopFlow(inventoryMenu);

                cursorSprite.isVisible = false;

                if (!worldCamera.turning)
                {
                    RayTrace rayTrace = worldCamera.GetRayAtScreenPosition(Menu.mousePosition - worldCamera.worldContainer.GetPosition());

                    if (rayTrace.hit)
                    {
                        bool inRange = (rayTrace.hitPosition - player.worldPosition).sqrMagnitude < 25f || !(player.pickItemStack.item is ItemBlock || player.pickItemStack.item is ItemPickaxe);

                        if (Input.GetKey(KeyCode.Mouse0) && inRange)
                            player.UseItem(rayTrace, Input.GetKeyDown(KeyCode.Mouse0));

                        SetConstructionGuide(rayTrace.hitTilePosition + Vector3.one * 0.5f, rayTrace.hitDirection, inRange);
                        cursorSprite.isVisible = true;
                    }
                }
            }

            base.Update(deltaTime);
        }

        public void SetConstructionGuide(Vector3 worldPosition, Vector3 hitDirection, bool inRange)
        {
            cursorSprite.SetPosition(worldCamera.GetScreenPosition(worldPosition));
            cursorSprite.sortZ = worldCamera.GetSortZ(worldPosition) + 0.1f;

            cursorSprite.color = inRange ? Color.white : Color.red;

            if (hitDirection == Vector3.up)
                cursorSprite.element = guideSprites[1];
            else
            {
                switch (worldCamera.viewDirection)
                {
                    case CameraViewDirection.NE:
                        cursorSprite.element = guideSprites[hitDirection == Vector3.left ? 0 : 2];
                        break;

                    case CameraViewDirection.NW:
                        cursorSprite.element = guideSprites[hitDirection == Vector3.back ? 0 : 2];
                        break;

                    case CameraViewDirection.SE:
                        cursorSprite.element = guideSprites[hitDirection == Vector3.forward ? 0 : 2];
                        break;

                    case CameraViewDirection.SW:
                        cursorSprite.element = guideSprites[hitDirection == Vector3.right ? 0 : 2];
                        break;
                }
            }
        }

        public class ItemSelect : Menu
        {
            public const int length = 8;

            private PlayerInterface playerInterface;

            public Player player
            {
                get
                { return playerInterface.player; }
            }

            private FSprite selectSprite;
            private ItemContainerVisualizer[] visualizer;
            private int selectedIndex;

            private float factor;
            private float sleepTime;

            private float scrollAmount;
            private float selectSpriteAngle;

            public ItemSelect(PlayerInterface playerInterface) : base()
            {
                this.playerInterface = playerInterface;

                selectSprite = new FSprite("itemselect");
                container.AddChild(selectSprite);

                visualizer = new ItemContainerVisualizer[length];
                for (int index = 0; index < length; index++)
                {
                    visualizer[index] = new ItemContainerVisualizer(this, player.inventory[index]);
                    AddElement(visualizer[index]);
                }

                factor = 0f;
                sleepTime = 0f;
                scrollAmount = 0f;
                selectSpriteAngle = 0f;
            }

            public override void Update(float deltaTime)
            {
                Vector2 anchorPosition = player.screenPosition + Vector2.up * 16f;
                float scrollDelta = Input.mouseScrollDelta.y;

                sleepTime = scrollDelta != 0 ? 1f : sleepTime - deltaTime;
                factor = Mathf.Clamp01(factor + (sleepTime > 0f ? deltaTime : -deltaTime) * 5f);

                if (!(factor > 0f))
                    Terminate();

                scrollAmount += scrollDelta;

                int oldSelectedIndex = selectedIndex;
                selectedIndex = (int)Mathf.Repeat(scrollAmount, length);

                if (oldSelectedIndex != selectedIndex)
                    player.PickItem(player.inventory[selectedIndex]);

                selectSpriteAngle = Mathf.LerpAngle(selectSpriteAngle, (float)selectedIndex / length * 360f, deltaTime * 10f);
                selectSprite.SetPosition(anchorPosition + new Vector2(Mathf.Cos(selectSpriteAngle * Mathf.Deg2Rad), Mathf.Sin(selectSpriteAngle * Mathf.Deg2Rad)) * 40f);

                for (int index = 0; index < length; index++)
                {
                    bool selected = index == selectedIndex;
                    float radian = (float)index / length * Mathf.PI * 2f;

                    visualizer[index].position = Vector2.Lerp(visualizer[index].position, anchorPosition + (new Vector2(Mathf.Cos(radian), Mathf.Sin(radian)) * factor * (selected ? 40f : 32f)), deltaTime * 10f);
                }

                base.Update(deltaTime);
            }
        }
    }
}