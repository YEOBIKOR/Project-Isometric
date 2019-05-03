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

    public Damage(Entity behaviour, float amount = 10f)
    {
        _behaviour = behaviour;
        _amount = amount;
    }

    public void OnApplyDamage(Entity target)
    {
        if (target.damagedCooldown < 0f)
        {
            if (target is EntityCreature)
                (target as EntityCreature).Damage(_amount);

            target.damagedCooldown = 0.3f;
            _behaviour.world.cameraHUD.IndicateDamage(this, new FixedPosition(target.worldPosition));
        }
    }
}
