using UnityEngine;
using System.Collections;

public class Bullet : Entity
{
    private Damage _damage;

    private EntityPart _part;

    public Bullet(Damage damage, Vector3 velocity) : base(0.2f)
    {
        _damage = damage;

        AttachPhysics(0.2f, 0.2f, 0f);

        this.velocity = velocity;

        _part = new EntityPart(this, Futile.atlasManager.GetElementWithName("entities/bullet8"));
        entityParts.Add(_part);
    }

    public override void Update(float deltaTime)
    {
        worldPosition += velocity * deltaTime;

        _part.worldPosition = worldPosition;

        base.Update(deltaTime);
    }
}
