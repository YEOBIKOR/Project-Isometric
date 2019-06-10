using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ChunkGenerator
{
    private World _world;

    private Task<Chunk> _chunkLoadTask;

    private List<IChunkGenerateProgress> _progresses;

    public ChunkGenerator(World world)
    {
        _world = world;
        _chunkLoadTask = new Task<Chunk>(GenerateChunk);

        InitializeProgresses();
    }

    public void InitializeProgresses()
    {
        _progresses = new List<IChunkGenerateProgress>();

        _progresses.Add(new ChunkBedrockGenerateProgress());
        _progresses.Add(new ChunkTerrainGenerateProgress());
        _progresses.Add(new ChunkGrowGrassProgress());
        _progresses.Add(new ChunkCreatureSpawnProgress());
    }

    public void RequestGenerateChunk(Chunk chunk)
    {
        _chunkLoadTask.AddTask(chunk);
    }

    public void GenerateChunk(Chunk chunk)
    {
        chunk.state = ChunkState.Loading;

        foreach (var progress in _progresses)
            progress.Generate(chunk);

        chunk.state = ChunkState.Loaded;
    }

    public Chunk[] GetLoadedChunkQueue()
    {
        return _chunkLoadTask.GetCompletedTasks();
    }
}