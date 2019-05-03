using UnityEngine;
using Custom;

class DecomposeCreaturePart : Entity
{
    private float _viewAngle;
    private float _decayTime;

    public DecomposeCreaturePart(EntityCreature creature, EntityPart part) : base(0.5f)
    {
        _entityParts.Add(new EntityPart(this, part.element));
        _entityParts[0].sortZOffset = 1f;

        _viewAngle = part.viewAngle;
        _decayTime = Random.Range(5f, 10f);

        AttachPhysics(0.25f, 0.5f);

        velocity = CustomMath.HorizontalRotate(creature.velocity, Random.Range(-30f, 30f));
    }

    public override void Update(float deltaTime)
    {
        _decayTime -= deltaTime;
        if (_decayTime < 0f)
            DespawnEntity();

        _entityParts[0].worldPosition = worldPosition;
        _entityParts[0].viewAngle = _viewAngle;
        _entityParts[0].color = new Color(0.8f, 0.8f, 0.8f);
        _entityParts[0].alpha = Mathf.Clamp01(_decayTime);

        base.Update(deltaTime);
    }
}