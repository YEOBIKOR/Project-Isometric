using UnityEngine;
using System.Collections;
using Custom;

public class EntityPpyongppyong : EntityCreature
{
    private float _jumpTime;

    public EntityPpyongppyong() : base(0.3f, 2.0f, 35f)
    {
        _entityParts.Add(new EntityPart(this, "entityppyongppyonghead"));
        _entityParts.Add(new EntityPart(this, "entityppyongppyongface"));
        _entityParts.Add(new EntityPart(this, "entityppyongppyongbody"));
        _entityParts[2].sortZOffset = 0.5f;
        _entityParts.Add(new EntityPart(this, "entityppyongppyongarm"));
        _entityParts[3].viewAngle = 60f;
        _entityParts[3].sortZOffset = 0.5f;
        _entityParts.Add(new EntityPart(this, "entityppyongppyongarm"));
        _entityParts[4].viewAngle = -60f;
        _entityParts[4].sortZOffset = 0.5f;
    }

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);

        if (_physics.landed)
        {
            Vector3 deltaV = world.player.worldPosition - worldPosition;

            if (deltaV.sqrMagnitude < 10f * 10f)
            {
                velocity = velocity * 0.5f;

                Vector3 deltaVNormalized = deltaV.normalized;
                _physics.AddForce(new Vector3(deltaVNormalized.x, Random.Range(13f, 15f), deltaVNormalized.z));
            }

            else if (!(_jumpTime > 0f))
            {
                _physics.AddForce(new Vector3(Random.Range(-1f, 1f), Random.Range(13f, 15f), Random.Range(-1f, 1f)));
                _jumpTime = Random.Range(0.5f, 3.0f);
            }
        }

        if (velocity.x != 0f && velocity.z != 0f)
            viewAngle = Mathf.LerpAngle(viewAngle, Mathf.Atan2(velocity.z, velocity.x) * Mathf.Rad2Deg, deltaTime * 10f);

        if (_physics.landed)
            _jumpTime -= deltaTime;

        _entityParts[0].worldPosition = worldPosition + new Vector3(0f, 1.6f, 0f);
        _entityParts[1].worldPosition = worldPosition + CustomMath.HorizontalRotate(new Vector3(0.2f, 1.8f, 0f), viewAngle);
        _entityParts[2].worldPosition = worldPosition + new Vector3(0f, 0.5f, 0f);
        _entityParts[3].worldPosition = worldPosition + CustomMath.HorizontalRotate(new Vector3(0.5f, 0.6f, 0.2f), viewAngle);
        _entityParts[4].worldPosition = worldPosition + CustomMath.HorizontalRotate(new Vector3(0.5f, 0.6f, -0.2f), viewAngle);

        for (int index = 0; index < _entityParts.Count; index++)
            _entityParts[index].viewAngle = viewAngle;

        _entityParts[3].viewAngle = viewAngle - 30f;
        _entityParts[4].viewAngle = viewAngle + 30f;
    }

    public override string name
    {
        get
        { return "Ppyongppyong"; }
    }
}
