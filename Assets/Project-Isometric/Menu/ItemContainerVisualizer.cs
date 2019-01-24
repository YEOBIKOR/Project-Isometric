using System;
using UnityEngine;
using Isometric.Items;

namespace Isometric.UI
{
    public class ItemContainerVisualizer : UIObject
    {
        private ItemContainer _itemContainer;
        public ItemContainer itemContainer
        {
            get
            { return _itemContainer; }
        }

        private FSprite itemSprite;
        private FLabel itemAmount;

        public ItemContainerVisualizer(Menu menu, ItemContainer itemContainer) : base(menu)
        {
            this._itemContainer = itemContainer;

            itemAmount = new FLabel("font", string.Empty);
            itemAmount.scale = 0.5f;
            container.AddChild(itemAmount);

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
                if (itemSprite == null)
                {
                    itemSprite = new FSprite(itemContainer.itemStack.item.element);

                    container.AddChild(itemSprite);
                }
                else
                    itemSprite.element = itemContainer.itemStack.item.element;

                itemAmount.text = itemContainer.itemStack.stackSize.ToString();
            }

            if (itemSprite != null)
            {
                itemSprite.isVisible = visible;

                bool isItemBlock = false;
                if (!itemContainer.blank)
                    isItemBlock = itemContainer.itemStack.item is ItemBlock;
                itemSprite.scale = isItemBlock ? 0.75f : 1f;

                itemAmount.isVisible = visible && itemContainer.itemStack.stackSize > 1;
            }
        }

        public override void Update(float deltaTime)
        {
            if (itemSprite != null)
                itemSprite.SetPosition(position);
            itemAmount.SetPosition(position + new Vector2(8f, -8f));

            base.Update(deltaTime);
        }
    }
}