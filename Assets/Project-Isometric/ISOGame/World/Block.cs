using System.Collections.Generic;
using UnityEngine;

public abstract class Block
{
    public static Block[] _registry;
    public static FAtlasElement[] blockSprites { get; private set; }

    public Tile tile { get; private set; }
    public bool installed { get { return tile != null; } }

    public Block()
    {
        if (blockSprites == null)
            LoadBlockTexture();
    }

    public virtual void OnInstalled(Tile tile)
    {
        this.tile = tile;
    }

    public static void LoadBlockTexture()
    {
        List<FAtlasElement> tileSpriteList = new List<FAtlasElement>();
        int index = 0;

        while (true)
        {
            try
            { tileSpriteList.Add(Futile.atlasManager.GetElementWithName(string.Concat("blocks/b", ++index))); }
            catch
            { break; }
        }

        blockSprites = tileSpriteList.ToArray();
    }

    public virtual bool fullBlock
    {
        get { return false; }
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
    private int id;

    public BlockSolid(int id) : base()
    {
        this.id = id;
    }

    public override bool fullBlock
    {
        get
        { return true; }
    }

    public override FAtlasElement sprite
    {
        get
        { return blockSprites[id]; }
    }
}