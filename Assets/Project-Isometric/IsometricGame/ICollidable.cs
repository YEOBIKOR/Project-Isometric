using UnityEngine;

public interface ICollidable <T>
{
    T owner { get; }

    float width { get; }
    float height { get; }

    bool Collision(Vector3 position, float width, float height);
}