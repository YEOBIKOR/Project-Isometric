using UnityEngine;
using System.Collections;

public class EntityAABBCollider : ICollidable<Entity>
{
    public Entity owner
    {
        get
        { return null; }
    }

    private float _width;
    public float width
    {
        get
        { return _width; }
    }

    private float _height;
    public float height
    {
        get
        { return _height; }
    }

    public EntityAABBCollider(float width, float height)
    {
        _width = width;
        _height = height;
    }

    public bool Collision(Vector3 position, float width, float height)
    {
        return false;
    }
}
