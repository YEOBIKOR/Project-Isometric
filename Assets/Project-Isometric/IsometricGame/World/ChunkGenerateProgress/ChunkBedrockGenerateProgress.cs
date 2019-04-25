
public class ChunkBedrockGenerateProgress : IChunkGenerateProgress
{
    public void Generate(Chunk chunk)
    {
        Block blockBedrock = Block.GetBlockByKey("bedrock");

        for (int i = 0; i < Chunk.Length * Chunk.Length; i++)
        {
            chunk[i / Chunk.Length, 0, i % Chunk.Length].SetBlock(blockBedrock);
        }
    }
}