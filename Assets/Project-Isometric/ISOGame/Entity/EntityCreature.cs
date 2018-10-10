using UnityEngine;
using System.Collections;
using Custom;
using Isometric.Items;

public abstract class EntityCreature : PhysicalEntity
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
            healthBar.SetHealthValue(value);
        }
    }

    private float _maxHealth;
    public float maxHealth
    {
        get { return _maxHealth; }
    }

    private HealthBar healthBar;

    public EntityCreature(float radius, float height, float maxHealth) : base(radius, height)
    {
        _viewAngle = 0f;
        _moveSpeed = 3f;
        _health = maxHealth;
        _maxHealth = maxHealth;

        healthBar = new HealthBar(this);
    }

    public override void OnDespawn()
    {
        healthBar.Hide();

        base.OnDespawn();
    }

    public override void Update(float deltaTime)
    {
        healthBar.Update(deltaTime);

        base.Update(deltaTime);
    }

    public void MoveTo(Vector2 direction, float force)
    {
        if (new Vector2(velocity.x, velocity.z).sqrMagnitude < moveSpeed * moveSpeed)
            AddForce(new Vector3(direction.x, 0f, direction.y).normalized * force);
    }

    public void Damage(float value)
    {
        health = health - value;

        healthBar.Show();
    }

    public void KillCreature()
    {
        for (int index = 0; index < entityParts.Count; index++)
            world.SpawnEntity(new DecomposeCreaturePart(this, entityParts[index]), entityParts[index].worldPosition);
        
        //for (int i = 0; i < 10; i++)
        //    world.SpawnEntity(new DroppedItem(new ItemCoin()), worldPosition + new Vector3(Random.Range(-0.5f, 0.5f), 0f, Random.Range(-0.5f, 0.5f)));

        DespawnEntity();
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

    public class HealthBar
    {
        private EntityCreature owner;

        private EntityPart backGround;
        private EntityPart foreGround;

        public HealthBar(EntityCreature owner)
        {
            this.owner = owner;

            FAtlasElement element = Futile.atlasManager.GetElementWithName("pixel");

            backGround = new EntityPart(owner, element);
            foreGround = new EntityPart(owner, element);

            backGround.scale = new Vector2(24f, 6f);
            backGround.color = Color.black;

            SetHealthValue(owner.health);
        }

        public void Show()
        {
            owner.world.AddCosmeticDrawble(backGround);
            owner.world.AddCosmeticDrawble(foreGround);
        }

        public void Hide()
        {
            backGround.Erase();
            foreGround.Erase();
        }

        public void SetHealthValue(float value)
        {
            foreGround.positionOffset = new Vector2(Mathf.Lerp(-11f, 0f, owner.health / owner.maxHealth), 0f);
            foreGround.scale = new Vector2(22f * owner.health / owner.maxHealth, 4f);
            foreGround.color = Color.Lerp(Color.red, new Color32(0xAC, 0xEF, 0x2A, 0xFF), owner.health / owner.maxHealth);
        }

        public void Update(float deltaTime)
        {
            backGround.worldPosition = owner.worldPosition + Vector3.up * 3f;
            foreGround.worldPosition = owner.worldPosition + Vector3.up * 3f;
        }
    }
}