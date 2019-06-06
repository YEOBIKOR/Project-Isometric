using System.Collections.Generic;
using UnityEngine;
using Isometric.Items;

public abstract class Block
{
    private static Registry<Block> _registry;
    private static Block _blockAir;
    
    public static void RegisterBlocks()
    {
        _registry = new Registry<Block>();

        _blockAir = new BlockAir();
        _registry.Add("air", _blockAir);
        _registry.Add("dirt", new BlockSolid("block_dirt", "b1"));
        _registry.Add("grass", new BlockSolid("block_dirt", "b2"));
        _registry.Add("stone", new BlockSolid("block_stone", "b3"));
        _registry.Add("mossy_stone", new BlockSolid("block_stone", "b4"));
        _registry.Add("sand", new BlockSolid("block_sand", "b5"));
        _registry.Add("sandstone", new BlockSolid("block_sandstone", "b6"));
        _registry.Add("wood", new BlockSolid("block_wood", "b7"));
        _registry.Add("bedrock", new BlockSolid(null, "b26"));
    }

    public static Block GetBlockByID(int id)
    {
        if (_registry == null)
            RegisterBlocks();

        return _registry[id];
    }

    public static Block GetBlockByKey(string key)
    {
        if (_registry == null)
            RegisterBlocks();

        return _registry[key];
    }

    public static int GetIDByBlock(Block block)
    {
        if (_registry == null)
            RegisterBlocks();

        return _registry.GetID(block);
    }

    public Block()
    {

    }

    public static Block BlockAir
    {
        get
        { return _blockAir; }
    }

    public virtual bool fullBlock
    {
        get
        { return false; }
    }

    public virtual FAtlasElement sprite
    {
        get
        { return null; }
    }

    public virtual ItemStack OnDropItem()
    {
        return null;
    }
}

public class BlockAir : Block
{
    public override bool fullBlock
    {
        get
        { return false; }
    }

    public override FAtlasElement sprite
    {
        get
        { return null; }
    }
}

public class BlockSolid : Block
{
    private string _dropItem;

    private FAtlasElement _sprite;

    public BlockSolid(string dropItem, string elementName) : base()
    {
        _dropItem = dropItem;

        _sprite = Futile.atlasManager.GetElementWithName(string.Concat("blocks/", elementName));
    }

    public override bool fullBlock
    {
        get
        { return true; }
    }

    public override FAtlasElement sprite
    {
        get
        { return _sprite; }
    }

    public override ItemStack OnDropItem()
    {
        return new ItemStack(Item.GetItemByKey(_dropItem), 1);
    }
}