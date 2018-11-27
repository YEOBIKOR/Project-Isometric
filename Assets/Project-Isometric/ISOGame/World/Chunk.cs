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
    public World world { get; private set; }
    public WorldCamera worldCamera { get { return world.worldCamera; } }
    public ISOGame game { get { return world.game; } }

    public ChunkState state { get; private set; }

    public const int length = 16;
    public const int height = 16;

    public Tile this[Vector3Int gridPosition]
    {
        get
        { return this[gridPosition.x, gridPosition.y, gridPosition.z]; }
    }

    public Tile this[int x, int y, int z]
    {
        get
        { return tiles[x, y, z]; }
    }

    private Tile[,,] tiles;
    private LinkedList<Entity> entities;
    
    public Vector2Int coordinate { get; private set; }
    public ChunkGraphics chunkGraphics { get; private set; }

    public Chunk(World world, Vector2Int coordinate)
    {
        this.world = world;
        this.coordinate = coordinate;

        state = ChunkState.Unloaded;

        tiles = new Tile[length, height, length];
        entities = new LinkedList<Entity>();

        chunkGraphics = new ChunkGraphics(this);
    }

    public void GenerateTiles()
    {
        state = ChunkState.Loading;

        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < length; j++)
            {
                int y = Mathf.CeilToInt(Mathf.PerlinNoise((i + coordinate.x * length + 1024f) * 0.1f, (j + coordinate.y * length + 1024f) * 0.1f) * 8f) + 4;
                //int y = 1;
                //if ((i == 2 && j > 2 && j < 6) || (i > 0 && i < 4 && j == 3)) y = 3;//Hi

                for (int k = 0; k < height; k++)
                {
                    tiles[i, k, j] = new Tile(this, (ushort)(i | (k << 4) | (j << 12)));

                    if (k == 0)
                        tiles[i, k, j].SetBlock(Block.GetBlockByKey("bedrock"));
                    else if (k < y)
                    {
                        //float noise = Mathf.PerlinNoise((i + coordinate.x * length) * 0.1f, (j + coordinate.y * length) * 0.1f);
                        tiles[i, k, j].SetBlock(Block.GetBlockByKey(k > 4 + RXRandom.Range(0, 5) ? "stone" : "grass"));
                    }
                    else
                        tiles[i, k, j].SetBlock(Block.BlockAir);
                }

                //y = Mathf.CeilToInt(Mathf.PerlinNoise((i + coordinate.x * length + 1024f) * 0.2f, (j + coordinate.y * length + 1024f) * 0.2f) * 9f);
                //for (int k = (int)GetSurface(new Vector2(i, j)); k < y; k++)
                //    tiles[i, k, j].SetBlock(new BlockSolid(2));
            }
        }

        world.OnChunkGenerated(this);

        LoadChunk();
    }

    public void LoadChunk()
    {
        state = ChunkState.Loaded;

        worldCamera.AddDrawable(chunkGraphics);
    }

    public void UnloadChunk()
    {
        state = ChunkState.Unloaded;
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
            int x = tile.worldPosition.x;
            int y = tile.worldPosition.y;
            int z = tile.worldPosition.z;

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
        if (position.y < 0 || position.y >= height)
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

        for (int y = height - 1; !(y < 0); y--)
            if (Tile.GetFullTile(this[new Vector3Int(tilePosition.x & 0x0F, y, tilePosition.y & 0x0F)]))
                return y + 1f;

        return 0f;
    }

    public bool GetPositionInChunk(Vector3 position)
    {
        return Chunk.ToChunkCoordinate(position) == coordinate;
    }

    public void GetCollidedEntites(PhysicalEntity owner)
    {
        LinkedListNode<Entity> node = entities.First;
        while (node != null)
        {
            PhysicalEntity entity = node.Value as PhysicalEntity;
            if (entity != null)
            {
                if (entity != owner && owner.GetCollisionWithOther(entity))
                    owner.OnCollisionWithOther(entity);
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

    public class ChunkGraphics : IDrawable
    {
        private Chunk chunk;
        
        private List<Tile> drawTiles;
        private Queue<Tile> updateDrawTiles;

        bool showing;

        public ChunkGraphics(Chunk chunk)
        {
            this.chunk = chunk;

            drawTiles = new List<Tile>();
            updateDrawTiles = new Queue<Tile>();

            showing = false;
        }

        public void OnInitializeSprite(SpriteLeaser spriteLeaser, WorldCamera camera)
        {
            UpdateTileRender(spriteLeaser, camera);
        }

        public void GraphicUpdate(SpriteLeaser spriteLeaser, WorldCamera camera)
        {
            bool inRange = ((chunk.coordinate + Vector2.one * 0.5f) * Chunk.length - new Vector2(chunk.world.player.worldPosition.x, chunk.world.player.worldPosition.z)).sqrMagnitude < 1024f;

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
            Vector3 spritePosition = tile.worldPosition + Vector3.one * 0.5f;
            target.SetPosition(camera.GetScreenPosition(spritePosition));
            target.sortZ = camera.GetSortZ(spritePosition);
            
            if (optimizeColor)
                target.color = new Color(tile.worldPosition.x, tile.worldPosition.y, tile.worldPosition.z);
        }

        public bool GetShownByCamera(SpriteLeaser spriteLeaser, WorldCamera camera)
        {
            return false;
        }

        public void UpdateTileRender(SpriteLeaser spriteLeaser, WorldCamera camera)
        {
            while (updateDrawTiles.Count > 0)
            {
                Tile tile = updateDrawTiles.Dequeue();

                if (tile != null)
                {
                    bool showing = false;

                    if (tile.block.sprite != null)
                        if (GetTileShowing(tile))
                            showing = true;

                    if (showing)
                    {
                        drawTiles.Add(tile);

                        FSprite sprite = new FSprite(tile.block.sprite);
                        sprite.shader = ISOMain.GetShader("WorldObject");

                        spriteLeaser.sprites.Add(sprite);
                        SetSpriteByTile(sprite, tile, camera);
                    }
                    //else
                    //{
                    //    Debug.Log(tile.worldPosition);

                    //    int index = drawTiles.IndexOf(tile);
                    //    if (index < 0)
                    //        return;

                    //    drawTiles.RemoveAt(index);

                    //    spriteLeaser.sprites[index].RemoveFromContainer();
                    //    spriteLeaser.sprites.RemoveAt(index);
                    //}
                }
            }
        }

        public void AddUpdateTile(Tile tile)
        {
            updateDrawTiles.Enqueue(tile);
        }

        private bool GetTileShowing(Tile tile)
        {
            if (tile.worldPosition.y + 2 > Chunk.height)
                return true;
            else
            {
                int x = tile.worldPosition.x;
                int y = tile.worldPosition.y;
                int z = tile.worldPosition.z;

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
