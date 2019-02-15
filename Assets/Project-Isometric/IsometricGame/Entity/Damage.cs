using System;
using UnityEngine;

public class Damage
{
    private Entity _behaviour;

    private float _amount;
    public float amount
    {
        get
        { return _amount; }
    }

    public Damage(Entity behaviour)
    {
        _behaviour = behaviour;
        _amount = 10f;
    }

    public void OnApplyDamage(Entity target)
    {
        if (target.damagedCooldown < 0f)
        {
            if (target is PhysicalEntity)
            {
                PhysicalEntity physicalEntity = target as PhysicalEntity;

                Vector2 pushVelocity = new Vector3(
                    target.worldPosition.x - _behaviour.worldPosition.x,
                    target.worldPosition.z - _behaviour.worldPosition.z).normalized;

                physicalEntity.AddForce(new Vector3(pushVelocity.x * 5f, 8f, pushVelocity.y * 5f));

                if (target is EntityCreature)
                    (target as EntityCreature).Damage(_amount);
            }

            target.damagedCooldown = 0.3f;
            _behaviour.worldCamera.ShakeCamera(2f);
            _behaviour.world.cameraHUD.IndicateDamage(this, target.worldPosition);
        }
    }
}
