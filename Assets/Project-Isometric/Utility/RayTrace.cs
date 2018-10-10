using System;
using UnityEngine;

public struct RayTrace
{
    public bool hit { get; private set; }
    public Vector3 hitPosition { get; private set; }
    public Vector3 hitDirection { get; private set; }
    public Vector3Int hitTilePosition { get; private set; }

    public RayTrace(Vector3 hitPosition, Vector3 hitDirection, Vector3Int hitTilePosition)
    {
        hit = true;

        this.hitPosition = hitPosition;
        this.hitDirection = hitDirection;
        this.hitTilePosition = hitTilePosition;
    }

    public Tile GetTileIn(World world)
    {
        return world.GetTileAtPosition(hitTilePosition);
    }

    public Tile GetTileOn(World world)
    {
        return world.GetTileAtPosition(hitTilePosition + new Vector3Int((int)hitDirection.x, (int)hitDirection.y, (int)hitDirection.z));
    }
}