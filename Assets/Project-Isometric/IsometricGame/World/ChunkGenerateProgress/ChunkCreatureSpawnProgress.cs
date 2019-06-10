using UnityEngine;

public class ChunkCreatureSpawnProgress : IChunkGenerateProgress
{
    public void Generate(Chunk chunk)
    {
        int spawnCount = (int)RXRandom.Range(0f, 5f);

        for (int i = 0; i < spawnCount; i++)
            AddEntityRandomPosition(chunk, new EntityPpyongppyong());

        spawnCount = (int)RXRandom.Range(0f, 1.05f);

        for (int i = 0; i < spawnCount; i++)
            AddEntityRandomPosition(chunk, new EntityDipper());
    }

    private void AddEntityRandomPosition(Chunk chunk, Entity entity)
    {
        Vector2 coordination = (chunk.coordination * Chunk.Length) + new Vector2(RXRandom.Range(0f, 16f), RXRandom.Range(0f, 16f));
        
        float y = chunk.GetSurface(coordination);
        entity.worldPosition = new Vector3(coordination.x, y, coordination.y);

        chunk.AddEntity(entity);
    }
}