using System;
using UnityEngine;

public class Shadow : EntityPart
{
    private static FAtlasElement[] sprites;

    private float shadowScale;

    public override FAtlasElement element
    {
        get
        {
            float scaleFactor = shadowScale / ((owner.worldPosition.y - worldPosition.y) * 0.2f + 1f);
            return sprites[(int)(Mathf.Clamp01(scaleFactor) * 12f)];
        }
    }

    public Shadow(Entity owner, float shadowScale) : base(owner, null as FAtlasElement)
    {
        sortZOffset = 0.5f;
        this.shadowScale = shadowScale;

        color = Color.black;
        alpha = 0.5f;
    }

    private static void LoadTexture()
    {
        sprites = new FAtlasElement[13];

        for (int index = 0; index < sprites.Length; index++)
            sprites[index] = Futile.atlasManager.GetElementWithName(string.Concat("entities/shadow", index + 1));
    }

    public override void Update(float deltaTime)
    {
        for (int y = Mathf.Min(owner.tilePosition.y, Chunk.Height - 1); y >= 0; y--)
        {
            if (Tile.GetFullTile(owner.chunk.GetTileAtWorldPosition(owner.tilePosition.x, y, owner.tilePosition.z)))
            {
                worldPosition = new Vector3(owner.worldPosition.x, y + 1, owner.worldPosition.z);
                break;
            }
        }

        base.Update(deltaTime);
    }

    public override void OnInitializeSprite(SpriteLeaser spriteLeaser, WorldCamera camera)
    {
        if (sprites == null)
            LoadTexture();

        base.OnInitializeSprite(spriteLeaser, camera);
    }
}