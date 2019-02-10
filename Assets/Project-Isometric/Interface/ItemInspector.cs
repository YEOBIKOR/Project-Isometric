using System;
using UnityEngine;
using Isometric.Items;

namespace Isometric.Interface
{
    public class ItemInspector : InterfaceObject
    {
        private RoundedRect _rect;

        private FLabel _itemName;
        private FLabel _itemInformation;

        public ItemInspector(Menu menu) : base(menu)
        {
            _rect = new RoundedRect(menu);
            _rect.size = new Vector2(100f, 20f);

            AddElement(_rect);

            _itemName = new FLabel("font", string.Empty);
            _itemName.scale = 0.5f;
            _itemInformation = new FLabel("font", string.Empty);
            _itemInformation.scale = 0.3f;

            container.AddChild(_itemName);
            container.AddChild(_itemInformation);

            activated = false;
        }

        public void InspectItem(ItemContainer itemContainer)
        {
            string name = string.Empty;

            if (itemContainer != null)
            {
                if (!itemContainer.blank)
                    name = itemContainer.itemStack.item.name;
            }

            if (name != string.Empty)
            {
                activated = true;

                _itemName.text = name;

                Vector2 contentSize = _itemName.textRect.size;
                // _rect.position = new Vector2(contentSize.x * 0.5f, contentSize.y * -0.5f);
                _rect.size = contentSize;
            }
            else
            {
                activated = false;
            }
        }
    }
}
