using System;
using UnityEngine;

public class ThrowableRock : PhysicalEntity
{
    private EntityCreature behaviour;
    private Damage rockDamage;

    private float decayTime = 5f;

    public ThrowableRock(EntityCreature behaviour) : base(0.2f, 0.4f)
    {
        this.behaviour = behaviour;

        entityParts.Add(new EntityPart(this, "throwablerock"));

        rockDamage = new Damage(this);
    }

    public override void Update(float deltaTime)
    {
        if (landed)
            decayTime -= deltaTime;
        if (decayTime < 0f)
            DespawnEntity();

        entityParts[0].worldPosition = worldPosition + Vector3.up * 0.3f;

        base.Update(deltaTime);
    }

    public override void OnCollisionWithOther(PhysicalEntity other)
    {
        base.OnCollisionWithOther(other);

        if (other != behaviour && !landed && other is EntityCreature)
        {
            other.ApplyDamage(rockDamage);
            velocity = new Vector3(-velocity.x, velocity.y, -velocity.z) * 0.5f;
        }
    }
}