using UnityEngine;
using Custom;

class DecomposeCreaturePart : PhysicalEntity
{
    private float viewAngle;
    private float decayTime;

    public DecomposeCreaturePart(EntityCreature creature, EntityPart part) : base(0.25f, 0.5f)
    {
        velocity = CustomMath.HorizontalRotate(creature.velocity, Random.Range(-30f, 30f));

        entityParts.Add(new EntityPart(this, part.element));
        entityParts[0].sortZOffset = 1f;

        viewAngle = part.viewAngle;
        decayTime = Random.Range(5f, 10f);
    }

    public override void Update(float deltaTime)
    {
        decayTime -= deltaTime;
        if (decayTime < 0f)
            DespawnEntity();

        entityParts[0].worldPosition = worldPosition;
        entityParts[0].viewAngle = viewAngle;
        entityParts[0].alpha = Mathf.Clamp01(decayTime);

        base.Update(deltaTime);
    }
}