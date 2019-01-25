using System;
using UnityEngine;

public abstract class PhysicalEntity : Entity, ICollidable
{
    private float _radius;
    private float _height;

    private bool _landed;

    public float radius
    {
        get { return _radius; }
        set { _radius = value; }
    }

    public float height
    {
        get { return _height; }
        set { _height = value; }
    }

    public bool landed
    {
        get { return _landed; }
        protected set { _landed = value; }
    }

    private bool _airControl;
    public bool airControl
    {
        get { return _airControl; }
        protected set { _airControl = value; }
    }

    public PhysicalEntity(float radius, float height) : base(radius * 2f)
    {
        _radius = radius;
        _height = height;
        _landed = false;
        _airControl = false;
    }

    public override void Update(float deltaTime)
    {
        if (chunk == null)
            return;

        chunk.GetCollidedEntites(this, OnCollisionWithOther);

        if (!landed)
            velocity += Vector3.up * -50f * deltaTime;

        if (_airControl || _landed)
        {
            Vector3 frictionForce = new Vector3(-velocity.x, 0f, -velocity.z).normalized * 30f;
            Vector3 appliedVelocity = velocity + (frictionForce * deltaTime);

            if (new Vector2(appliedVelocity.x, appliedVelocity.z).magnitude < 10f * deltaTime)
                appliedVelocity = new Vector3(0f, appliedVelocity.y, 0f);

            velocity = appliedVelocity;
        }

        Vector3 appliedPosition = worldPosition + velocity * deltaTime;
        Vector3 finalPosition = appliedPosition;
        Vector3 finalVelocity = velocity;

        int x = Mathf.FloorToInt(appliedPosition.x);
        int xMin = Mathf.FloorToInt(appliedPosition.x - radius);
        int xMax = Mathf.FloorToInt(appliedPosition.x + radius);
        int yMin = Mathf.FloorToInt(appliedPosition.y);
        int yMax = Mathf.FloorToInt(appliedPosition.y + height);
        int z = Mathf.FloorToInt(appliedPosition.z);
        int zMin = Mathf.FloorToInt(appliedPosition.z - radius);
        int zMax = Mathf.FloorToInt(appliedPosition.z + radius);

        _landed = false;

        if (appliedPosition.y + height >= 0f && appliedPosition.y <= Chunk.Height)
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
                    finalPosition.y = yMax - height;
                    finalVelocity.y = 0f;
                }
            }

            yMin = Mathf.FloorToInt(finalPosition.y);
            yMax = Mathf.FloorToInt(finalPosition.y + height) - 1;
            
            for (int y = yMin; y <= yMax; y++)
            {
                if (velocity.x < 0f)
                {
                    if (!Tile.GetCrossable(chunk.GetTileAtWorldPosition(xMin, y, z)))
                    {
                        finalPosition.x = xMin + 1 + radius;
                        finalVelocity.x = 0f;

                        break;
                    }
                }
                else if (velocity.x > 0f)
                {
                    if (!Tile.GetCrossable(chunk.GetTileAtWorldPosition(xMax, y, z)))
                    {
                        finalPosition.x = xMax - radius;
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
                        finalPosition.z = zMin + 1 + radius;
                        finalVelocity.z = 0f;

                        break;
                    }
                }
                else if (velocity.z > 0f)
                {
                    if (!Tile.GetCrossable(chunk.GetTileAtWorldPosition(x, y, zMax)))
                    {
                        finalPosition.z = zMax - radius;
                        finalVelocity.z = 0f;

                        break;
                    }
                }
            }
        }

        worldPosition = finalPosition;
        velocity = finalVelocity;

        base.Update(deltaTime);
    }

    public void AddForce(Vector3 force)
    {
        if (landed)
        {
            _landed = false;
            velocity += Vector3.up * -velocity.y;
        }
        
        velocity += force;
    }

    public virtual void OnCollisionWithOther(PhysicalEntity other)
    {
        Vector3 pushVelocity = new Vector3(
            worldPosition.x - other.worldPosition.x,
            0f,
            worldPosition.z - other.worldPosition.z).normalized;

        AddForce(pushVelocity);
    }

    public bool GetCollisionWithOther(PhysicalEntity other)
    {
        if (worldPosition.y > other.worldPosition.y + other.height || worldPosition.y + height < other.worldPosition.y)
            return false;

        Vector2 deltaVector = new Vector2(other.worldPosition.x - worldPosition.x, other.worldPosition.z - worldPosition.z);
        return deltaVector.sqrMagnitude <= (radius + other.radius) * (radius + other.radius);
    }

    public bool GetCollision(ICollidable other)
    {
        return false;
    }

    public override string debugString
    {
        get
        {
            return string.Concat(
                base.debugString,
                "landed : ", landed, "\n"
                );
        }
    }
}