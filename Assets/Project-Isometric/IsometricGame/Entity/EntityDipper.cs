﻿using UnityEngine;
using System.Collections;
using Custom;

public class EntityDipper : EntityCreature
{
    public EntityDipper() : base(0.8f, 3.0f, 35f)
    {
        entityParts.Add(new EntityPart(this, "dipperbody"));
        entityParts.Add(new EntityPart(this, "dippereye"));
        entityParts.Add(new EntityPart(this, "dippereye"));
        entityParts.Add(new EntityPart(this, "dipperleg"));
        entityParts.Add(new EntityPart(this, "dipperleg"));
        entityParts.Add(new EntityPart(this, "dipperleg"));
        entityParts.Add(new EntityPart(this, "dipperleg"));
        
        moveSpeed = 1f;
    }

    public override void Update(float deltaTime)
    {
        Vector3 deltaV = world.player.worldPosition - worldPosition;
        if (deltaV.sqrMagnitude < 10f * 10f)
            MoveTo(new Vector2(deltaV.x, deltaV.z), 50f * deltaTime);

        if (velocity.x != 0f && velocity.z != 0f)
            viewAngle = Mathf.LerpAngle(viewAngle, Mathf.Atan2(velocity.z, velocity.x) * Mathf.Rad2Deg, deltaTime * 10f);

        entityParts[0].worldPosition = worldPosition + new Vector3(0f, 2f, 0f);
        entityParts[1].worldPosition = worldPosition + CustomMath.HorizontalRotate(new Vector3(0.8f, 2.2f, 0f), viewAngle + 30f);
        entityParts[2].worldPosition = worldPosition + CustomMath.HorizontalRotate(new Vector3(0.8f, 2.2f, 0f), viewAngle - 30f);
        entityParts[3].worldPosition = worldPosition + CustomMath.HorizontalRotate(new Vector3(0.5f, 0.4f, 0.5f), viewAngle);
        entityParts[4].worldPosition = worldPosition + CustomMath.HorizontalRotate(new Vector3(0.5f, 0.4f, -0.5f), viewAngle);
        entityParts[5].worldPosition = worldPosition + CustomMath.HorizontalRotate(new Vector3(-0.5f, 0.4f, 0.5f), viewAngle);
        entityParts[6].worldPosition = worldPosition + CustomMath.HorizontalRotate(new Vector3(-0.5f, 0.4f, -0.5f), viewAngle);

        for (int index = 0; index < entityParts.Count; index++)
            entityParts[index].viewAngle = viewAngle;

        base.Update(deltaTime);
    }

    public override void OnCollisionWithOther(PhysicalEntity other)
    {
        base.OnCollisionWithOther(other);

        if (other == world.player)
            other.ApplyDamage(new Damage(this));
    }
}