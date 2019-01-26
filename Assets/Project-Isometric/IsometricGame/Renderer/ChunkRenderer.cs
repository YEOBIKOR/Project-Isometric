using System.Collections.Generic;
using UnityEngine;

public class ChunkRenderer : IRenderer
{
    private Chunk chunk;

    private List<Tile> drawTiles;
    private Queue<Tile> tilesQueue;

    private bool showing;

    private bool _initialized;
    public bool initialized
    {
        get
        { return _initialized; }
    }

    public ChunkRenderer(Chunk chunk)
    {
        this.chunk = chunk;

        drawTiles = new List<Tile>();
        tilesQueue = new Queue<Tile>();

        showing = false;
        _initialized = false;
    }

    public void OnInitializeSprite(SpriteLeaser spriteLeaser, WorldCamera camera)
    {
        _initialized = true;

        UpdateTileRender(spriteLeaser, camera);
    }

    public void RenderUpdate(SpriteLeaser spriteLeaser, WorldCamera camera)
    {
        bool inRange = ((chunk.coordination + Vector2.one * 0.5f) * Chunk.Length - new Vector2(chunk.world.player.worldPosition.x, chunk.world.player.worldPosition.z)).sqrMagnitude < 1024f;

        if (showing && !inRange)
        {
            spriteLeaser.RemoveFromContainer();
            showing = false;
        }
        else if (!showing && inRange)
            showing = true;

        if (showing)
        {
            UpdateTileRender(spriteLeaser, camera);

            for (int index = 0; index < drawTiles.Count; index++)
            {
                Tile tile = drawTiles[index];
                FSprite sprite = spriteLeaser.sprites[index];

                SetSpriteByTile(sprite, tile, camera, true);

                if (sprite.container != null && !spriteLeaser.InScreenRect(sprite))
                    sprite.RemoveFromContainer();
                else if (sprite.container == null && spriteLeaser.InScreenRect(sprite))
                    camera.worldContainer.AddChild(sprite);
            }
        }
    }

    public void SetSpriteByTile(FSprite target, Tile tile, WorldCamera camera, bool optimizeColor = false)
    {
        Vector3 spritePosition = tile.coordination + Vector3.one * 0.5f;
        target.SetPosition(camera.GetScreenPosition(spritePosition));
        target.sortZ = camera.GetSortZ(spritePosition);

        if (optimizeColor)
            target.color = new Color(tile.coordination.x, tile.coordination.y, tile.coordination.z);
    }

    public bool GetShownByCamera(SpriteLeaser spriteLeaser, WorldCamera camera)
    {
        return false;
    }

    public void UpdateTileRender(SpriteLeaser spriteLeaser, WorldCamera camera)
    {
        while (tilesQueue.Count > 0)
        {
            Tile tile = tilesQueue.Dequeue();

            if (tile != null)
            {
                bool showing = false;

                if (tile.block.sprite != null)
                    if (GetTileShowing(tile))
                        showing = true;

                int index = drawTiles.IndexOf(tile);

                if (showing)
                {
                    FSprite sprite;

                    if (index < 0)
                    {
                        sprite = new FSprite(tile.block.sprite);
                        sprite.shader = IsometricMain.GetShader("WorldObject");

                        drawTiles.Add(tile);
                        spriteLeaser.sprites.Add(sprite);
                    }
                    else
                    {
                        sprite = spriteLeaser.sprites[index];
                        sprite.element = tile.block.sprite;
                    }

                    SetSpriteByTile(sprite, tile, camera);
                }
                else
                {
                    if (index < 0)
                        continue;

                    drawTiles.RemoveAt(index);

                    spriteLeaser.sprites[index].RemoveFromContainer();
                    spriteLeaser.sprites.RemoveAt(index);
                }
            }
        }
    }

    public void AddUpdateTile(Tile tile)
    {
        tilesQueue.Enqueue(tile);
    }

    private bool GetTileShowing(Tile tile)
    {
        if (tile.coordination.y + 2 > Chunk.Height)
            return true;
        else
        {
            int x = tile.coordination.x;
            int y = tile.coordination.y;
            int z = tile.coordination.z;

            if (!Tile.GetFullTile(chunk.GetTileAtWorldPosition(new Vector3Int(x, y + 1, z))))
                return true;

            else if (!Tile.GetFullTile(chunk.GetTileAtWorldPosition(new Vector3Int(x + 1, y, z))))
                return true;

            else if (!Tile.GetFullTile(chunk.GetTileAtWorldPosition(new Vector3Int(x - 1, y, z))))
                return true;

            else if (!Tile.GetFullTile(chunk.GetTileAtWorldPosition(new Vector3Int(x, y, z + 1))))
                return true;

            else if (!Tile.GetFullTile(chunk.GetTileAtWorldPosition(new Vector3Int(x, y, z - 1))))
                return true;
        }

        return false;
    }
}