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

        AttachCollider(radius * 2f, height);
        AttachPhysics(radius, height);
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
        for (int index = 0; index < _entityParts.Count; index++)
            world.SpawnEntity(new DecomposeCreaturePart(this, _entityParts[index]), _entityParts[index].worldPosition);

        DespawnEntity();
    }

    public virtual void Trigger()
    {

    }

    public virtual EntityPart[][] decomposeCreatureParts
    {
        get
        {
            EntityPart[][] parts = new EntityPart[_entityParts.Count][];

            for (int index = 0; index < _entityParts.Count; index++)
                parts[index] = new EntityPart[1] { _entityParts[index] };

            return parts;
        }
    }

    public virtual string name
    {
        get
        { return ToString(); }
    }
}