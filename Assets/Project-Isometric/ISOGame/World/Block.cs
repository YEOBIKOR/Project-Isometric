using System.Collections.Generic;
using UnityEngine;

public abstract class Block
{
    private static Dictionary<string, Block> _registry;
    private static Block _blockAir;

    public Block()
    {

    }

    public static void RegisterBlocks()
    {
        _registry = new Dictionary<string, Block>();

        _blockAir = new BlockAir();
        _registry.Add("air", _blockAir);
        _registry.Add("dirt", new BlockSolid("b1"));
        _registry.Add("grass", new BlockSolid("b2"));
        _registry.Add("stone", new BlockSolid("b3"));
        _registry.Add("mossy_stone", new BlockSolid("b4"));
        _registry.Add("sand", new BlockSolid("b5"));
        _registry.Add("sandstone", new BlockSolid("b6"));
        _registry.Add("wood", new BlockSolid("b7"));
        _registry.Add("bedrock", new BlockSolid("b26"));
    }

    public static Block GetBlockByKey(string key)
    {
        if (_registry == null)
            RegisterBlocks();

        return _registry[key];
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