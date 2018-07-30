using UnityEngine;
using System.Collections;
using Custom;

public class EntityPpyongppyong : EntityCreature
{
    private float jumpTime;

    public EntityPpyongppyong() : base(0.3f, 2.0f, 35f)
    {
        entityParts.Add(new EntityPart(this, "entityppyongppyonghead"));
        entityParts.Add(new EntityPart(this, "entityppyongppyongface"));
        entityParts.Add(new EntityPart(this, "entityppyongppyongbody"));
        entityParts[2].sortZOffset = 0.5f;
        entityParts.Add(new EntityPart(this, "entityppyongppyongarm"));
        entityParts[3].viewAngle = 60f;
        entityParts[3].sortZOffset = 0.5f;
        entityParts.Add(new EntityPart(this, "entityppyongppyongarm"));
        entityParts[4].viewAngle = -60f;
        entityParts[4].sortZOffset = 0.5f;
    }

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);

        if (landed)
        {
            Vector3 deltaV = world.player.worldPosition - worldPosition;

            if (deltaV.sqrMagnitude < 10f * 10f)
            {
                velocity = velocity * 0.5f;

                Vector3 deltaVNormalized = deltaV.normalized;
                AddForce(new Vector3(deltaVNormalized.x, Random.Range(13f, 15f), deltaVNormalized.z));
            }

            else if (!(jumpTime > 0f))
            {
                AddForce(new Vector3(Random.Range(-1f, 1f), Random.Range(13f, 15f), Random.Range(-1f, 1f)));
                jumpTime = Random.Range(0.5f, 3.0f);
            }
        }

        if (velocity.x != 0f && velocity.z != 0f)
            viewAngle = Mathf.LerpAngle(viewAngle, Mathf.Atan2(velocity.z, velocity.x) * Mathf.Rad2Deg, deltaTime * 10f);

        if (landed)
            jumpTime -= deltaTime;

        entityParts[0].worldPosition = worldPosition + new Vector3(0f, 1.6f, 0f);
        entityParts[1].worldPosition = worldPosition + CustomMath.HorizontalRotate(new Vector3(0.2f, 1.8f, 0f), viewAngle);
        entityParts[2].worldPosition = worldPosition + new Vector3(0f, 0.5f, 0f);
        entityParts[3].worldPosition = worldPosition + CustomMath.HorizontalRotate(new Vector3(0.5f, 0.6f, 0.2f), viewAngle);
        entityParts[4].worldPosition = worldPosition + CustomMath.HorizontalRotate(new Vector3(0.5f, 0.6f, -0.2f), viewAngle);

        for (int index = 0; index < entityParts.Count; index++)
            entityParts[index].viewAngle = viewAngle;

        entityParts[3].viewAngle = viewAngle - 30f;
        entityParts[4].viewAngle = viewAngle + 30f;
    }

    public override void OnCollisionWithOther(PhysicalEntity other)
    {
        base.OnCollisionWithOther(other);

        if (other == world.player)
            other.ApplyDamage(new Damage(this));
    }
}
