using System;
using UnityEngine;
using Isometric.Items;

public class DroppedItem : PhysicalEntity
{
    private Item _item;
    public Item item
    {
        get
        { return _item; }
    }

    private bool acquirable;

    public DroppedItem(Item item) : base(0.25f, 0.5f)
    {
        _item = item;

        entityParts.Add(new EntityPart(this, item.element));
        entityParts[0].sortZOffset = 1f;

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
            world.player.AcquireItem(item);
            DespawnEntity();
        }
        else
            base.OnCollisionWithOther(other);
    }
}