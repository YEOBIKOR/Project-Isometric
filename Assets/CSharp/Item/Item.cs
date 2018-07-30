using System;
using UnityEngine;

namespace ISO.Items
{
    public class Item : ICloneable
    {
        private int _itemID;
        public int itemID
        {
            get
            { return _itemID; }
        }

        private int _stackSize;
        public int stackSize
        {
            get
            { return _stackSize; }
            set
            { _stackSize = value; }
        }

        public Item(int itemID, int stackSize)
        {
            _itemID = itemID;
            _stackSize = stackSize;
        }

        public Item(Item item)
        {
            _itemID = item._itemID;
            _stackSize = item._stackSize;
        }

        public virtual void OnUseItem(Player player, RayTrace rayTrace)
        {

        }

        public virtual object Clone()
        {
            return new Item(this);
        }

        public virtual int maxStack
        {
            get
            { return 64; }
        }

        public virtual FAtlasElement element
        {
            get
            { return null; }
        }

        public virtual float useCoolTime
        {
            get
            { return 0f; }
        }

        public virtual bool repeatableUse
        {
            get
            { return false; }
        }
    }
}