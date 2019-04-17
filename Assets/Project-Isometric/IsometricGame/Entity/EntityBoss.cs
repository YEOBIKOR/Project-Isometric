using System;
using System.Collections.Generic;
using UnityEngine;
using Custom;

public class EntityBoss : EntityCreature
{
    private const int runeNum = 8;
    private const int runeStateNum = 3;

    private List<EntityBossRune>[] runes;
    private LinkedList<PatternNode> attackPattern;

    public EntityBoss() : base(1f, 2f, 500f)
    {
        runes = new List<EntityBossRune>[runeStateNum];
        for (int i = 0; i < runeStateNum; i++)
            runes[i] = new List<EntityBossRune>();

        for (int i = 0; i < runeNum; i++)
            runes[(int)RuneState.Hold].Add(new EntityBossRune(this, i));

        attackPattern = new LinkedList<PatternNode>();

        entityParts.Add(new EntityPart(this, "boss"));
        entityParts.Add(new EntityPart(this, "bossbeard"));
        entityParts[0].sortZOffset = 3f;
    }

    public override void OnSpawn(Chunk chunk, Vector3 position)
    {
        base.OnSpawn(chunk, position);

        foreach (var rune in runes[(int)RuneState.Hold])
            world.SpawnEntity(rune, position + Vector3.up * 10f);

        SetAttackPattern(AllocateRunes, 3f);
        entityParts[1].worldPosition = worldPosition;
    }

    public override void Update(float deltaTime)
    {
        for (var iterator = attackPattern.First; iterator != null; iterator = iterator.Next)
        {
            PatternNode pattern = iterator.Value;

            pattern.Update(deltaTime);

            if (pattern.time < 0f)
            {
                pattern.function();
                attackPattern.Remove(pattern);
            }
        }

        velocity = Vector3.Lerp(velocity, world.player.worldPosition + new Vector3(4f, 8f + Mathf.Sin(time * 5f), 4f) - worldPosition, deltaTime * 24f);

        int holdRuneCount = GetRunesByState(RuneState.Hold).Count;

        for (int i = 0; i < holdRuneCount; i++)
        {
            GetRunesByState(RuneState.Hold)[i].targetPosition = worldPosition +
                CustomMath.HorizontalRotate(Vector3.right * 12f, (i * 360f / holdRuneCount) + (time * 120f));
        }

        foreach (var rune in GetRunesByState(RuneState.ReadyDrop))
        {
            rune.targetPosition = world.player.worldPosition + Vector3.up * 6f;
        }

        entityParts[0].worldPosition = worldPosition;
        entityParts[1].worldPosition = Vector3.Lerp(entityParts[1].worldPosition, worldPosition, deltaTime * 10f);

        base.Update(deltaTime);
    }

    public void AllocateRunes()
    {
        int num = Mathf.CeilToInt(runeNum / 5f);
        List<EntityBossRune> runes = GetRunesByState(RuneState.Hold);

        for (int i = 0; i < num && runes.Count > 0; i++)
            SetRuneState(runes[RXRandom.Range(0, runes.Count)], RuneState.ReadyDrop);

        SetAttackPattern(DropRunes, 5f);
    }

    public void DropRunes()
    {
        List<EntityBossRune> runes = GetRunesByState(RuneState.ReadyDrop);

        while (runes.Count > 0)
            SetRuneState(runes[0], RuneState.Drop);

        SetAttackPattern(CollectRunes, 10f);
    }

    public void CollectRunes()
    {
        List<EntityBossRune> runes = GetRunesByState(RuneState.Drop);

        while (runes.Count > 0)
            SetRuneState(runes[0], RuneState.Hold);

        SetAttackPattern(AllocateRunes, 10f);
    }

    public void OnDespawnRune(EntityBossRune rune)
    {
        GetRunesByState(rune.state).Remove(rune);
    }

    public List<EntityBossRune> GetRunesByState(RuneState state)
    {
        return runes[(int)state];
    }

    public void SetRuneState(EntityBossRune rune, RuneState newState)
    {
        if (rune.state != newState)
        {
            GetRunesByState(rune.state).Remove(rune);
            rune.state = newState;

            GetRunesByState(newState).Add(rune);
            rune.onControl = newState != RuneState.Drop;

            if (newState == RuneState.Drop)
            {
                rune.velocity += Vector3.down * 64f;
            }
        }
    }

    public void SetAttackPattern(Action function, float time)
    {
        attackPattern.AddLast(new PatternNode(function, time));
    }

    public class EntityBossRune : EntityCreature
    {
        private EntityBoss boss;
        private int index;

        public RuneState state;
        public Vector3 targetPosition;
        public bool onControl;

        private bool faint;

        private Vector3 offsetPosition;

        public EntityBossRune(EntityBoss boss, int index) : base(1f, 3f, 50f)
        {
            this.boss = boss;
            this.index = index;
            this.state = RuneState.Hold;
            this.targetPosition = Vector3.zero;
            this.onControl = true;
            this.offsetPosition = new Vector3(RXRandom.Range(-3f, 3f), 0f, RXRandom.Range(-3f, 3f));

            entityParts.Add(new EntityPart(this, "bossrune"));
            entityParts.Add(new EntityPart(this, string.Concat("bossrunemark", index + 1)));
            entityParts[1].sortZOffset = 0.1f;
        }

        public override void OnDespawn()
        {
            boss.OnDespawnRune(this);

            base.OnDespawn();
        }

        public override void Update(float deltaTime)
        {
            Vector3 targetPosition = this.targetPosition;
            if (state == RuneState.ReadyDrop)
                targetPosition += offsetPosition;

            if (onControl)
                velocity = Vector3.Lerp(velocity, targetPosition - worldPosition, deltaTime * (state == RuneState.ReadyDrop ? 64f : 32f));

            entityParts[0].worldPosition = worldPosition + Vector3.up * 1.2f;
            entityParts[1].worldPosition = worldPosition + Vector3.up * 1.2f;
            
            if (state != RuneState.Drop)
            {
                faint = false;
            }

            else if (!faint && _physics.landed)
            {
                faint = true;

                world.QuakeAtPosition(worldPosition);
                worldCamera.ShakeCamera(24f);
            }

            if (!faint)
                entityParts[1].positionOffset = UnityEngine.Random.insideUnitCircle * 1.2f;
            entityParts[1].color = Color.Lerp(entityParts[1].color, faint ? Color.clear :
                (state == RuneState.ReadyDrop ? Color.red : Color.white), deltaTime * 5f);

            base.Update(deltaTime);
        }
    }

    public enum RuneState
    {
        Hold,
        ReadyDrop,
        Drop
    }

    public class PatternNode
    {
        public Action function;
        public float time;

        public PatternNode (Action function, float time)
        {
            this.function = function;
            this.time = time;
        }

        public void Update(float deltaTime)
        {
            time -= deltaTime;
        }
    }
} 