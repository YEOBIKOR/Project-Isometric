using System.Collections.Generic;
using UnityEngine;
using Custom;

public class Tile
{
    public Chunk chunk { get; private set; }
    public World world { get { return chunk.world; } }

    public Block block { get; private set; }

    private ushort index;

    public Vector3Int worldPosition
    {
        get
        {
            return new Vector3Int(
            chunk.coordinate.x * Chunk.length + (index & 0x000F),
            index >> 4 & 0x00FF,
            chunk.coordinate.y * Chunk.length + (index >> 12 & 0x000F));
        }
    }

    public Tile(Chunk chunk, ushort index)
    {
        this.chunk = chunk;
        this.index = index;
    }

    public void SetBlock(Block block)
    {
        this.block = block;
        chunk.OnTileBlockSet(this);
    }

    public Tile GetTileAtWorldPosition(Vector3Int position)
    {
        return chunk.GetTileAtWorldPosition(position);
    }

    public static bool GetFullTile(Tile tile)
    {
        if (tile == null)
            return true;
        else if (tile.block != null)
            return tile.block.fullBlock;

        return false;
    }

    public static bool GetCrossable(Tile tile)
    {
        if (tile == null)
            return true;
        else if (tile.block != null)
            return !tile.block.fullBlock;

        return true;
    }
}