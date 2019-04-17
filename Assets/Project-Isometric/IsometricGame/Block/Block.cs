using System.Collections.Generic;
using UnityEngine;

public abstract class Block
{
    private static Registry<Block> registry;
    private static Block _blockAir;
    
    public static void RegisterBlocks()
    {
        registry = new Registry<Block>();

        _blockAir = new BlockAir();
        registry.Add("air", _blockAir);
        registry.Add("dirt", new BlockSolid("b1"));
        registry.Add("grass", new BlockSolid("b2"));
        registry.Add("stone", new BlockSolid("b3"));
        registry.Add("mossy_stone", new BlockSolid("b4"));
        registry.Add("sand", new BlockSolid("b5"));
        registry.Add("sandstone", new BlockSolid("b6"));
        registry.Add("wood", new BlockSolid("b7"));
        registry.Add("bedrock", new BlockSolid("b26"));
    }

    public static Block GetBlockByKey(string key)
    {
        if (registry == null)
            RegisterBlocks();

        return registry[key];
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
    private FAtlasElement _sprite;

    public BlockSolid(string elementName) : base()
    {
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
}