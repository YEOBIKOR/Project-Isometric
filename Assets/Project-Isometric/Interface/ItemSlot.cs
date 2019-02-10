using System;
using UnityEngine;
using Isometric.Items;

namespace Isometric.Interface
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

        public override void OnMouseHover()
        {
            base.OnMouseHover();

            inventoryMenu.InspectItem(itemContainer);
        }

        public override void OnMouseLeave()
        {
            base.OnMouseLeave();

            inventoryMenu.InspectItem(null);
        }

        public override void OnPressUp()
        {
            base.OnPressUp();

            ItemStack returnItemStack = itemContainer.SetItem(inventoryMenu.cursorItemContainer.itemStack);
            inventoryMenu.cursorItemContainer.SetItem(returnItemStack);
        }
    }
}