using System;
using UnityEngine;
using Custom;

public class AnimationRigQuadrupedal : AnimationRig <AnimationRigQuadrupedal>
{
    private EntityPart _body;
    private EntityPart _head;

    public float viewAngle { get; set; }
    public float moveSpeed { get; set; }

    public AnimationRigQuadrupedal(EntityPart body, EntityPart head) : base()
    {
        _body = body;
        _head = head;
    }
}