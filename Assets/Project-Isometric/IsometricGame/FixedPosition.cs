using UnityEngine;

public struct FixedPosition : IPositionable
{
    private Vector3 _worldPosition;
    public Vector3 worldPosition
    {
        get
        { return _worldPosition; }
    }

    public FixedPosition(Vector3 worldPosition)
    {
        _worldPosition = worldPosition;
    }
}
