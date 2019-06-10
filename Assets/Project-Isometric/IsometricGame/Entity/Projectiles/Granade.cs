using UnityEngine;
using System.Collections;

public class Granade : Entity
{
    private Entity _owner;

    private Damage _damage;

    private EntityPart _part;

    private AudioClip _hitAudio;

    public Granade(Entity owner, Damage damage, Vector3 velocity) : base(0f)
    {
        _owner = owner;
        _damage = damage;
        
        AttachPhysics(0.2f, 0.2f);

        this.velocity = velocity;

        _part = new EntityPart(this, Futile.atlasManager.GetElementWithName("entities/bullet12"));
        _entityParts.Add(_part);

        _hitAudio = Resources.Load<AudioClip>("SoundEffects/BulletHit");
    }

    public override void Update(float deltaTime)
    {
        if (time > 3f)
            Explode();

        _part.worldPosition = worldPosition;

        chunk.GetCollidedEntities(worldPosition, 0.5f, 0.5f, OnCollision);

        base.Update(deltaTime);
    }

    private void OnCollision(Entity entity)
    {
        if (spawned)
        {
            EntityCreature creature = entity as EntityCreature;

            if (creature != null && creature != _owner)
                Explode();
        }
    }

    private void Explode()
    {
        new WorldExplosion(world, worldPosition, 32f).Execute();

        DespawnEntity();
    }
}
