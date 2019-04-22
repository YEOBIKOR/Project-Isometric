using System.Collections.Generic;
using UnityEngine;
using Custom;
using Isometric.Interface;

public class LoopFlowManager : LoopFlow
{
    protected LoopFlow requestedLoopFlow;
    protected LoopFlow currentLoopFlow;

    protected float transitTime;
    protected bool transiting;

    public bool Transiting
    {
        get
        { return transiting; }
    }

    public LoopFlowManager() : base()
    {
        currentLoopFlow = null;
    }

    public override void Update(float deltaTime)
    {
        transitTime = transitTime - deltaTime;

        if (transitTime <= 0f)
            SwitchLoopFlow(requestedLoopFlow);

        base.Update(deltaTime);
    }

    public virtual void SwitchLoopFlow(LoopFlow newLoopFlow)
    {
        if (newLoopFlow == null || currentLoopFlow == newLoopFlow)
            return;

        if (currentLoopFlow != null)
            currentLoopFlow.Terminate();

        currentLoopFlow = newLoopFlow;
        AddSubLoopFlow(currentLoopFlow);

        transiting = false;
    }

    public virtual void RequestSwitchLoopFlow(LoopFlow newLoopFlow, float fadeOutSeconds = 0.5f)
    {
        if (transiting)
            return;

        requestedLoopFlow = newLoopFlow;

        transiting = true;
        transitTime = fadeOutSeconds;
    }
}