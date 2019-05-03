using System;
using UnityEngine;
using Isometric.Items;

namespace Isometric.Interface
{
    public class ItemContainerVisualizer : InterfaceObject
    {
        private ItemContainer _itemContainer;
        public ItemContainer itemContainer
        {
            get
            { return _itemContainer; }
        }

        private FSprite _itemSprite;
        private FLabel _itemAmount;

        public ItemContainerVisualizer(MenuFlow menu, ItemContainer itemContainer) : base(menu)
        {
            _itemContainer = itemContainer;

            _itemAmount = new FLabel("font", string.Empty);
            _itemAmount.SetPosition(new Vector2(4f, -4f));

            itemContainer.SignalItemChange += OnItemChanged;
        }

        public void OnItemChanged()
        {
            bool visible = false;

            if (!_itemContainer.blank)
            {
                if (_itemContainer.itemStack.item.element != null)
                    visible = true;
            }

            if (visible)
            {
                if (_itemSprite == null)
                {
                    _itemSprite = new FSprite(itemContainer.itemStack.item.element);

                    container.AddChild(_itemSprite);
                    container.AddChild(_itemAmount);
                }
                else
                    _itemSprite.element = itemContainer.itemStack.item.element;

                _itemAmount.text = itemContainer.itemStack.stackSize.ToString();
            }

            if (_itemSprite != null)
            {
                _itemSprite.isVisible = visible;

                bool isItemBlock = false;
                if (!itemContainer.blank)
                    isItemBlock = itemContainer.itemStack.item is ItemBlock;
                _itemSprite.scale = isItemBlock ? 0.75f : 1f;

                _itemAmount.isVisible = visible && itemContainer.itemStack.stackSize > 1;
            }
        }
    }
}