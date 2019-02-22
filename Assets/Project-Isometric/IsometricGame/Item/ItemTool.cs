using System;

namespace Isometric.Items
{
    public class ItemTool : Item
    {
        public ItemTool(string name, string textureName) : base(name, textureName)
        {

        }
        
        public override int maxStack
        {
            get
            { return 1; }
        }
    }
}