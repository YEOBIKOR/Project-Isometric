using UnityEngine;
using System.Collections;

public class ChunkLoader : IPositionable
{
    public Vector3 worldPosition
    {
        get
        { throw new System.NotImplementedException(); }
    }

    private float _loadRange;
}