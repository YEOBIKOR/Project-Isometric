using System;
using UnityEngine;

public class ThrowableRock : Entity
{
    private EntityCreature _behaviour;
    private Damage _rockDamage;

    private float _decayTime;

    public ThrowableRock(EntityCreature behaviour) : base(0.4f)
    {
        this._behaviour = behaviour;

        _entityParts.Add(new EntityPart(this, "throwablerock"));

        _rockDamage = new Damage(this);

        AttachPhysics(0.2f, 0.4f);

        _decayTime = 5f;
    }

    public override void Update(float deltaTime)
    {
        if (_physics.landed)
            _decayTime -= deltaTime;
        if (_decayTime < 0f)
            DespawnEntity();

        _entityParts[0].worldPosition = worldPosition + Vector3.up * 0.3f;

        base.Update(deltaTime);
    }

    //public override void OnCollisionWithOther(PhysicalEntity other)
    //{
    //    base.OnCollisionWithOther(other);

    //    if (other != behaviour && !landed && other is EntityCreature)
    //    {
    //        other.ApplyDamage(rockDamage);
    //        velocity = new Vector3(-velocity.x, velocity.y, -velocity.z) * 0.5f;
    //    }
    //}
}