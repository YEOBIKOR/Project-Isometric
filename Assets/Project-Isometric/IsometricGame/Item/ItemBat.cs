using System;

namespace Isometric.Items
{
    public class ItemBat : ItemTool
    {
        public ItemBat(string name) : base(name)
        {

        }

        public override FAtlasElement element
        {
            get
            { return Futile.atlasManager.GetElementWithName("items/itemaluminumbat"); }
        }
    }
}