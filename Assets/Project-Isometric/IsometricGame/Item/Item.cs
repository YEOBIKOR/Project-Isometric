using UnityEngine;
using Isometric.Interface;

namespace Isometric.Items
{
    public abstract class Item
    {
        private static Registry<Item> _registry;

        public static void RegisterItems()
        {
            _registry = new Registry<Item>();

            _registry.Add("pickaxe", new ItemPickaxe("Pickaxe", "pickaxe"));
            _registry.Add("throwable_rock", new ItemThrowableRock("Rock", "throwablerock"));
            _registry.Add("block_dirt", new ItemBlock("Dirt Block", "dirt"));
            _registry.Add("block_grass", new ItemBlock("Grass Block", "grass"));
            _registry.Add("block_stone", new ItemBlock("Stone Block", "stone"));
            _registry.Add("block_mossy_stone", new ItemBlock("Mossy Stone Block", "mossy_stone"));
            _registry.Add("block_sand", new ItemBlock("Sand Block", "sand"));
            _registry.Add("block_sandstone", new ItemBlock("Sandstone Block", "sandstone"));
            _registry.Add("block_wood", new ItemBlock("Wood Block", "wood"));
            _registry.Add("gunak47", new ItemGun("AK47", "gunak47", 0.08f, true));
            _registry.Add("guncannon", new ItemGun("Cannon", "guncannon", 1f, false));
            _registry.Add("gungranade", new ItemGranadeLauncher("Granade Launcher", "gungranade"));
            _registry.Add("gunpistol", new ItemGun("Pistol", "gunpistol", 0.5f, false));
            _registry.Add("gunshot", new ItemShotgun("Shotgun", "gunshot", 0.8f, false));
        }

        public static Item GetItemByID(int id)
        {
            if (_registry == null)
                RegisterItems();

            return _registry[id];
        }

        public static Item GetItemByKey(string key)
        {
            if (_registry == null)
                RegisterItems();

            return _registry[key];
        }

        public static Item[] GetItemAll()
        {
            if (_registry == null)
                RegisterItems();

            return _registry.GetAll();
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

        public virtual void OnUseItem(World world, Player player, ItemContainer itemContainer, Vector3Int tilePosition)
        {

        }

        public virtual void OnUseItem(World world, Player player, ItemContainer itemContainer, Vector3 targetPosition)
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

        public virtual CursorType cursorType
        {
            get
            { return CursorType.None; }
        }
    }
}