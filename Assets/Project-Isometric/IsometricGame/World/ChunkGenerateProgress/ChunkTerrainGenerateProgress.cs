using UnityEngine;

public class ChunkTerrainGenerateProgress : IChunkGenerateProgress
{
    public void Generate(Chunk chunk)
    {
        Vector2Int coordination = chunk.coordination;
        
        Block blockStone = Block.GetBlockByKey("stone");
        Block blockDirt = Block.GetBlockByKey("dirt");

        for (int i = 0; i < Chunk.Length; i++)
        {
            for (int j = 0; j < Chunk.Length; j++)
            {
                Vector2 perlinCoordination = (new Vector2(i, j) + coordination * Chunk.Length + Vector2.one * 1024f) * 0.1f;
                int y = Mathf.CeilToInt(Mathf.PerlinNoise(perlinCoordination.x, perlinCoordination.y) * 10f) + 2;

                // int y = 1;
                // if ((i == 2 && j > 2 && j < 6) || (i > 0 && i < 4 && j == 3)) y = 3;//Hi

                for (int k = 0; k < Chunk.Height; k++)
                {
                    Block block = Block.BlockAir;

                    if (k < y)
                        block = k > 2 + RXRandom.Range(0, 5) ? blockStone : blockDirt;

                    if (chunk[i, k, j].block == Block.BlockAir)
                        chunk[i, k, j].SetBlock(block);
                }
            }
        }
    }
}