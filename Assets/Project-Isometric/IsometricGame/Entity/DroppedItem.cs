using System;
using UnityEngine;
using Isometric.Items;

public class DroppedItem : Entity
{
    private ItemStack _itemStack;
    public ItemStack itemStack
    {
        get
        { return _itemStack; }
    }

    private bool _acquirable;
    private bool _acquired;

    private float _acquiretime;
    private Vector3 _acquirePosition;

    public DroppedItem(ItemStack itemStack) : base(0.5f)
    {
        _itemStack = itemStack;

        _entityParts.Add(new EntityPart(this, itemStack.item.element));
        _entityParts[0].sortZOffset = 1f;
        _entityParts[0].scale = Vector2.one * 0.5f;
        _entityParts[0].shader = IsometricMain.GetShader("DroppedItem");

        _acquirable = false;
        _acquired = false;

        AttachPhysics(0.25f, 0.5f);
    }

    public override void Update(float deltaTime)
    {
        if (_acquired)
            AcquireUpdate(time - _acquiretime);
        else
        {
            if (time > 60f)
                DespawnEntity();

            _acquirable = time > 2f;

            if (_acquirable)
            {
                Vector3 deltaVec = world.player.worldPosition - worldPosition;
                if (deltaVec.sqrMagnitude < 4f)
                {
                    _acquired = true;
                    _acquiretime = time;
                    _acquirePosition = worldPosition;
                }
            }
        }

        _entityParts[0].worldPosition = worldPosition + Vector3.up * (Mathf.Sin(time * Mathf.PI) + 2f) * 0.3f;

        base.Update(deltaTime);
    }
    
    public void AcquireUpdate(float time)
    {
        float factor = time * 2f;

        worldPosition = Vector3.Lerp(_acquirePosition, world.player.worldPosition, factor) + Vector3.up * Mathf.Sin(factor * Mathf.PI) * 2f;

        if (factor > 1f)
        {
            world.player.AcquireItem(itemStack);
            DespawnEntity();
        }
    }
}