using UnityEngine;

public interface ITarget : IPositionable
{
    Rect boundRect { get; }

    void Trigger();
}