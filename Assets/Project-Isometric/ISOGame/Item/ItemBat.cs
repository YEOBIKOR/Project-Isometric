using System;

namespace Isometric.Items
{
    public class ItemBat : ItemTool
    {
        public ItemBat() : base()
        {

        }

        public override FAtlasElement element
        {
            get
            { return Futile.atlasManager.GetElementWithName("items/itemaluminumbat"); }
        }
    }
}