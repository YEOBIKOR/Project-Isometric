using System;
using UnityEngine;
using Isometric.Items;

public class DroppedItem : PhysicalEntity
{
    private ItemStack _itemStack;
    public ItemStack itemStack
    {
        get
        { return _itemStack; }
    }

    private bool acquirable;

    public DroppedItem(ItemStack itemStack) : base(0.25f, 0.5f)
    {
        _itemStack = itemStack;

        entityParts.Add(new EntityPart(this, itemStack.item.element));
        entityParts[0].sortZOffset = 1f;
        entityParts[0].scale = Vector2.one * 0.5f;

        acquirable = false;
    }

    public override void Update(float deltaTime)
    {
        if (time > 60f)
            DespawnEntity();

        acquirable = time > 2f;

        if (acquirable)
        {
            Vector3 deltaVec = world.player.worldPosition - worldPosition;
            if (deltaVec.sqrMagnitude < 4f)
                AddForce(deltaVec.normalized * 100f / deltaVec.sqrMagnitude * deltaTime);
        }

        entityParts[0].worldPosition = worldPosition + Vector3.up * (Mathf.Sin(time * Mathf.PI) + 2f) * 0.3f;

        base.Update(deltaTime);
    }

    public override void OnCollisionWithOther(PhysicalEntity other)
    {
        if (other == world.player && acquirable)
        {
            world.player.AcquireItem(itemStack);
            DespawnEntity();
        }
        else
            base.OnCollisionWithOther(other);
    }
}