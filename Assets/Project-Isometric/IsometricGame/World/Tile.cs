using System;
using UnityEngine;

public class Tile : IPositionable, ISerializable<Tile.Serialized>
{
    private Chunk _chunk;
    public Chunk chunk
    {
        get { return _chunk; }
    }
    public World world
    {
        get { return _chunk.world; }
    }

    private Block _block;
    public Block block
    {
        get { return _block; }
    }

    private ushort _index;

    public Vector3Int coordination
    {
        get
        {
            return new Vector3Int(
            chunk.coordination.x * Chunk.Length + (_index & 0x000F),
            _index >> 4 & 0x00FF,
            chunk.coordination.y * Chunk.Length + (_index >> 12 & 0x000F));
        }
    }

    public Vector3 worldPosition
    {
        get { return coordination; }
    }

    public Tile(Chunk chunk, ushort index)
    {
        _chunk = chunk;
        _index = index;

        _block = Block.BlockAir;
    }

    public void SetBlock(Block block)
    {
        _block = block;

        if (chunk.state == ChunkState.Loaded)
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

    public Serialized Serialize()
    {
        Serialized data = new Serialized();

        data.blockID = Block.GetIDByBlock(block);

        return data;
    }

    public void Deserialize(Serialized data)
    {
        Block block = Block.GetBlockByID(data.blockID);

        SetBlock(block);
    }

    [Serializable]
    public struct Serialized
    {
        public int blockID;
    }
}