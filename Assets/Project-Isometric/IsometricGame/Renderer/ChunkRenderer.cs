using System.Collections.Generic;
using UnityEngine;

public class ChunkRenderer : IRenderer
{
    private Chunk _chunk;

    private List<Tile> _drawTiles;
    private Queue<Tile> _tilesQueue;

    private bool _showing;

    private bool _initialized;
    public bool initialized
    {
        get
        { return _initialized; }
    }

    private FShader _shader;

    public ChunkRenderer(Chunk chunk)
    {
        _chunk = chunk;

        _drawTiles = new List<Tile>();
        _tilesQueue = new Queue<Tile>();

        _showing = false;
        _initialized = false;

        _shader = IsometricMain.GetShader("WorldObject");
    }

    public void OnInitializeSprite(SpriteLeaser spriteLeaser, WorldCamera camera)
    {
        _initialized = true;

        UpdateTileRender(spriteLeaser, camera);
    }

    public void RenderUpdate(SpriteLeaser spriteLeaser, WorldCamera camera)
    {
        bool inRange = ((_chunk.coordination + Vector2.one * 0.5f) * Chunk.Length - new Vector2(_chunk.world.player.worldPosition.x, _chunk.world.player.worldPosition.z)).sqrMagnitude < 1024f;

        if (_showing && !inRange)
        {
            spriteLeaser.RemoveFromContainer();
            _showing = false;
        }
        else if (!_showing && inRange)
            _showing = true;

        if (_showing)
        {
            UpdateTileRender(spriteLeaser, camera);

            for (int index = 0; index < _drawTiles.Count; index++)
            {
                Tile tile = _drawTiles[index];
                FSprite sprite = spriteLeaser.sprites[index];

                // if (camera.turning)
                    SetSpriteByTile(sprite, tile, camera, true);

                bool inScreenRect = spriteLeaser.InScreenRect(sprite);

                if (sprite.container != null && !inScreenRect)
                    sprite.RemoveFromContainer();
                else if (sprite.container == null && inScreenRect)
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
        while (_tilesQueue.Count > 0)
        {
            Tile tile = _tilesQueue.Dequeue();

            if (tile != null)
            {
                bool showing = false;

                if (tile.block.sprite != null)
                    if (GetTileShowing(tile))
                        showing = true;

                int index = _drawTiles.IndexOf(tile);

                if (showing)
                {
                    FSprite sprite;

                    if (index < 0)
                    {
                        sprite = new FSprite(tile.block.sprite);
                        sprite.shader = _shader;

                        _drawTiles.Add(tile);
                        spriteLeaser.sprites.Add(sprite);
                    }
                    else
                    {
                        sprite = spriteLeaser.sprites[index];
                        sprite.element = tile.block.sprite;
                    }

                    SetSpriteByTile(sprite, tile, camera, true);
                }
                else
                {
                    if (index < 0)
                        continue;

                    _drawTiles.RemoveAt(index);

                    spriteLeaser.sprites[index].RemoveFromContainer();
                    spriteLeaser.sprites.RemoveAt(index);
                }
            }
        }
    }

    public void AddUpdateTile(Tile tile)
    {
        _tilesQueue.Enqueue(tile);
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

            if (!Tile.GetFullTile(_chunk.GetTileAtWorldPosition(new Vector3Int(x, y + 1, z))))
                return true;

            else if (!Tile.GetFullTile(_chunk.GetTileAtWorldPosition(new Vector3Int(x + 1, y, z))))
                return true;

            else if (!Tile.GetFullTile(_chunk.GetTileAtWorldPosition(new Vector3Int(x - 1, y, z))))
                return true;

            else if (!Tile.GetFullTile(_chunk.GetTileAtWorldPosition(new Vector3Int(x, y, z + 1))))
                return true;

            else if (!Tile.GetFullTile(_chunk.GetTileAtWorldPosition(new Vector3Int(x, y, z - 1))))
                return true;
        }

        return false;
    }
}