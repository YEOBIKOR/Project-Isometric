using System;

namespace Isometric.Items
{
    public class ItemTool : Item
    {
        public ItemTool(string name) : base(name)
        {

        }
        
        public override int maxStack
        {
            get
            { return 1; }
        }
    }
}