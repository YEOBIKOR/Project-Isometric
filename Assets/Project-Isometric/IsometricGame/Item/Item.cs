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

            registry.Add("pickaxe", new ItemPickaxe());
            registry.Add("throwable_rock", new ItemThrowableRock());
            registry.Add("block_dirt", new ItemBlock("dirt"));
            registry.Add("block_grass", new ItemBlock("grass"));
            registry.Add("block_stone", new ItemBlock("stone"));
            registry.Add("block_mossy_stone", new ItemBlock("mossy_stone"));
            registry.Add("block_sand", new ItemBlock("sand"));
            registry.Add("block_sandstone", new ItemBlock("sandstone"));
            registry.Add("block_wood", new ItemBlock("wood"));
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

        public Item()
        {

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