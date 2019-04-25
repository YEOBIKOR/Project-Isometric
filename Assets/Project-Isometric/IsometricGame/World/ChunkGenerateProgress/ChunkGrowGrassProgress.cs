
public class ChunkGrowGrassProgress : IChunkGenerateProgress
{
    public void Generate(Chunk chunk)
    {
        Block blockDirt = Block.GetBlockByKey("dirt");
        Block blockGrass = Block.GetBlockByKey("grass");

        for (int i = 0; i < Chunk.Length; i++)
        {
            for (int j = 0; j < Chunk.Length; j++)
            {
                for (int k = 0; k < Chunk.Height; k++)
                {
                    if (chunk[i, k, j].block == blockDirt)
                    {
                        bool suitable = k >= Chunk.Height;

                        if (!suitable)
                            suitable = chunk[i, k + 1, j].block == Block.BlockAir;

                        if (suitable)
                            chunk[i, k, j].SetBlock(blockGrass);
                    }
                }
            }
        }
    }
}