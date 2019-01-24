using System;

namespace Isometric.Items
{
    public class ItemContainer
    {
        private ItemStack _itemStack;
        public ItemStack itemStack
        {
            get
            { return _itemStack; }
        }

        public bool blank
        {
            get
            { return _itemStack == null; }
        }

        public event Action SignalItemChange;

        public ItemStack SetItem(ItemStack itemStack)
        {
            ItemStack.Apply(ref _itemStack, ref itemStack);

            if (SignalItemChange != null)
                SignalItemChange();

            return itemStack;
        }
    }
}
