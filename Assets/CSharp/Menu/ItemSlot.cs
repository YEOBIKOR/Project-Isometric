using System;
using UnityEngine;
using ISO.Items;

namespace ISO.UI
{
    public class ItemSlot : GeneralButton
    {
        public InventoryMenu inventoryMenu
        {
            get
            { return menu as InventoryMenu; }
        }

        private ItemContainer itemContainer;
        private ItemContainerVisualizer visualizer;

        public ItemSlot(InventoryMenu menu, ItemContainer itemContainer) : base(menu, string.Empty, null)
        {
            this.itemContainer = itemContainer;

            position = position;
            size = Vector2.one * 24f;

            visualizer = new ItemContainerVisualizer(menu, itemContainer);
            AddElement(visualizer);
        }

        public override void Update(float deltaTime)
        {
            visualizer.position = position;

            base.Update(deltaTime);
        }

        public override void OnPressUp()
        {
            base.OnPressUp();

            Item returnItem = itemContainer.SetItem(inventoryMenu.cursorItemContainer.item);
            inventoryMenu.cursorItemContainer.SetItem(returnItem);
        }
    }
}