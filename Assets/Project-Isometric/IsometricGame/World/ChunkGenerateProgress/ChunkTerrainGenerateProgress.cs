using UnityEngine;

public class ChunkTerrainGenerateProgress : IChunkGenerateProgress
{
    public void Generate(Chunk chunk)
    {
        Vector2Int coordination = chunk.coordination;

        Block blockSand = Block.GetBlockByKey("sand");
        Block blockStone = Block.GetBlockByKey("stone");
        Block blockDirt = Block.GetBlockByKey("dirt");

        for (int i = 0; i < Chunk.Length; i++)
        {
            for (int j = 0; j < Chunk.Length; j++)
            {
                int y = Mathf.CeilToInt(Mathf.PerlinNoise((i + coordination.x * Chunk.Length + 1024f) * 0.1f, (j + coordination.y * Chunk.Length + 1024f) * 0.1f) * 4f) + 2;

                // int y = 1;
                // if ((i == 2 && j > 2 && j < 6) || (i > 0 && i < 4 && j == 3)) y = 3;//Hi

                for (int k = 0; k < Chunk.Height; k++)
                {
                    Block block = Block.BlockAir;

                    if (k < y)
                    {
                        int biome = 0; //Mathf.RoundToInt(Mathf.PerlinNoise((i + coordination.x * Chunk.Length + 1024f) * 0.01f, (j + coordination.y * Chunk.Length + 1024f) * 0.01f));

                        if (biome > 0)
                            block = blockSand;
                        else
                            block = k > 2 + RXRandom.Range(0, 5) ? blockStone : blockDirt;
                    }

                    if (chunk[i, k, j].block == Block.BlockAir)
                        chunk[i, k, j].SetBlock(block);
                }
            }
        }
    }
}