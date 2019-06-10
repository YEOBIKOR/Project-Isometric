using UnityEngine;

public class WorldExplosion : IPositionable
{
    private World _world;

    private Vector3 _position;

    private float _power;

    private AudioClip _explosionAudio;

    public WorldExplosion(World world, Vector3 position, float power)
    {
        _world = world;
        _position = position;
        _power = power;

        _explosionAudio = Resources.Load<AudioClip>("SoundEffects/large-explosion-1");
    }

    public Vector3 worldPosition
    {
        get
        { return _position; }
    }

    public void Execute()
    {
        Chunk chunk = _world.GetChunkByCoordinate(Chunk.ToChunkCoordinate(_position));

        if (chunk != null)
        {
            chunk.GetCollidedEntities(_position - Vector3.down, 10f, 10f, EffectEntity);
        }

        _world.QuakeAtPosition(_position);
        _world.worldCamera.ShakeCamera(24f);
        _world.worldCamera.worldMicrophone.PlaySound(_explosionAudio, this);
    }

    private void EffectEntity(Entity entity)
    {
        Vector3 delta = entity.worldPosition - _position;

        if (entity.physics != null)
        {

            entity.physics.AddForce(delta.normalized * _power / delta.magnitude);
        }

        EntityCreature creature = entity as EntityCreature;

        if (creature != null)
            creature.ApplyDamage(new Damage(entity, _power / delta.magnitude));
    }
}