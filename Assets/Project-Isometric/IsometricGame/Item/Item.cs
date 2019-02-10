using System;
using System.Collections.Generic;

namespace Isometric.Items
{
    public class Item
    {
        private static Registry<Item> registry;

        public static void RegisterItems()
        {
            registry = new Registry<Item>();

            registry.Add("pickaxe", new ItemPickaxe("Pickaxe"));
            registry.Add("throwable_rock", new ItemThrowableRock("Rock"));
            registry.Add("block_dirt", new ItemBlock("Dirt Block", "dirt"));
            registry.Add("block_grass", new ItemBlock("Grass Block", "grass"));
            registry.Add("block_stone", new ItemBlock("Stone Block", "stone"));
            registry.Add("block_mossy_stone", new ItemBlock("Mossy Stone Block", "mossy_stone"));
            registry.Add("block_sand", new ItemBlock("Sand Block", "sand"));
            registry.Add("block_sandstone", new ItemBlock("Sandstone Block", "sandstone"));
            registry.Add("block_wood", new ItemBlock("Wood Block", "wood"));
        }

        public static Item GetItemByID(int id)
        {
            if (registry == null)
                RegisterItems();

            return registry[id];
        }

        public static Item GetItemByKey(string key)
        {
            if (registry == null)
                RegisterItems();

            return registry[key];
        }

        public static Item[] GetItemAll()
        {
            if (registry == null)
                RegisterItems();

            return registry.GetAll();
        }

        private string _name;
        public string name
        {
            get
            { return _name; }
        }

        public Item(string name)
        {
            _name = name;
        }

        public virtual void OnUseItem(Player player, RayTrace rayTrace)
        {

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