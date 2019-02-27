using UnityEngine;
using System.Collections;
using Custom;
using Isometric.Items;

public abstract class EntityCreature : Entity, ITarget
{
    private float _viewAngle;
    public float viewAngle
    {
        get { return _viewAngle; }
        set { _viewAngle = CustomMath.ToAngle(value); }
    }

    private float _moveSpeed;
    public float moveSpeed
    {
        get
        { return _moveSpeed; }
        set
        { _moveSpeed = value; }
    }

    private float _health;
    public float health
    {
        get { return _health; }
        set
        {
            if (value <= 0f)
                KillCreature();

            value = Mathf.Min(value, maxHealth);
            _health = value;
        }
    }

    private float _maxHealth;
    public float maxHealth
    {
        get { return _maxHealth; }
    }

    public virtual Rect boundRect
    {
        get
        { return new Rect(0f, 12f, 24f, 36f); }
    }

    public EntityCreature(float radius, float height, float maxHealth) : base(radius * 2f)
    {
        _viewAngle = 0f;
        _moveSpeed = 3f;
        _health = maxHealth;
        _maxHealth = maxHealth;

        _physics = new EntityPhysics(radius, height);
    }

    public void MoveTo(Vector2 direction, float force)
    {
        if (new Vector2(velocity.x, velocity.z).sqrMagnitude < moveSpeed * moveSpeed)
            _physics.AddForce(new Vector3(direction.x, 0f, direction.y).normalized * force);
    }

    public void Damage(float value)
    {
        health = health - value;
    }

    public void KillCreature()
    {
        for (int index = 0; index < entityParts.Count; index++)
            world.SpawnEntity(new DecomposeCreaturePart(this, entityParts[index]), entityParts[index].worldPosition);
        
        for (int i = 0; i < 10; i++)
            world.SpawnEntity(new DroppedItem(new ItemStack(Item.GetItemByKey("pickaxe"), 1)), worldPosition + new Vector3(Random.Range(-0.5f, 0.5f), 0f, Random.Range(-0.5f, 0.5f)));

        DespawnEntity();
    }

    public virtual void Trigger()
    {

    }

    public virtual EntityPart[][] decomposeCreatureParts
    {
        get
        {
            EntityPart[][] parts = new EntityPart[entityParts.Count][];

            for (int index = 0; index < entityParts.Count; index++)
                parts[index] = new EntityPart[1] { entityParts[index] };

            return parts;
        }
    }

    public override string debugString
    {
        get
        {
            return string.Concat(
                base.debugString,
                "viewAngle : ", viewAngle, "\n",
                "health : ", health, "\n",
                "damageCool : ", damagedCooldown, "\n"
                );
        }
    }
}