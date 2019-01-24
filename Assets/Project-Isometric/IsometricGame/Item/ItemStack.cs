using System;

namespace Isometric.Items
{
    public class ItemStack
    {
        private Item _item;
        public Item item
        {
            get
            { return _item; }
        }

        private int _stackSize;
        public int stackSize
        {
            get
            { return _stackSize; }
        }

        public ItemStack(Item item, int stackSize)
        {
            _item = item;
            _stackSize = stackSize;
        }

        public void OnUseItem(Player player, RayTrace rayTrace)
        {
            _item.OnUseItem(player, rayTrace);
        }

        public int StackUp(int amount)
        {
            _stackSize += amount;

            if (_stackSize > _item.maxStack)
            {
                int delta = _stackSize - _item.maxStack;

                _stackSize = item.maxStack;
                return delta;
            }

            return 0;
        }

        public static void Apply(ref ItemStack destination, ref ItemStack source)
        {
            bool flag = false;

            if (destination != null && source != null)
                flag = destination.item == source.item;

            if (flag)
            {
                int stackSize = destination.StackUp(source.stackSize);

                if (stackSize > 0)
                    source._stackSize = stackSize;
                else
                    source = null;
            }
            else
            {
                ItemStack temp = destination;
                destination = source;
                source = temp;
            }
        }
    }
}