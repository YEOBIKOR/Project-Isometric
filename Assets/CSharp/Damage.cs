using System;
using UnityEngine;

public class Damage
{
    private Entity behaviour;

    public Damage(Entity behaviour)
    {
        this.behaviour = behaviour;
    }

    public void OnApplyDamage(Entity target)
    {
        if (target.damagedCooldown < 0f)
        {
            if (target is PhysicalEntity)
            {
                PhysicalEntity physicalEntity = target as PhysicalEntity;

                Vector2 pushVelocity = new Vector3(
                    target.worldPosition.x - behaviour.worldPosition.x,
                    target.worldPosition.z - behaviour.worldPosition.z).normalized;

                physicalEntity.AddForce(new Vector3(pushVelocity.x * 5f, 8f, pushVelocity.y * 5f));

                if (target is EntityCreature)
                    (target as EntityCreature).Damage(10f);
            }

            target.damagedCooldown = 0.3f;
            behaviour.worldCamera.ShakeCamera(2f);
        }
    }
}
