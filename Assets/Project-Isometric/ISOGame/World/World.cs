using UnityEngine;
using System;
using System.Threading;
using System.Collections.Generic;

public class World
{
    public ISOGame game { get; private set; }
    public WorldCamera worldCamera { get; private set; }

    private Dictionary<long, Chunk> chunkMap;
    private Queue<Chunk> loadingChunks;

    private LinkedList<CosmeticDrawable> cosmeticDrawables;

    private int seed;

    public Player player { get; private set; }

    private const float loadChunkRange = 30f;
    private const float unloadChunkRange = 50f;

    private WorldProfiler worldProfiler;

    public World(ISOGame game)
    {
        this.game = game;

        worldCamera = new WorldCamera(this);

        chunkMap = new Dictionary<long, Chunk>(256);
        loadingChunks = new Queue<Chunk>();

        cosmeticDrawables = new LinkedList<CosmeticDrawable>();
        
        player = new Player();
        RequestLoadChunk(new Vector2Int(0, 0));

        worldProfiler = new WorldProfiler(this);
    }

    public void Update(float deltaTime)
    {
        if (Input.GetKeyDown(KeyCode.F3))
            worldProfiler.updateProfiler.SwitchProfiler();

        worldProfiler.updateProfiler.StartMeasureTime();

        Vector2 playerCoordinate = new Vector2(player.worldPosition.x, player.worldPosition.z);

        int xMin = Mathf.FloorToInt((playerCoordinate.x - loadChunkRange) / Chunk.length);
        int xMax = Mathf.FloorToInt((playerCoordinate.x + loadChunkRange) / Chunk.length);
        int yMin = Mathf.FloorToInt((playerCoordinate.y - loadChunkRange) / Chunk.length);
        int yMax = Mathf.FloorToInt((playerCoordinate.y + loadChunkRange) / Chunk.length);

        //for (int x = xMin; x <= xMax; x++)
        //{
        //    for (int y = yMin; y <= yMax; y++)
        //    {
        //        if (((new Vector2(x + 0.5f, y + 0.5f) * Chunk.length) - playerCoordinate).sqrMagnitude < loadChunkRange * loadChunkRange)
        //            RequestLoadChunk(new Vector2Int(x, y));
        //    }
        //}

        var chunks = chunkMap.Values;

        foreach (var chunk in chunks)
        {
            if (chunk.state == ChunkState.Loaded)
            {
                chunk.Update(deltaTime);

                Vector2 chunkDelta = new Vector2(chunk.coordinate.x + 0.5f, chunk.coordinate.y + 0.5f) * Chunk.length - playerCoordinate;
                if (chunkDelta.x * chunkDelta.x + chunkDelta.y * chunkDelta.y > unloadChunkRange * unloadChunkRange)
                    chunk.UnloadChunk();
            }
        }

        for(var iterator = cosmeticDrawables.First; iterator != null; iterator = iterator.Next)
        {
            CosmeticDrawable cosmeticDrawable = iterator.Value;
            
            if (cosmeticDrawable.world == this)
                cosmeticDrawable.Update(deltaTime);
            else
                cosmeticDrawables.Remove(iterator);
        }

        worldProfiler.updateProfiler.MeasureTime(UpdateProfilerType.ChunkUpdate);
        
        worldCamera.GraphicUpdate(deltaTime);
        worldProfiler.updateProfiler.MeasureTime(UpdateProfilerType.RenderUpdate);
    }

    public void OnTerminate()
    {
        worldCamera.CleanUp();
        worldProfiler.updateProfiler.CleanUp();
    }

    public void RequestLoadChunk(Vector2Int coordinate)
    {
        Chunk chunk;
        chunkMap.TryGetValue(((long)coordinate.x << 32) + coordinate.y, out chunk);

        if (chunk == null)
        {
            Chunk newChunk = new Chunk(this, coordinate);
            chunkMap.Add(((long)coordinate.x << 32) + coordinate.y, newChunk);

            loadingChunks.Enqueue(newChunk);
            
            if (loadingChunks.Count < 2)
            {
                Thread loadThread = new Thread(LoadChunk);
                loadThread.Priority = System.Threading.ThreadPriority.Lowest;

                loadThread.Start();
            }
        }

        else if (chunk.state == ChunkState.Unloaded)
            chunk.LoadChunk();
    }

    public void LoadChunk()
    {
        do
        {
            loadingChunks.Peek().GenerateTiles();
            loadingChunks.Dequeue();
        } while (loadingChunks.Count > 0);
    }

    public void OnChunkGenerated(Chunk chunk)
    {
        if (chunk.coordinate == new Vector2Int(0, 0))
        {
            SpawnEntity(player, new Vector3(1f, GetSurface(new Vector2(1f, 1f)), 1f));
            //SpawnEntity(new EntityBoss(), new Vector3(8f, 16f, 8f));
            //for (int i = 0; i < 300; i++)
            //{
            //    Vector2 position = Vector2.one * 10f;
            //    SpawnEntity(new EntityPpyongppyong(), new Vector3(position.x, 30f, position.y));
            //}
        }

        int spawnCount = (int)RXRandom.Range(0f, 1.1f);

        for (int i = 0; i < spawnCount; i++)
        {
            Vector2 position = (chunk.coordinate * Chunk.length) + new Vector2(RXRandom.Range(0f, 16f), RXRandom.Range(0f, 16f));
            SpawnEntity(new EntityPpyongppyong(), new Vector3(position.x, GetSurface(new Vector3(position.x, 0f, position.y)), position.y));
        }
        
        spawnCount = (int)RXRandom.Range(0f, 1.05f);

        for (int i = 0; i < spawnCount; i++)
        {
            Vector2 position = (chunk.coordinate * Chunk.length) + new Vector2(RXRandom.Range(0f, 16f), RXRandom.Range(0f, 16f));
            SpawnEntity(new EntityDipper(), new Vector3(position.x, GetSurface(new Vector3(position.x, 0f, position.y)), position.y));
        }
    }

    public Chunk GetChunkByCoordinate(Vector2Int chunkPosition)
    {
        try
        { return chunkMap[((long)chunkPosition.x << 32) + chunkPosition.y]; }
        catch
        { return null; }
    }

    public Chunk.ChunkGraphics GetChunkGraphicsAtPosition(Vector3Int tilePosition)
    {
        Chunk chunk = GetChunkByCoordinate(Chunk.ToChunkCoordinate(tilePosition));

        if (chunk != null)
            return chunk.chunkGraphics;
        return null;
    }

    public Tile GetTileAtPosition(Vector3Int tilePosition)
    {
        if (tilePosition.y < 0 || tilePosition.y >= Chunk.height)
            return null;

        Chunk chunk = GetChunkByCoordinate(Chunk.ToChunkCoordinate(tilePosition));

        if (chunk != null)
            return chunk[new Vector3Int(
                tilePosition.x & 0x0F,
                tilePosition.y,
                tilePosition.z & 0x0F)];
        return null;
    }

    public float GetSurface(Vector3 worldPosition)
    {
        Chunk chunk = GetChunkByCoordinate(Chunk.ToChunkCoordinate(worldPosition));

        if (chunk != null)
            return chunk.GetSurface(worldPosition);

        return 0f;
    }

    public RayTrace RayTraceTile(Vector3 startPosition, Vector3 direction, float distance)
    {
        Vector3 tracePosition = startPosition;
        
        while (true)
        {
            Vector3 intersectionDelta = Vector3.positiveInfinity;
            Vector3 faceDirection = Vector3.zero;

            for (int i = 0; i < 3; i++)
            {
                if (direction[i] == 0f)
                    continue;

                int nextStep = direction[i] > 0f ? Mathf.FloorToInt(tracePosition[i]) + 1 : Mathf.CeilToInt(tracePosition[i]) - 1;
                Vector3 v = direction / direction[i] * (nextStep - tracePosition[i]);

                if (v.sqrMagnitude < intersectionDelta.sqrMagnitude)
                {
                    intersectionDelta = v;

                    switch (i)
                    {
                        case 0:
                            faceDirection = direction[0] < 0f ? Vector3.right : Vector3.left;
                            break;

                        case 1:
                            faceDirection = direction[1] < 0f ? Vector3.up : Vector3.down;
                            break;

                        case 2:
                            faceDirection = direction[2] < 0f ? Vector3.forward : Vector3.back;
                            break;
                    }
                }
            }

            tracePosition += intersectionDelta;

            if (tracePosition.y < 0f || tracePosition.y > Chunk.height)
                return new RayTrace();

            Vector3Int tilePosition = new Vector3Int();
            for (int i = 0; i < 3; i++)
                tilePosition[i] = direction[i] > 0f ? Mathf.FloorToInt(tracePosition[i]) : Mathf.CeilToInt(tracePosition[i]) - 1;

            Tile tile = GetTileAtPosition(tilePosition);
            if (Tile.GetFullTile(tile))
                return new RayTrace(tracePosition, faceDirection, tilePosition);
        }
    }

    public void SpawnEntity(Entity entity, Vector3 position)
    {
        if (entity.chunk == null)
        {
            Chunk chunk = GetChunkByCoordinate(Chunk.ToChunkCoordinate(position));

            if (chunk != null)
            {
                chunk.AddEntity(entity);
                entity.OnSpawn(chunk, position);
            }
        }
        else
            Debug.LogWarning(string.Concat("Entity ", entity.GetType(), " has already spawn."));
    }

    public void AddCosmeticDrawble(CosmeticDrawable cosmeticDrawable)
    {
        cosmeticDrawables.AddLast(cosmeticDrawable);
        cosmeticDrawable.OnShow(this);
    }

    public void PlaceBlock(Vector3Int tilePosition, Block newBlock)
    {
        Tile tile = GetTileAtPosition(tilePosition);

        tile.SetBlock(newBlock);
    }

    public void DestroyBlock(Vector3Int tilePosition)
    {
        if (tilePosition.y > 0)
        {
            Tile tile = GetTileAtPosition(tilePosition);

            tile.SetBlock(Block.GetBlockByKey("air"));
        }
    }
}
