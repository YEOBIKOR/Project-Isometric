using System.Collections.Generic;
using UnityEngine;
using Custom;
using Isometric.Interface;

public class LoopFlowManager : LoopFlow
{
    protected LoopFlow _requestedLoopFlow;
    protected LoopFlow _currentLoopFlow;

    protected float _transitTime;
    protected bool _transiting;

    public bool transiting
    {
        get
        { return _transiting; }
    }

    public LoopFlowManager() : base()
    {
        _currentLoopFlow = null;
    }

    public override void Update(float deltaTime)
    {
        _transitTime = _transitTime - deltaTime;

        if (_transitTime <= 0f)
            SwitchLoopFlow(_requestedLoopFlow);

        base.Update(deltaTime);
    }

    public virtual void SwitchLoopFlow(LoopFlow newLoopFlow)
    {
        if (newLoopFlow == null || _currentLoopFlow == newLoopFlow)
            return;

        if (_currentLoopFlow != null)
            _currentLoopFlow.Terminate();

        _currentLoopFlow = newLoopFlow;
        AddSubLoopFlow(_currentLoopFlow);

        _transiting = false;
    }

    public virtual void RequestSwitchLoopFlow(LoopFlow newLoopFlow, float fadeOutSeconds = 0.5f)
    {
        if (_transiting)
            return;

        _requestedLoopFlow = newLoopFlow;

        _transiting = true;
        _transitTime = fadeOutSeconds;
    }
}