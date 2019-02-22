using System;
using UnityEngine;

namespace Isometric.Items
{
    public class ItemCoin : Item
    {
        public ItemCoin(string name, string textureName) : base(name, textureName)
        {

        }

        public override int maxStack
        {
            get
            { return 999; }
        }
    }
}