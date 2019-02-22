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

            registry.Add("pickaxe", new ItemPickaxe("Pickaxe", "pickaxe"));
            registry.Add("throwable_rock", new ItemThrowableRock("Rock", "throwablerock"));
            registry.Add("block_dirt", new ItemBlock("Dirt Block", "dirt"));
            registry.Add("block_grass", new ItemBlock("Grass Block", "grass"));
            registry.Add("block_stone", new ItemBlock("Stone Block", "stone"));
            registry.Add("block_mossy_stone", new ItemBlock("Mossy Stone Block", "mossy_stone"));
            registry.Add("block_sand", new ItemBlock("Sand Block", "sand"));
            registry.Add("block_sandstone", new ItemBlock("Sandstone Block", "sandstone"));
            registry.Add("block_wood", new ItemBlock("Wood Block", "wood"));
            registry.Add("gunak47", new ItemGun("AK47", "gunak47"));
            registry.Add("guncannon", new ItemGun("Cannon", "guncannon"));
            registry.Add("gungranade", new ItemGun("Granade Launcher", "gungranade"));
            registry.Add("gunpistol", new ItemGun("Pistol", "gunpistol"));
            registry.Add("gunshot", new ItemGun("Shotgun", "gunshot"));
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

        private FAtlasElement _texture;


        public Item(string name, string textureName) : this(name, Futile.atlasManager.GetElementWithName(string.Concat("items/", textureName)))
        {

        }

        public Item(string name, FAtlasElement texture) : this(name)
        {
            _texture = texture;
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
            { return _texture; }
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

        public virtual HoldType holdType
        {
            get
            { return HoldType.None; }
        }
    }
}