using System;

namespace Isometric.Items
{
    public class ItemTool : Item
    {
        public ItemTool() : base()
        {

        }
        
        public override int maxStack
        {
            get
            { return 1; }
        }
    }
}