using System.Collections.Generic;
using UnityEngine;

public abstract class LoopFlow
{
    private LoopFlow _owner;
    public LoopFlow owner
    {
        get
        { return _owner; }
    }

    public LoopFlowManager loopFlowManager
    {
        get
        { return this is LoopFlowManager ? this as LoopFlowManager : _owner.loopFlowManager; }
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

    private List<LoopFlow> _subLoopFlows;

    public LoopFlow()
    {
        _timeScale = 1f;
        _paused = false;

        _subLoopFlows = new List<LoopFlow>();
    }

    public virtual void RawUpdate(float deltaTime)
    {
        if (!paused)
            Update(deltaTime * timeScale);

        for (int index = 0; index < _subLoopFlows.Count; index++)
            _subLoopFlows[index].RawUpdate(deltaTime);
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
        foreach (var subLoopFlow in _subLoopFlows)
            subLoopFlow.OnTerminate();
    }

    public void AddSubLoopFlow(LoopFlow loopFlow)
    {
        if (loopFlow.activated)
            loopFlow.Terminate();

        _subLoopFlows.Add(loopFlow);

        loopFlow._owner = this;
        loopFlow.OnActivate();

        Debug.Log(string.Concat(this, "\n> ", loopFlow));
    }

    public void RemoveSubLoopFlow(LoopFlow loopFlow)
    {
        if (_subLoopFlows.Remove(loopFlow))
        {
            loopFlow.OnTerminate();
            loopFlow._owner = null;

            Debug.Log(string.Concat(this, "\nX ", loopFlow));
        }
    }

    public virtual bool HandleExecuteEscape()
    {
        for (int index = _subLoopFlows.Count - 1; !(index < 0); index--)
        {
            if (_subLoopFlows[index].HandleExecuteEscape())
            {
                return true;
            }
        }

        return OnExecuteEscape();
    }

    public virtual bool OnExecuteEscape()
    {
        return false;
    }
}