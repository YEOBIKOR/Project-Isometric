using UnityEngine;
using System.Collections;

public class EntityAABBCollider : ICollidable<Entity>
{
    private Entity _owner;
    public Entity owner
    {
        get
        { return _owner; }
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

    public EntityAABBCollider(Entity owner, float width, float height)
    {
        _owner = owner;
        _width = width;
        _height = height;
    }

    public bool Collision(Vector3 position, float width, float height)
    {
        Vector3 min = _owner.worldPosition + new Vector3(_width * -0.5f, 0f, _width * -0.5f);
        Vector3 max = _owner.worldPosition + new Vector3(_width * 0.5f, _height, _width * 0.5f);

        Vector3 omin = position + new Vector3(width * -0.5f, 0f, width * -0.5f);
        Vector3 omax = position + new Vector3(width * 0.5f, height, width * 0.5f);

        if (min.x > omax.x || max.x < omin.x)
            return false;
        if (min.y > omax.y || max.y < omin.y)
            return false;
        if (min.z > omax.z || max.z < omin.z)
            return false;

        return true;
    }
}
