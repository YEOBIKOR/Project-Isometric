using System;
using System.Collections.Generic;
using UnityEngine;
using Isometric.Items;

namespace Isometric.Interface
{
    public class InventoryMenu : PopupMenuFlow
    {
        private PlayerInterface _playerInterface;

        public Player player
        {
            get
            { return _playerInterface.player; }
        }
        
        private ItemSlot[] _itemSlots;

        private ItemContainer _cursorItemContainer;
        public ItemContainer cursorItemContainer
        {
            get
            { return _cursorItemContainer; }
        }

        private InventoryCursor _inventoryCursor;

        private GeneralButton _exitButton;

        public InventoryMenu(PlayerInterface playerInterface) : base(null, true, 0.2f, 0.1f)
        {
            _playerInterface = playerInterface;

            AddElement(new FadePanel(this));

            _itemSlots = new ItemSlot[player.inventorySize];

            for (int index = 0; index < 8f; index++)
            {
                if (player.inventory[index] != null)
                {
                    _itemSlots[index] = new ItemSlot(this, player.inventory[index]);
                    _itemSlots[index].position =
                        new Vector2(Mathf.Cos(index / 4f * Mathf.PI), Mathf.Sin(index / 4f * Mathf.PI)) * 40f;

                    AddElement(_itemSlots[index]);
                }
            }

            for (int index = 8; index < _itemSlots.Length; index++)
            {
                if (player.inventory[index] != null)
                {
                    _itemSlots[index] = new ItemSlot(this, player.inventory[index]);
                    _itemSlots[index].position =
                        leftUp + new Vector2(20f, -20f) + new Vector2((index / 8) - 1, -(index % 8)) * 32f;
                    
                    AddElement(_itemSlots[index]);
                }
            }

            _cursorItemContainer = new ItemContainer();
            _inventoryCursor = new InventoryCursor(this);
            AddElement(_inventoryCursor);

            _exitButton = new GeneralButton(this, "X", RequestTerminate, false);
            _exitButton.position = rightUp + new Vector2(-24f, -24f);
            _exitButton.size = new Vector2(16f, 16f);
            AddElement(_exitButton);
        }

        public override void OnTerminate()
        {
            player.DropItem(cursorItemContainer);

            base.OnTerminate();
        }

        public override void Update(float deltaTime)
        {
            _playerInterface.player.game.timeScale = Mathf.Lerp(1f, 0.02f, factor);

            InspectItem(null);

            base.Update(deltaTime);
        }

        public void InspectItem(ItemSlot itemSlot)
        {
            _inventoryCursor.InspectItem(itemSlot);
        }
    }

    public class InventoryCursor : InterfaceObject
    {
        private ItemContainerVisualizer _visualizer;

        private ItemInspector _inspector;

        public InventoryCursor(InventoryMenu menu) : base(menu)
        {
            _visualizer = new ItemContainerVisualizer(menu, menu.cursorItemContainer);
            _inspector = new ItemInspector(menu);

            AddElement(_visualizer);
            AddElement(_inspector);
        }

        public override void Update(float deltaTime)
        {
            _visualizer.position = MenuFlow.mousePosition;

            base.Update(deltaTime);
        }

        public void InspectItem(ItemSlot itemSlot)
        {
            if (itemSlot != null)
            {
                _inspector.position = itemSlot.position + Vector2.down * 28f;

                _inspector.InspectItem(itemSlot.itemContainer);
            }

            else
            {
                _inspector.InspectItem(null);
            }
        }
    }
}
