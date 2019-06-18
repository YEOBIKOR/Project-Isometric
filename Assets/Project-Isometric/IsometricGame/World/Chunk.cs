using System;
using System.Collections.Generic;
using UnityEngine;

public enum ChunkState
{
    Unloaded,
    Loading,
    Loaded
}

public class Chunk : ISerializable <Chunk.Serialized>
{
    private World _world;
    public World world
    {
        get { return _world; }
    }
    public WorldCamera worldCamera
    {
        get { return _world.worldCamera; }
    }
    public IsometricGame game
    {
        get { return _world.game; }
    }

    private Vector2Int _coordination;
    public Vector2Int coordination
    {
        get
        { return _coordination; }
    }

    private ChunkRenderer _chunkRenderer;
    public ChunkRenderer chunkRenderer
    {
        get
        { return _chunkRenderer; }
    }

    private Chunk[] _nearbyChunks;

    private ChunkState _state;
    public ChunkState state
    {
        get { return _state; }
        set { _state = value; }
    }

    public const int Length = 16;
    public const int Height = 16;

    public Tile this[Vector3Int gridPosition]
    {
        get { return this[gridPosition.x, gridPosition.y, gridPosition.z]; }
    }

    public Tile this[int x, int y, int z]
    {
        get { return _tiles[x, y, z]; }
    }

    private Tile[,,] _tiles;
    private LinkedList<Entity> _entities;
    private LinkedList<ICollidable<Entity>> _collidables;

    public Chunk(World world, Vector2Int coordination)
    {
        _world = world;
        _coordination = coordination;

        _nearbyChunks = new Chunk[8];

        _chunkRenderer = new ChunkRenderer(this);

        _state = ChunkState.Unloaded;
        _tiles = new Tile[Length, Height, Length];

        _entities = new LinkedList<Entity>();
        _collidables = new LinkedList<ICollidable<Entity>>();

        for (int i = 0; i < Length; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                for (int k = 0; k < Length; k++)
                {
                    _tiles[i, j, k] = new Tile(this, (ushort)(i | (j << 4) | (k << 12)));
                }
            }
        }
    }

    public void LoadChunk()
    {
        _state = ChunkState.Loaded;
    }

    public void UnloadChunk()
    {
        _state = ChunkState.Unloaded;
    }

    public void Update(float deltaTime)
    {
        LinkedListNode<Entity> node = _entities.First;

        while (node != null)
        {
            Entity entity = node.Value;

            if (entity.spawned)
                entity.Update(deltaTime);

            if (!GetPositionInChunk(entity.worldPosition))
                entity.MoveToOtherChunk();

            if (!entity.spawned)
            {
                _world.OnDespawnEntity(entity);

                entity.OnDespawn();
            }

            LinkedListNode<Entity> nextNode = node.Next;
            if (entity.chunk != this)
            {
                _entities.Remove(node);

                if (entity.collider != null)
                    _collidables.Remove(entity.collider);
            }

            node = nextNode;
        }

        if (state == ChunkState.Loaded && !chunkRenderer.initialized)
        {
            foreach (Tile tile in _tiles)
            {
                if (Tile.GetFullTile(tile))
                    chunkRenderer.AddUpdateTile(tile);
            }

            worldCamera.AddRenderer(chunkRenderer);
        }
    }

    public void AddEntity(Entity entity)
    {
        if (!GetPositionInChunk(entity.worldPosition))
        {
            Debug.LogWarning("The Entity trying to add is out of range: " + entity);
            return;
        }

        _entities.AddLast(entity);
        entity.OnOtherChunk(this);

        if (entity.collider != null)
            _collidables.AddLast(entity.collider);
    }

    public void OnChunkActivate()
    {
        foreach (var entity in _entities)
        {
            entity.OnSpawn();

            world.OnSpawnEntity(entity);
        }
    }

    public void OnTileBlockSet(Tile tile)
    {
        chunkRenderer.AddUpdateTile(tile);

        if (state == ChunkState.Loaded)
        {
            int x = tile.coordination.x;
            int y = tile.coordination.y;
            int z = tile.coordination.z;

            Vector3Int[] positions = new Vector3Int[]
            {
                new Vector3Int(x + 1, y, z),
                new Vector3Int(x - 1, y, z),
                // new Vector3Int(x, y + 1, z),
                new Vector3Int(x, y - 1, z),
                new Vector3Int(x, y, z + 1),
                new Vector3Int(x, y, z - 1)
            };

            for (int index = 0; index < positions.Length; index++)
                world.GetChunkGraphicsAtPosition(positions[index]).AddUpdateTile(GetTileAtWorldPosition(positions[index]));
        }
    }

    public void SetNearbyChunk(int index, Chunk chunk)
    {
        _nearbyChunks[index] = chunk;
    }

    public Tile GetTileAtWorldPosition(int x, int y, int z)
    {
        return GetTileAtWorldPosition(new Vector3Int(x, y, z));
    }

    public Tile GetTileAtWorldPosition(Vector3Int position)
    {
        if (position.y < 0 || position.y >= Height)
            return null;

        if (GetPositionInChunk(position))
            return this[new Vector3Int(
                position.x & 0x0F,
                position.y,
                position.z & 0x0F)];

        for (int index = 0; index < _nearbyChunks.Length; index++)
        {
            if (_nearbyChunks[index] != null)
            {
                if (_nearbyChunks[index].GetPositionInChunk(position))
                {
                    return _nearbyChunks[index][new Vector3Int(
                        position.x & 0x0F,
                        position.y,
                        position.z & 0x0F)];
                }
            }
        }

        return world.GetTileAtPosition(position);
    }

    public float GetSurface(Vector2 position)
    {
        Vector2Int tilePosition = Vector2Int.FloorToInt(position);

        for (int y = Height - 1; !(y < 0); y--)
            if (Tile.GetFullTile(this[new Vector3Int(tilePosition.x & 0x0F, y, tilePosition.y & 0x0F)]))
                return y + 1f;

        return 0f;
    }

    public bool GetPositionInChunk(Vector3 position)
    {
        return ToChunkCoordinate(position) == coordination;
    }
    
    public void GetCollidedEntities(Vector3 position, float width, float height, Action<Entity> callback)
    {
        Action<Chunk> action = delegate (Chunk chunk)
        {
            LinkedListNode<ICollidable<Entity>> node = chunk._collidables.First;
            while (node != null)
            {
                ICollidable<Entity> collidable = node.Value;
                if (collidable != null)
                {
                    if (collidable.Collision(position, width, height))
                        callback(collidable.owner);
                }

                node = node.Next;
            }
        };

        action(this);
        for (int index = 0; index < _nearbyChunks.Length; index++)
        {
            if (_nearbyChunks[index] != null)
                action(_nearbyChunks[index]);
        }
    }

    public static bool IsEmptyTile(Tile tile)
    {
        if (tile == null)
            return true;
        return false;
    }

    public static Vector2Int ToChunkCoordinate(Vector3 worldPosition)
    {
        return ToChunkCoordinate(Vector3Int.FloorToInt(worldPosition));
    }

    public static Vector2Int ToChunkCoordinate(Vector3Int worldPosition)
    {
        return new Vector2Int(worldPosition.x >> 4, worldPosition.z >> 4);
    }

    public Serialized Serialize()
    {
        Serialized data = new Serialized();

        data.coordinationX = _coordination.x;
        data.coordinationY = _coordination.y;

        List<ushort> tiles = new List<ushort>();

        Block blockA = _tiles[0, 0, 0].block;

        int i = 0;
        for (int j = 0; j < Length * Height * Length; i++, j++)
        {
            Block blockB = _tiles[(j / Length) % Length, j / (Length * Length), j % Length].block;

            if (blockA != blockB)
            {
                tiles.Add((ushort)i);
                tiles.Add((ushort)Block.GetIDByBlock(blockA));

                i = 0;
                blockA = blockB;
            }
        }

        tiles.Add((ushort)i);
        tiles.Add((ushort)Block.GetIDByBlock(blockA));

        data.tiles = tiles.ToArray();

        return data;
    }

    public void Deserialize(Serialized data)
    {
        _coordination.x = data.coordinationX;
        _coordination.y = data.coordinationY;

        int i = 0;
        for (int j = 0; j < data.tiles.Length; j += 2)
        {
            Block block = Block.GetBlockByID(data.tiles[j + 1]);

            for (int k = 0; k < data.tiles[j]; i++, k++)
            {
                _tiles[(i / Length) % Length, i / (Length * Length), i % Length].SetBlock(block);
            }
        }

        state = ChunkState.Loaded;
    }

    [Serializable]
    public struct Serialized
    {
        public int coordinationX;
        public int coordinationY;
        public ushort[] tiles;
    }
}

public enum NearbyChunkDirection
{
    N,
    NE,
    E,
    SE,
    S,
    SW,
    W,
    NW
}