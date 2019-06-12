using System;
using UnityEngine;

public class EntityPhysics
{
    private EntityAABBCollider _collider;
    public EntityAABBCollider collider
    {
        get
        { return _collider; }
    }

    private float _gravityModifier;

    private Vector3 _velocity;
    public Vector3 velocity
    {
        get
        { return _velocity; }
        set
        { _velocity = value; }
    }

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

    private Action _onTileCallback;

    public EntityPhysics(EntityAABBCollider collider, float gravityModifier = 1f, Action onTileCallback = null)
    {
        _collider = collider;
        _onTileCallback = onTileCallback;

        _gravityModifier = gravityModifier;

        _velocity = Vector3.zero;
        _landed = false;
        _airControl = false;
    }

    private const float Gravity = -39.24f;

    public void ApplyPhysics(Chunk chunk, Entity owner, float deltaTime, ref Vector3 position)
    {
        float width = _collider.width;
        float height = _collider.height;

        position = position + _velocity * deltaTime;

        if (!_landed && _gravityModifier != 0f)
            _velocity += Vector3.up * Gravity * _gravityModifier * deltaTime;

        if (_airControl || _landed)
        {
            Vector3 frictionForce = new Vector3(-_velocity.x, 0f, -_velocity.z).normalized * 30f;
            Vector3 appliedVelocity = _velocity + (frictionForce * deltaTime);

            if (new Vector2(appliedVelocity.x, appliedVelocity.z).magnitude < 10f * deltaTime)
                appliedVelocity = new Vector3(0f, appliedVelocity.y, 0f);

            _velocity = appliedVelocity;
        }

        Vector3 appliedPosition = position;
        Vector3 finalPosition = appliedPosition;
        Vector3 finalVelocity = _velocity;

        int x = Mathf.FloorToInt(appliedPosition.x);
        int xMin = Mathf.FloorToInt(appliedPosition.x - width);
        int xMax = Mathf.FloorToInt(appliedPosition.x + width);
        int yMin = Mathf.FloorToInt(appliedPosition.y);
        int yMax = Mathf.FloorToInt(appliedPosition.y + height);
        int z = Mathf.FloorToInt(appliedPosition.z);
        int zMin = Mathf.FloorToInt(appliedPosition.z - width);
        int zMax = Mathf.FloorToInt(appliedPosition.z + width);

        _landed = false;

        bool collided = false;

        if (appliedPosition.y + height >= 0f && appliedPosition.y <= Chunk.Height)
        {
            if (_velocity.y < 0f)
            {
                if (!Tile.GetCrossable(chunk.GetTileAtWorldPosition(x, yMin, z)))
                {
                    finalPosition.y = yMin + 1;
                    finalVelocity.y = 0f;

                    _landed = true;

                    collided = true;
                }
            }
            else if (_velocity.y > 0f)
            {
                if (!Tile.GetCrossable(chunk.GetTileAtWorldPosition(x, yMax, z)))
                {
                    finalPosition.y = yMax - height;
                    finalVelocity.y = 0f;

                    collided = true;
                }
            }

            yMin = Mathf.FloorToInt(finalPosition.y);
            yMax = Mathf.FloorToInt(finalPosition.y + height) - 1;

            for (int y = yMin; y <= yMax; y++)
            {
                if (_velocity.x < 0f)
                {
                    if (!Tile.GetCrossable(chunk.GetTileAtWorldPosition(xMin, y, z)))
                    {
                        finalPosition.x = xMin + 1 + width;
                        finalVelocity.x = 0f;

                        collided = true;

                        break;
                    }
                }
                else if (_velocity.x > 0f)
                {
                    if (!Tile.GetCrossable(chunk.GetTileAtWorldPosition(xMax, y, z)))
                    {
                        finalPosition.x = xMax - width;
                        finalVelocity.x = 0f;

                        collided = true;

                        break;
                    }
                }
            }

            x = Mathf.FloorToInt(finalPosition.x);

            for (int y = yMin; y <= yMax; y++)
            {
                if (_velocity.z < 0f)
                {
                    if (!Tile.GetCrossable(chunk.GetTileAtWorldPosition(x, y, zMin)))
                    {
                        finalPosition.z = zMin + 1 + width;
                        finalVelocity.z = 0f;

                        collided = true;

                        break;
                    }
                }
                else if (_velocity.z > 0f)
                {
                    if (!Tile.GetCrossable(chunk.GetTileAtWorldPosition(x, y, zMax)))
                    {
                        finalPosition.z = zMax - width;
                        finalVelocity.z = 0f;

                        collided = true;

                        break;
                    }
                }
            }
        }

        position = finalPosition;
        _velocity = finalVelocity;

        if (collided && _onTileCallback != null)
            _onTileCallback();

        Action<Entity> callback = delegate (Entity other)
        {
            if (other != owner)
            {
                Vector3 pushVelocity = new Vector3(
                    owner.worldPosition.x - other.worldPosition.x,
                    0f,
                    owner.worldPosition.z - other.worldPosition.z).normalized;

                AddForce(pushVelocity);
            }
        };

        chunk.GetCollidedEntities(position, collider.width, collider.height, callback);
    }

    public void AddForce(Vector3 force)
    {
        if (_landed)
        {
            _landed = false;
            _velocity += Vector3.up * -_velocity.y;
        }

        _velocity += force;
    }
}
