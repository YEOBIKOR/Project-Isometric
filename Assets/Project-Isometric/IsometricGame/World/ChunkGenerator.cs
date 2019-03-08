using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ChunkGenerator
{
    private World _world;

    private Queue<Chunk> _inChunkQueue;
    private Queue<Chunk> _outChunkQueue;

    public ChunkGenerator(World world)
    {
        _world = world;

        _inChunkQueue = new Queue<Chunk>();
        _outChunkQueue = new Queue<Chunk>();
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
        Vector2Int coordination = chunk.coordination;
        chunk.state = ChunkState.Loading;
        
        for (int i = 0; i < Chunk.Length; i++)
        {
            for (int j = 0; j < Chunk.Length; j++)
            {
                int y = Mathf.CeilToInt(Mathf.PerlinNoise((i + coordination.x * Chunk.Length + 1024f) * 0.1f, (j + coordination.y * Chunk.Length + 1024f) * 0.1f) * 4f) + 2;
                //int y = 1;
                //if ((i == 2 && j > 2 && j < 6) || (i > 0 && i < 4 && j == 3)) y = 3;//Hi

                for (int k = 0; k < Chunk.Height; k++)
                {
                    Block block = Block.BlockAir;

                    if (k == 0)
                        block = Block.GetBlockByKey("bedrock");
                    else if (k < y)
                    {
                        int biome = 0; //Mathf.RoundToInt(Mathf.PerlinNoise((i + coordination.x * Chunk.Length + 1024f) * 0.01f, (j + coordination.y * Chunk.Length + 1024f) * 0.01f));

                        if (biome > 0)
                            block = Block.GetBlockByKey("sand");
                        else
                            block = Block.GetBlockByKey(k > 2 + RXRandom.Range(0, 5) ? "stone" : (k + 1 == y) ? "grass" : "dirt");
                    }

                    chunk[i, k, j].SetBlock(block);
                }
            }
        }

        chunk.state = ChunkState.Loaded;
    }

    public Queue<Chunk> GetLoadedChunkQueue()
    {
        return _outChunkQueue;
    }
}
