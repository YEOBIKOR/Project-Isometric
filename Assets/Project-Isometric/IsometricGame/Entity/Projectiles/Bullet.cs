using UnityEngine;
using System.Collections;

public class Bullet : Entity
{
    private Entity _owner;

    private Damage _damage;

    private EntityPart _part;

    private AudioClip _hitAudio;

    public Bullet(Entity owner, Damage damage, Vector3 velocity) : base(0f)
    {
        _owner = owner;
        _damage = damage;

        AttachPhysics(0.2f, 0.2f, 0f, Hit);

        this.velocity = velocity;

        _part = new EntityPart(this, Futile.atlasManager.GetElementWithName("entities/bullet8"));
        _entityParts.Add(_part);
        
        _hitAudio = Resources.Load<AudioClip>("SoundEffects/BulletHit");
    }

    public override void Update(float deltaTime)
    {
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
            {
                creature.ApplyDamage(_damage);
                Hit();
            }
        }
    }

    private void Hit()
    {
        world.worldMicrophone.PlaySound(_hitAudio, this);

        DespawnEntity();
    }
}
