using UnityEngine;
using System.Collections;
using Custom;

public class EntityDipper : EntityCreature
{
    public override Rect boundRect
    {
        get
        { return new Rect(0f, 24f, 36f, 48f); }
    }

    public EntityDipper() : base(0.8f, 3.0f, 35f)
    {
        _entityParts.Add(new EntityPart(this, "dipperbody"));
        _entityParts.Add(new EntityPart(this, "dippereye"));
        _entityParts.Add(new EntityPart(this, "dippereye"));
        _entityParts.Add(new EntityPart(this, "dipperleg"));
        _entityParts.Add(new EntityPart(this, "dipperleg"));
        _entityParts.Add(new EntityPart(this, "dipperleg"));
        _entityParts.Add(new EntityPart(this, "dipperleg"));
        
        moveSpeed = 1f;
    }

    public override void Update(float deltaTime)
    {
        Vector3 deltaV = world.player.worldPosition - worldPosition;
        if (deltaV.sqrMagnitude < 10f * 10f)
            MoveTo(new Vector2(deltaV.x, deltaV.z), 50f * deltaTime);

        if (velocity.x != 0f && velocity.z != 0f)
            viewAngle = Mathf.LerpAngle(viewAngle, Mathf.Atan2(velocity.z, velocity.x) * Mathf.Rad2Deg, deltaTime * 10f);

        _entityParts[0].worldPosition = worldPosition + new Vector3(0f, 2f, 0f);
        _entityParts[1].worldPosition = worldPosition + CustomMath.HorizontalRotate(new Vector3(0.8f, 2.2f, 0f), viewAngle + 30f);
        _entityParts[2].worldPosition = worldPosition + CustomMath.HorizontalRotate(new Vector3(0.8f, 2.2f, 0f), viewAngle - 30f);
        _entityParts[3].worldPosition = worldPosition + CustomMath.HorizontalRotate(new Vector3(0.5f, 0.4f, 0.5f), viewAngle);
        _entityParts[4].worldPosition = worldPosition + CustomMath.HorizontalRotate(new Vector3(0.5f, 0.4f, -0.5f), viewAngle);
        _entityParts[5].worldPosition = worldPosition + CustomMath.HorizontalRotate(new Vector3(-0.5f, 0.4f, 0.5f), viewAngle);
        _entityParts[6].worldPosition = worldPosition + CustomMath.HorizontalRotate(new Vector3(-0.5f, 0.4f, -0.5f), viewAngle);

        for (int index = 0; index < _entityParts.Count; index++)
            _entityParts[index].viewAngle = viewAngle;

        base.Update(deltaTime);
    }

    public override string name
    {
        get
        { return "Dipper"; }
    }

    //public override void OnCollisionWithOther(PhysicalEntity other)
    //{
    //    base.OnCollisionWithOther(other);

    //    if (other == world.player)
    //        other.ApplyDamage(new Damage(this));
    //}
}
