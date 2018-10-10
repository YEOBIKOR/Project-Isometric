using System;
using System.Collections.Generic;
using UnityEngine;
using Isometric.Items;

namespace Isometric.UI
{
    public class InventoryMenu : PopupMenu
    {
        private PlayerInterface playerInterface;

        public Player player
        {
            get
            { return playerInterface.player; }
        }
        
        private ItemSlot[] itemSlots;

        private ItemContainer _cursorItemContainer;
        public ItemContainer cursorItemContainer
        {
            get
            { return _cursorItemContainer; }
        }

        private InventoryCursor inventoryCursor;

        private GeneralButton exitButton;

        public InventoryMenu(PlayerInterface playerInterface) : base(null, true, 0.5f, 0.5f)
        {
            this.playerInterface = playerInterface;

            AddElement(new FadePanel(this));

            itemSlots = new ItemSlot[player.inventorySize];

            for (int index = 0; index < 8f; index++)
            {
                if (player.inventory[index] != null)
                {
                    itemSlots[index] = new ItemSlot(this, player.inventory[index]);
                    itemSlots[index].position =
                        new Vector2(Mathf.Cos(index / 4f * Mathf.PI), Mathf.Sin(index / 4f * Mathf.PI)) * 40f;

                    AddElement(itemSlots[index]);
                }
            }

            for (int index = 8; index < itemSlots.Length; index++)
            {
                if (player.inventory[index] != null)
                {
                    itemSlots[index] = new ItemSlot(this, player.inventory[index]);
                    itemSlots[index].position =
                        leftUp + new Vector2(20f, -20f) + new Vector2((index / 8) - 1, -(index % 8)) * 32f;
                    
                    AddElement(itemSlots[index]);
                }
            }

            _cursorItemContainer = new ItemContainer();
            inventoryCursor = new InventoryCursor(this);
            AddElement(inventoryCursor);

            exitButton = new GeneralButton(this, "X", RequestTerminate);
            exitButton.position = rightUp + new Vector2(-24f, -24f);
            exitButton.size = new Vector2(16f, 16f);
            AddElement(exitButton);
        }

        public override void OnTerminate()
        {
            player.DropItem(cursorItemContainer);

            base.OnTerminate();
        }

        public override void RawUpdate(float deltaTime)
        {
            if (Input.GetKeyDown(KeyCode.I) && time > 0f)
                RequestTerminate();

            base.RawUpdate(deltaTime);
        }

        public override void Update(float deltaTime)
        {
            playerInterface.player.game.timeScale = Mathf.Lerp(1f, 0.1f, factor);

            base.Update(deltaTime);
        }
    }

    public class InventoryCursor : UIObject
    {
        private ItemContainerVisualizer visualizer;

        public InventoryCursor(InventoryMenu menu) : base(menu)
        {
            visualizer = new ItemContainerVisualizer(menu, menu.cursorItemContainer);
            AddElement(visualizer);
        }

        public override void Update(float deltaTime)
        {
            visualizer.position = Menu.mousePosition;

            base.Update(deltaTime);
        }
    }
}
