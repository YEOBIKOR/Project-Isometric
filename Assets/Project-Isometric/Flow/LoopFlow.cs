﻿using System.Collections.Generic;
using UnityEngine;

public class LoopFlow
{
    private LoopFlow _owner;
    public LoopFlow owner
    {
        get
        { return _owner; }
    }

    public FlowManager flowManager
    {
        get
        { return this is FlowManager ? this as FlowManager : _owner.flowManager; }
    }
    
    public bool activated
    {
        get
        { return _owner != null; }
    }

    private bool _paused;
    public bool paused
    {
        get
        { return _paused; }
        set
        { _paused = value; }
    }

    private float _timeScale;
    public float timeScale
    {
        get
        { return _paused ? 0f : _timeScale; }
        set
        { _timeScale = Mathf.Max(value, 0f); }
    }

    private float _time;
    public float time
    {
        get { return _time; }
    }

    private List<LoopFlow> subLoopFlows;

    public LoopFlow()
    {
        _timeScale = 1f;
        _paused = false;

        subLoopFlows = new List<LoopFlow>();
    }

    public virtual void RawUpdate(float deltaTime)
    {
        timeScale = Mathf.Lerp(timeScale,
                Input.GetKey(KeyCode.Keypad1) ? 0f :
                Input.GetKey(KeyCode.Keypad2) ? 0.3f :
                Input.GetKey(KeyCode.Keypad3) ? 3f :
                1f, deltaTime * 10f);

        if (!paused)
            Update(deltaTime * timeScale);

        for (int index = 0; index < subLoopFlows.Count; index++)
            subLoopFlows[index].RawUpdate(deltaTime);
    }

    public virtual void Update(float deltaTime)
    {
        _time = _time + deltaTime;
    }

    public virtual void OnActivate()
    {
        _time = 0f;
    }

    public void Terminate()
    {
        if (owner != null)
            owner.RemoveSubLoopFlow(this);
    }

    public virtual void OnTerminate()
    {
        foreach (var subLoopFlow in subLoopFlows)
            subLoopFlow.OnTerminate();
    }

    public void AddSubLoopFlow(LoopFlow loopFlow)
    {
        if (loopFlow.activated)
            loopFlow.Terminate();

        subLoopFlows.Add(loopFlow);

        loopFlow._owner = this;
        loopFlow.OnActivate();

        Debug.Log(string.Concat(this, "\n> ", loopFlow));
    }

    public void RemoveSubLoopFlow(LoopFlow loopFlow)
    {
        if (subLoopFlows.Remove(loopFlow))
        {
            loopFlow._owner = null;
            loopFlow.OnTerminate();

            Debug.Log(string.Concat(this, "\nX ", loopFlow));
        }
    }
}