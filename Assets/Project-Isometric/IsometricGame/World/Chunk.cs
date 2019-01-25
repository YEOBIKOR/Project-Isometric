using UnityEngine;
using System;
using System.Collections.Generic;

public enum ChunkState
{
    Unloaded,
    Loading,
    Loaded
}

public class Chunk
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
        get { return tiles[x, y, z]; }
    }

    private Tile[,,] tiles;
    private LinkedList<Entity> entities;
    
    public Vector2Int coordination { get; private set; }
    public ChunkRenderer chunkGraphics { get; private set; }

    public Chunk(World world, Vector2Int coordination)
    {
        _world = world;
        this.coordination = coordination;

        _nearbyChunks = new Chunk[8];

        _state = ChunkState.Unloaded;

        tiles = new Tile[Length, Height, Length];
        entities = new LinkedList<Entity>();

        chunkGraphics = new ChunkRenderer(this);

        for (int i = 0; i < Length; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                for (int k = 0; k < Length; k++)
                {
                    tiles[i, j, k] = new Tile(this, (ushort)(i | (j << 4) | (k << 12)));
                }
            }
        }
    }

    public void LoadChunk()
    {
        _state = ChunkState.Loaded;

        //worldCamera.AddRenderer(chunkGraphics);
    }

    public void UnloadChunk()
    {
        _state = ChunkState.Unloaded;
    }

    public void Update(float deltaTime)
    {
        LinkedListNode<Entity> node = entities.First;
        while (node != null)
        {
            Entity entity = node.Value;

            entity.Update(deltaTime);

            if (!entity.spawned)
                entity.OnDespawn();

            LinkedListNode<Entity> nextNode = node.Next;
            if (entity.chunk != this)
                entities.Remove(node);

            node = nextNode;
        }

        if (state == ChunkState.Loaded && !chunkGraphics.initialized)
        {
            foreach (Tile tile in tiles)
            {
                if (Tile.GetFullTile(tile))
                    chunkGraphics.AddUpdateTile(tile);
            }

            worldCamera.AddRenderer(chunkGraphics);
        }
    }

    public void AddEntity(Entity entity)
    {
        entities.AddLast(entity);
    }

    public void OnTileBlockSet(Tile tile)
    {
        chunkGraphics.AddUpdateTile(tile);

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
        else
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
        return Chunk.ToChunkCoordinate(position) == coordination;
    }

    public void GetCollidedEntites(PhysicalEntity owner, Action<PhysicalEntity> callback)
    {
        LinkedListNode<Entity> node = entities.First;
        while (node != null)
        {
            PhysicalEntity entity = node.Value as PhysicalEntity;
            if (entity != null)
            {
                if (entity != owner && owner.GetCollisionWithOther(entity))
                    callback(entity);
            }

            node = node.Next;
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

    public class ChunkRenderer : IRenderer
    {
        private struct TileSpritePair
        {
            public Tile tile;
            public FSprite sprite;
        }

        private Chunk chunk;

        private List<Tile> drawTiles;
        private Queue<Tile> tilesQueue;

        private bool showing;

        private bool _initialized;
        public bool initialized
        {
            get
            { return _initialized; }
        }

        public ChunkRenderer(Chunk chunk)
        {
            this.chunk = chunk;
            
            drawTiles = new List<Tile>();
            tilesQueue = new Queue<Tile>();

            showing = false;
            _initialized = false;
        }

        public void OnInitializeSprite(SpriteLeaser spriteLeaser, WorldCamera camera)
        {
            _initialized = true;

            UpdateTileRender(spriteLeaser, camera);
        }

        public void RenderUpdate(SpriteLeaser spriteLeaser, WorldCamera camera)
        {
            bool inRange = ((chunk.coordination + Vector2.one * 0.5f) * Chunk.Length - new Vector2(chunk.world.player.worldPosition.x, chunk.world.player.worldPosition.z)).sqrMagnitude < 1024f;

            if (showing && !inRange)
            {
                spriteLeaser.RemoveFromContainer();
                showing = false;
            }
            else if (!showing && inRange)
                showing = true;

            if (showing)
            {
                UpdateTileRender(spriteLeaser, camera);

                for (int index = 0; index < drawTiles.Count; index++)
                {
                    Tile tile = drawTiles[index];
                    FSprite sprite = spriteLeaser.sprites[index];

                    SetSpriteByTile(sprite, tile, camera, true);

                    if (sprite.container != null && !spriteLeaser.InScreenRect(sprite))
                        sprite.RemoveFromContainer();
                    else if (sprite.container == null && spriteLeaser.InScreenRect(sprite))
                        camera.worldContainer.AddChild(sprite);
                }
            }
        }

        public void SetSpriteByTile(FSprite target, Tile tile, WorldCamera camera, bool optimizeColor = false)
        {
            Vector3 spritePosition = tile.coordination + Vector3.one * 0.5f;
            target.SetPosition(camera.GetScreenPosition(spritePosition));
            target.sortZ = camera.GetSortZ(spritePosition);
            
            if (optimizeColor)
                target.color = new Color(tile.coordination.x, tile.coordination.y, tile.coordination.z);
        }

        public bool GetShownByCamera(SpriteLeaser spriteLeaser, WorldCamera camera)
        {
            return false;
        }

        public void UpdateTileRender(SpriteLeaser spriteLeaser, WorldCamera camera)
        {
            while (tilesQueue.Count > 0)
            {
                Tile tile = tilesQueue.Dequeue();

                if (tile != null)
                {
                    bool showing = false;

                    if (tile.block.sprite != null)
                        if (GetTileShowing(tile))
                            showing = true;

                    int index = drawTiles.IndexOf(tile);

                    if (showing)
                    {
                        FSprite sprite;

                        if (index < 0)
                        {
                            sprite = new FSprite(tile.block.sprite);
                            sprite.shader = IsometricMain.GetShader("WorldObject");

                            drawTiles.Add(tile);
                            spriteLeaser.sprites.Add(sprite);
                        }
                        else
                        {
                            sprite = spriteLeaser.sprites[index];
                            sprite.element = tile.block.sprite;
                        }

                        SetSpriteByTile(sprite, tile, camera);
                    }
                    else
                    {
                        if (index < 0)
                            continue;

                        drawTiles.RemoveAt(index);

                        spriteLeaser.sprites[index].RemoveFromContainer();
                        spriteLeaser.sprites.RemoveAt(index);
                    }
                }
            }
        }

        public void AddUpdateTile(Tile tile)
        {
            tilesQueue.Enqueue(tile);
        }

        private bool GetTileShowing(Tile tile)
        {
            if (tile.coordination.y + 2 > Chunk.Height)
                return true;
            else
            {
                int x = tile.coordination.x;
                int y = tile.coordination.y;
                int z = tile.coordination.z;

                if (!Tile.GetFullTile(chunk.GetTileAtWorldPosition(new Vector3Int(x, y + 1, z))))
                    return true;

                else if (!Tile.GetFullTile(chunk.GetTileAtWorldPosition(new Vector3Int(x + 1, y, z))))
                    return true;

                else if (!Tile.GetFullTile(chunk.GetTileAtWorldPosition(new Vector3Int(x - 1, y, z))))
                    return true;

                else if (!Tile.GetFullTile(chunk.GetTileAtWorldPosition(new Vector3Int(x, y, z + 1))))
                    return true;

                else if (!Tile.GetFullTile(chunk.GetTileAtWorldPosition(new Vector3Int(x, y, z - 1))))
                    return true;
            }

            return false;
        }
    }
}
