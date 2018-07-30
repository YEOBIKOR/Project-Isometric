using System;

namespace ISO.Items
{
    public class ItemContainer
    {
        private Item _item;

        public event Action SignalItemChange;

        public Item item
        {
            get
            { return _item; }
        }

        public bool blank
        {
            get
            { return _item == null; }
        }

        public Item SetItem(Item item)
        {
            Item returnItem = _item;
            _item = item;

            if (_item != null && returnItem != null)
            {
                if (_item.itemID == returnItem.itemID)
                {
                    _item.stackSize += returnItem.stackSize;

                    if (_item.stackSize > _item.maxStack)
                    {
                        returnItem.stackSize = _item.stackSize - item.maxStack;
                        _item.stackSize = _item.maxStack;
                    }
                    else
                        returnItem = null;
                }
            }

            if (SignalItemChange != null)
                SignalItemChange();

            return returnItem;
        }
    }
}
