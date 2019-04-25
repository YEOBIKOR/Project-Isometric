using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ChunkGenerator
{
    private World _world;

    private Queue<Chunk> _inChunkQueue;
    private Queue<Chunk> _outChunkQueue;

    private List<IChunkGenerateProgress> _progresses;

    public ChunkGenerator(World world)
    {
        _world = world;

        _inChunkQueue = new Queue<Chunk>();
        _outChunkQueue = new Queue<Chunk>();

        _progresses = new List<IChunkGenerateProgress>();
        InitializeProgresses();
    }

    public void InitializeProgresses()
    {
        _progresses.Add(new ChunkBedrockGenerateProgress());
        _progresses.Add(new ChunkTerrainGenerateProgress());
        _progresses.Add(new ChunkGrowGrassProgress());
    }

    public void RequestGenerateChunk(Chunk chunk)
    {
        lock (_inChunkQueue)
        {
            _inChunkQueue.Enqueue(chunk);
        }

        if (_inChunkQueue.Count < 2)
        {
            Thread loadThread = new Thread(GenerateChunks);
            loadThread.Start();
        }
    }

    private void GenerateChunks()
    {
        do
        {
            Chunk chunk = _inChunkQueue.Peek();
            GenerateChunk(chunk);

            lock (_inChunkQueue)
            {
                _inChunkQueue.Dequeue();
            }

            lock (_outChunkQueue)
            {
                _outChunkQueue.Enqueue(chunk);
            }

        } while (_inChunkQueue.Count > 0);
    }

    public void GenerateChunk(Chunk chunk)
    {
        chunk.state = ChunkState.Loading;

        foreach (var progress in _progresses)
            progress.Generate(chunk);

        chunk.state = ChunkState.Loaded;
    }

    public Queue<Chunk> GetLoadedChunkQueue()
    {
        return _outChunkQueue;
    }
}