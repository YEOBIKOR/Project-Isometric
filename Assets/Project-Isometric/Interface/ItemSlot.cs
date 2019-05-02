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

        private ItemContainer _itemContainer;
        public ItemContainer itemContainer
        {
            get
            { return _itemContainer; }
        }

        private ItemContainerVisualizer visualizer;

        public ItemSlot(InventoryMenu menu, ItemContainer itemContainer) : base(menu, string.Empty, null, false)
        {
            this._itemContainer = itemContainer;

            position = position;
            size = Vector2.one * 24f;

            visualizer = new ItemContainerVisualizer(menu, itemContainer);
            AddElement(visualizer);
        }

        public override void OnHover()
        {
            base.OnHover();

            inventoryMenu.InspectItem(this);
        }

        public override void OnHoverOut()
        {
            base.OnHoverOut();
        }

        public override void OnPressUp()
        {
            base.OnPressUp();

            ItemStack returnItemStack = _itemContainer.Apply(inventoryMenu.cursorItemContainer.itemStack);
            inventoryMenu.cursorItemContainer.Apply(returnItemStack);
        }
    }
}