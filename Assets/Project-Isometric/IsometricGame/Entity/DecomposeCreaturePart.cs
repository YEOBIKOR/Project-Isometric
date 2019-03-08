using UnityEngine;
using Custom;

class DecomposeCreaturePart : Entity
{
    private float viewAngle;
    private float decayTime;

    public DecomposeCreaturePart(EntityCreature creature, EntityPart part) : base(0.5f)
    {
        entityParts.Add(new EntityPart(this, part.element));
        entityParts[0].sortZOffset = 1f;

        viewAngle = part.viewAngle;
        decayTime = Random.Range(5f, 10f);

        AttachPhysics(0.25f, 0.5f);

        velocity = CustomMath.HorizontalRotate(creature.velocity, Random.Range(-30f, 30f));
    }

    public override void Update(float deltaTime)
    {
        decayTime -= deltaTime;
        if (decayTime < 0f)
            DespawnEntity();

        entityParts[0].worldPosition = worldPosition;
        entityParts[0].viewAngle = viewAngle;
        entityParts[0].color = new Color(0.8f, 0.8f, 0.8f);
        entityParts[0].alpha = Mathf.Clamp01(decayTime);

        base.Update(deltaTime);
    }
}