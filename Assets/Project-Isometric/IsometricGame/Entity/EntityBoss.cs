using System;
using System.Collections.Generic;
using UnityEngine;
using Isometric.Interface;
using Custom;

public class EntityBoss : EntityCreature
{
    private BossInterface _bossInterface;

    private Player _player;

    private const int RuneNum = 8;
    private const int RuneStateNum = 3;

    private List<EntityBossRune>[] _runes;
    private LinkedList<PatternNode> _attackPattern;

    public EntityBoss() : base(1f, 2f, 500f)
    {
        _bossInterface = new BossInterface(this);

        _runes = new List<EntityBossRune>[RuneStateNum];
        for (int i = 0; i < RuneStateNum; i++)
            _runes[i] = new List<EntityBossRune>();

        for (int i = 0; i < RuneNum; i++)
            _runes[(int)RuneState.Hold].Add(new EntityBossRune(this, i));

        _attackPattern = new LinkedList<PatternNode>();

        _entityParts.Add(new EntityPart(this, "boss"));
        _entityParts.Add(new EntityPart(this, "bossbeard"));
        _entityParts[0].sortZOffset = 3f;
    }

    public override void OnSpawn()
    {
        base.OnSpawn();

        _player = world.player;

        foreach (var rune in _runes[(int)RuneState.Hold])
            world.SpawnEntity(rune, worldPosition + Vector3.up * 10f);

        SetAttackPattern(AllocateRunes, 3f);
        _entityParts[1].worldPosition = worldPosition;

        game.AddSubLoopFlow(_bossInterface);
    }

    public override void OnDespawn()
    {
        _bossInterface.Terminate();

        base.OnDespawn();
    }

    public override void Update(float deltaTime)
    {
        for (var iterator = _attackPattern.First; iterator != null; iterator = iterator.Next)
        {
            PatternNode pattern = iterator.Value;

            pattern.Update(deltaTime);

            if (pattern.time < 0f)
            {
                pattern.function();
                _attackPattern.Remove(pattern);
            }
        }

        Vector2 perlinValue = (new Vector2(Mathf.PerlinNoise(time * 0.2f, 0f), Mathf.PerlinNoise(0f, time * 0.2f)) * 2f - Vector2.one) * 8f;
        Vector3 targetPosition = _player.worldPosition + new Vector3(perlinValue.x, 8f + Mathf.Sin(time * 5f), perlinValue.y);

        velocity = Vector3.Lerp(velocity, targetPosition - worldPosition, deltaTime * 24f);

        int holdRuneCount = GetRunesByState(RuneState.Hold).Count;

        for (int i = 0; i < holdRuneCount; i++)
        {
            GetRunesByState(RuneState.Hold)[i]._targetPosition = worldPosition +
                CustomMath.HorizontalRotate(Vector3.right * 12f, (i * 360f / holdRuneCount) + (time * 120f));
        }

        foreach (var rune in GetRunesByState(RuneState.ReadyDrop))
        {
            rune._targetPosition = world.player.worldPosition + Vector3.up * 6f;
        }

        _entityParts[0].worldPosition = worldPosition;
        _entityParts[1].worldPosition = Vector3.Lerp(_entityParts[1].worldPosition, worldPosition, deltaTime * 10f);

        base.Update(deltaTime);
    }

    public void AllocateRunes()
    {
        int num = Mathf.CeilToInt(RuneNum / 5f);
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
        GetRunesByState(rune._state).Remove(rune);
    }

    public List<EntityBossRune> GetRunesByState(RuneState state)
    {
        return _runes[(int)state];
    }

    public void SetRuneState(EntityBossRune rune, RuneState newState)
    {
        if (rune._state != newState)
        {
            GetRunesByState(rune._state).Remove(rune);
            rune._state = newState;

            GetRunesByState(newState).Add(rune);
            rune._onControl = newState != RuneState.Drop;

            if (newState == RuneState.Drop)
            {
                rune.velocity += Vector3.down * 64f;
            }
        }
    }

    public void SetAttackPattern(Action function, float time)
    {
        _attackPattern.AddLast(new PatternNode(function, time));
    }
    
    public override string name
    {
        get
        { return "Boss - Baphomet"; }
    }

    public class EntityBossRune : EntityCreature
    {
        private EntityBoss _boss;
        private int _index;

        public RuneState _state;
        public Vector3 _targetPosition;
        public bool _onControl;

        private bool _faint;

        private Vector3 _offsetPosition;

        public EntityBossRune(EntityBoss boss, int index) : base(1f, 3f, 50f)
        {
            _boss = boss;
            _index = index;
            _state = RuneState.Hold;
            _targetPosition = Vector3.zero;
            _onControl = true;
            _offsetPosition = new Vector3(RXRandom.Range(-3f, 3f), 0f, RXRandom.Range(-3f, 3f));

            _entityParts.Add(new EntityPart(this, "bossrune"));
            _entityParts.Add(new EntityPart(this, string.Concat("bossrunemark", index + 1)));
            _entityParts[1].sortZOffset = 0.1f;
        }

        public override void OnDespawn()
        {
            _boss.OnDespawnRune(this);

            base.OnDespawn();
        }

        public override void Update(float deltaTime)
        {
            Vector3 targetPosition = _targetPosition;
            if (_state == RuneState.ReadyDrop)
                targetPosition += _offsetPosition;

            if (_onControl)
                velocity = Vector3.Lerp(velocity, targetPosition - worldPosition, deltaTime * (_state == RuneState.ReadyDrop ? 64f : 32f));

            _entityParts[0].worldPosition = worldPosition + Vector3.up * 1.2f;
            _entityParts[1].worldPosition = worldPosition + Vector3.up * 1.2f;
            
            if (_state != RuneState.Drop)
            {
                _faint = false;
            }

            else if (!_faint && _physics.landed)
            {
                _faint = true;

                world.QuakeAtPosition(worldPosition);
                worldCamera.ShakeCamera(24f);
            }

            if (!_faint)
                _entityParts[1].positionOffset = UnityEngine.Random.insideUnitCircle * 1.2f;
            _entityParts[1].color = Color.Lerp(_entityParts[1].color, _faint ? Color.clear :
                (_state == RuneState.ReadyDrop ? Color.red : Color.white), deltaTime * 5f);

            base.Update(deltaTime);
        }
        
        public override string name
        {
            get
            { return "Rune - " + _index; }
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