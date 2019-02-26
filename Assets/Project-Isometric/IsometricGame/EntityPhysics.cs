using UnityEngine;
using System.Collections;

public class EntityPhysics
{
    private float _width;
    private float _height;

    private bool _landed;
    public bool landed
    {
        get
        { return _landed; }
    }

    private bool _airControl;
    public bool airControl
    {
        get
        { return _airControl; }
        set
        { _airControl = value; }
    }

    public EntityPhysics(float width, float height)
    {
        _width = width;
        _height = height;
    }

    public void ApplyPhysics(Chunk chunk, float deltaTime, ref Vector3 position, ref Vector3 velocity)
    {
        if (!_landed)
            velocity += Vector3.up * -50f * deltaTime;

        if (_airControl || _landed)
        {
            Vector3 frictionForce = new Vector3(-velocity.x, 0f, -velocity.z).normalized * 30f;
            Vector3 appliedVelocity = velocity + (frictionForce * deltaTime);

            if (new Vector2(appliedVelocity.x, appliedVelocity.z).magnitude < 10f * deltaTime)
                appliedVelocity = new Vector3(0f, appliedVelocity.y, 0f);

            velocity = appliedVelocity;
        }

        Vector3 appliedPosition = position;
        Vector3 finalPosition = appliedPosition;
        Vector3 finalVelocity = velocity;

        int x = Mathf.FloorToInt(appliedPosition.x);
        int xMin = Mathf.FloorToInt(appliedPosition.x - _width);
        int xMax = Mathf.FloorToInt(appliedPosition.x + _width);
        int yMin = Mathf.FloorToInt(appliedPosition.y);
        int yMax = Mathf.FloorToInt(appliedPosition.y + _height);
        int z = Mathf.FloorToInt(appliedPosition.z);
        int zMin = Mathf.FloorToInt(appliedPosition.z - _width);
        int zMax = Mathf.FloorToInt(appliedPosition.z + _width);

        _landed = false;

        if (appliedPosition.y + _height >= 0f && appliedPosition.y <= Chunk.Height)
        {
            if (velocity.y < 0f)
            {
                if (!Tile.GetCrossable(chunk.GetTileAtWorldPosition(x, yMin, z)))
                {
                    finalPosition.y = yMin + 1;
                    finalVelocity.y = 0f;

                    _landed = true;
                }
            }
            else if (velocity.y > 0f)
            {
                if (!Tile.GetCrossable(chunk.GetTileAtWorldPosition(x, yMax, z)))
                {
                    finalPosition.y = yMax - _height;
                    finalVelocity.y = 0f;
                }
            }

            yMin = Mathf.FloorToInt(finalPosition.y);
            yMax = Mathf.FloorToInt(finalPosition.y + _height) - 1;

            for (int y = yMin; y <= yMax; y++)
            {
                if (velocity.x < 0f)
                {
                    if (!Tile.GetCrossable(chunk.GetTileAtWorldPosition(xMin, y, z)))
                    {
                        finalPosition.x = xMin + 1 + _width;
                        finalVelocity.x = 0f;

                        break;
                    }
                }
                else if (velocity.x > 0f)
                {
                    if (!Tile.GetCrossable(chunk.GetTileAtWorldPosition(xMax, y, z)))
                    {
                        finalPosition.x = xMax - _width;
                        finalVelocity.x = 0f;

                        break;
                    }
                }
            }

            x = Mathf.FloorToInt(finalPosition.x);

            for (int y = yMin; y <= yMax; y++)
            {
                if (velocity.z < 0f)
                {
                    if (!Tile.GetCrossable(chunk.GetTileAtWorldPosition(x, y, zMin)))
                    {
                        finalPosition.z = zMin + 1 + _width;
                        finalVelocity.z = 0f;

                        break;
                    }
                }
                else if (velocity.z > 0f)
                {
                    if (!Tile.GetCrossable(chunk.GetTileAtWorldPosition(x, y, zMax)))
                    {
                        finalPosition.z = zMax - _width;
                        finalVelocity.z = 0f;

                        break;
                    }
                }
            }
        }

        position = finalPosition;
        velocity = finalVelocity;
    }

    public void AddForce(Vector3 force, ref Vector3 velocity)
    {
        if (_landed)
        {
            _landed = false;
            velocity += Vector3.up * -velocity.y;
        }

        velocity += force;
    }
}
