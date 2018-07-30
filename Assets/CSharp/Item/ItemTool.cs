using System;

namespace ISO.Items
{
    public class ItemTool : Item
    {
        public ItemTool() : base(1, 1)
        {

        }
        
        public override int maxStack
        {
            get
            { return 1; }
        }
    }
}