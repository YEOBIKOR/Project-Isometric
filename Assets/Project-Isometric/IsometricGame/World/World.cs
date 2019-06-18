using System;
using System.Collections.Generic;
using UnityEngine;

using Isometric.Interface;
using Isometric.Items;

public class World : ISerializable<World.Serialized>
{
    private readonly IsometricGame _game;
    public IsometricGame game
    {
        get
        { return _game; }
    }

    private readonly WorldCamera _worldCamera;
    public WorldCamera worldCamera
    {
        get
        { return _worldCamera; }
    }
    public WorldMicrophone worldMicrophone
    {
        get
        { return _worldCamera.worldMicrophone; }
    }

    private string _worldName;
    public string worldName
    {
        get
        { return _worldName; }
    }

    private float _worldTime;

    private ChunkGenerator _chunkGenerator;
    private LinkedList<Chunk> _chunks;
    private Dictionary<int, Chunk> _chunkMap;

    private LinkedList<CosmeticRenderer> _cosmeticDrawables;

    private List<ITarget> _targets;
    public List<ITarget> targets
    {
        get
        { return _targets; }
    }

    private int _seed;

    public Player player { get; private set; }

    private readonly CameraHUDMenu _cameraHUD;
    public CameraHUDMenu cameraHUD
    {
        get
        { return _cameraHUD; }
    }

    private const float LoadChunkRange = 30f;
    private const float UnloadChunkRange = 50f;

    private const int NumMaxEpicenters = 4;

    private Queue<Vector4> _epicenters;

    private WorldProfiler _worldProfiler;

    public World(IsometricGame game, string worldName)
    {
        _game = game;
        _worldName = worldName;

        _worldCamera = new WorldCamera(this);

        _worldTime = 0f;

        _chunkGenerator = new ChunkGenerator(this);
        _chunks = new LinkedList<Chunk>();
        _chunkMap = new Dictionary<int, Chunk>(256);

        _cosmeticDrawables = new LinkedList<CosmeticRenderer>();

        _targets = new List<ITarget>();

        player = new Player();

        _cameraHUD = new CameraHUDMenu(_game, _worldCamera);
        game.AddSubLoopFlow(_cameraHUD);

        _worldProfiler = new WorldProfiler(this);

        _epicenters = new Queue<Vector4>(NumMaxEpicenters);
        UpdateEpicenters();
    }

    public void Update(float deltaTime)
    {
        _worldTime += deltaTime;

        if (Input.GetKeyDown(KeyCode.F3))
            _worldProfiler.updateProfiler.SwitchProfiler();

        _worldProfiler.updateProfiler.StartMeasureTime();

        foreach (var loadedChunk in _chunkGenerator.GetLoadedChunkQueue())
            OnChunkGenerated(loadedChunk);

        ChunkLoadUpdate(deltaTime);

        DrawablesUpdate(deltaTime);

        _worldProfiler.updateProfiler.MeasureTime(UpdateProfilerType.ChunkUpdate);

        Shader.SetGlobalFloat("_WorldTime", _worldTime);
        worldCamera.GraphicUpdate(deltaTime);
        _worldProfiler.updateProfiler.MeasureTime(UpdateProfilerType.RenderUpdate);

        if (_epicenters.Count > 0)
        {
            if (_worldTime - _epicenters.Peek().z > 3f)
            {
                _epicenters.Dequeue();
                UpdateEpicenters();
            }
        }
    }

    private void ChunkLoadUpdate(float deltaTime)
    {
        Vector2 playerCoordinate = new Vector2(player.worldPosition.x, player.worldPosition.z);

        int xMin = Mathf.FloorToInt((playerCoordinate.x - LoadChunkRange) / Chunk.Length);
        int xMax = Mathf.FloorToInt((playerCoordinate.x + LoadChunkRange) / Chunk.Length);
        int yMin = Mathf.FloorToInt((playerCoordinate.y - LoadChunkRange) / Chunk.Length);
        int yMax = Mathf.FloorToInt((playerCoordinate.y + LoadChunkRange) / Chunk.Length);

        for (int x = xMin; x <= xMax; x++)
        {
            for (int y = yMin; y <= yMax; y++)
            {
                if (((new Vector2(x + 0.5f, y + 0.5f) * Chunk.Length) - playerCoordinate).sqrMagnitude < LoadChunkRange * LoadChunkRange)
                    RequestLoadChunk(new Vector2Int(x, y));
            }
        }

        for (LinkedListNode<Chunk> node = _chunks.First; node != null; node = node.Next)
        {
            Chunk chunk = node.Value;

            if (chunk.state == ChunkState.Loaded)
            {
                chunk.Update(deltaTime);

                Vector2 chunkDelta = new Vector2(chunk.coordination.x + 0.5f, chunk.coordination.y + 0.5f) * Chunk.Length - playerCoordinate;
                if (chunkDelta.x * chunkDelta.x + chunkDelta.y * chunkDelta.y > UnloadChunkRange * UnloadChunkRange)
                    chunk.UnloadChunk();
            }
        }
    }

    private void DrawablesUpdate(float deltaTime)
    {
        for (var iterator = _cosmeticDrawables.First; iterator != null; iterator = iterator.Next)
        {
            CosmeticRenderer cosmeticDrawable = iterator.Value;

            if (cosmeticDrawable.world == this)
                cosmeticDrawable.Update(deltaTime);
            else
                _cosmeticDrawables.Remove(iterator);
        }
    }

    public void OnTerminate()
    {
        worldCamera.CleanUp();
        _worldProfiler.updateProfiler.CleanUp();
    }


    public void RequestLoadChunk(Vector2Int coordination)
    {
        Chunk chunk;
        _chunkMap.TryGetValue((coordination.x << 16) + coordination.y, out chunk);

        if (chunk == null)
        {
            chunk = new Chunk(this, coordination);

            AddChunk(chunk);
            _chunkGenerator.RequestGenerateChunk(chunk);
        }

        else if (chunk.state == ChunkState.Unloaded)
            chunk.LoadChunk();
    }

    public void AddChunk(Chunk chunk)
    {
        Vector2Int coordination = chunk.coordination;

        _chunks.AddLast(chunk);
        _chunkMap.Add((coordination.x << 16) + coordination.y, chunk);
    }

    public void OnChunkGenerated(Chunk chunk)
    {
        chunk.OnChunkActivate();

        if (chunk.coordination == new Vector2Int(0, 0))
        {
            Vector3 standUpPosition = new Vector3(1f, GetSurface(new Vector2(1f, 1f)), 1f);

            SpawnEntity(player, standUpPosition);
            SpawnEntity(new TutorialNPC(), standUpPosition);

            worldCamera.SetCameraTarget(player, true);

            SpawnEntity(new EntityBoss(), new Vector3(8f, 16f, 8f));

            //for (int i = 0; i < 1; i++)
            //{
            //    Vector2 position = new Vector3(10f, GetSurface(new Vector2(10f, 10f)), 10f);
            //    SpawnEntity(new EntityPpyongppyong(), position);
            //    SpawnEntity(new EntityDipper(), position);
            //}
        }

        Vector2Int[] nearbyCoordinations = new Vector2Int[]
        {
            new Vector2Int(0, -1),
            new Vector2Int(-1, -1),
            new Vector2Int(-1, 0),
            new Vector2Int(-1, 1),
            new Vector2Int(0, 1),
            new Vector2Int(1, 1),
            new Vector2Int(1, 1),
            new Vector2Int(1, 0)
        };

        for (int index = 0; index < nearbyCoordinations.Length; index++)
        {
            Chunk nearbyChunk = GetChunkByCoordinate(chunk.coordination + nearbyCoordinations[index]);

            if (nearbyChunk != null)
            {
                chunk.SetNearbyChunk((index + 4) % 8, nearbyChunk);
                nearbyChunk.SetNearbyChunk(index, chunk);
            }
        }
    }

    public Chunk GetChunkByCoordinate(Vector2Int chunkPosition)
    {
        try
        { return _chunkMap[(chunkPosition.x << 16) + chunkPosition.y]; }
        catch
        { return null; }
    }

    public ChunkRenderer GetChunkGraphicsAtPosition(Vector3Int tilePosition)
    {
        Chunk chunk = GetChunkByCoordinate(Chunk.ToChunkCoordinate(tilePosition));

        if (chunk != null)
            return chunk.chunkRenderer;
        return null;
    }

    public Tile GetTileAtPosition(Vector3Int tilePosition)
    {
        if (tilePosition.y < 0 || tilePosition.y >= Chunk.Height)
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

            if (tracePosition.y < 0f || tracePosition.y > Chunk.Height)
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
                entity.worldPosition = position;

                chunk.AddEntity(entity);
                entity.OnSpawn();

                OnSpawnEntity(entity);
            }
        }
        else
            Debug.LogWarning(string.Concat("Entity ", entity.GetType(), " has already spawn."));
    }

    public void OnSpawnEntity(Entity entity)
    {
        if (entity is ITarget && entity != player)
            _targets.Add(entity as ITarget);
    }

    public void OnDespawnEntity(Entity entity)
    {
        if (entity is ITarget && entity != player)
            _targets.Remove(entity as ITarget);
    }

    public void AddCosmeticDrawble(CosmeticRenderer cosmeticDrawable)
    {
        _cosmeticDrawables.AddLast(cosmeticDrawable);
        cosmeticDrawable.OnShow(this);
    }

    public void PlaceBlock(Vector3Int tilePosition, Block newBlock)
    {
        Tile tile = GetTileAtPosition(tilePosition);

        tile.SetBlock(newBlock);
    }

    public void DestroyBlock(Vector3Int tilePosition)
    {
        Tile tile = GetTileAtPosition(tilePosition);

        if (tile == null)
            return;

        ItemStack item = tile.block.OnDropItem();
        if (item != null)
        {
            Entity droppedItem = new DroppedItem(item);
            SpawnEntity(droppedItem, tilePosition + Vector3.one * 0.5f);
        }

        tile.SetBlock(Block.BlockAir);
    }

    public void QuakeAtPosition(Vector3 epicenter)
    {
        if (_epicenters.Count >= NumMaxEpicenters)
            return;

        Vector4 vector = new Vector4();

        vector.x = epicenter.x;
        vector.y = epicenter.z;
        vector.z = _worldTime;

        _epicenters.Enqueue(vector);
        UpdateEpicenters();
    }

    private void UpdateEpicenters()
    {
        Vector4[] array = new Vector4[NumMaxEpicenters];
        Vector4[] epicenters = _epicenters.ToArray();

        for (int index = 0; index < epicenters.Length; index++)
            array[index] = epicenters[index];

        Shader.SetGlobalVectorArray("_Epicenters", array);
        Shader.SetGlobalInt("_NumEpicenters", epicenters.Length);
    }

    public Serialized Serialize()
    {
        Serialized data = new Serialized();

        data.chunks = new Chunk.Serialized[_chunks.Count];

        LinkedListNode<Chunk> node = _chunks.First;
        for (int index = 0; index < _chunks.Count; index++, node = node.Next)
        {
            data.chunks[index] = node.Value.Serialize();
        }

        data.playerPositionX = player.worldPosition.x;
        data.playerPositionY = player.worldPosition.y;
        data.playerPositionZ = player.worldPosition.z;
        data.playerViewAngle = player.viewAngle;

        return data;
    }

    public void Deserialize(Serialized data)
    {
        for (int index = 0; index < data.chunks.Length; index++)
        {
            Chunk.Serialized chunkData = data.chunks[index];

            Vector2Int coordination = new Vector2Int
            {
                x = chunkData.coordinationX,
                y = chunkData.coordinationY
            };

            Chunk chunk = new Chunk(this, coordination);
            AddChunk(chunk);

            chunk.Deserialize(chunkData);
            OnChunkGenerated(chunk);
        }

        player.worldPosition = new Vector3(
            data.playerPositionX,
            data.playerPositionY,
            data.playerPositionZ
            );
        player.viewAngle = data.playerViewAngle;

        worldCamera.SetCameraTarget(player, true);
    }

    [Serializable]
    public struct Serialized
    {
        public Chunk.Serialized[] chunks;

        public float playerPositionX;
        public float playerPositionY;
        public float playerPositionZ;
        public float playerViewAngle;
    }
}
