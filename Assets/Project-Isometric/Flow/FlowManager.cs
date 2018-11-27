using System.Collections.Generic;
using UnityEngine;
using Custom;
using Isometric.UI;

public class FlowManager : LoopFlow
{
    private LoopFlow currentLoopFlow;
    private LoopFlow requestedLoopFlow;

    private FSprite fadeSprite;

    private bool transiting;
    private float transitFactor;

    public FlowManager(ISOMain main) : base()
    {
        fadeSprite = new FSprite("pixel");
        fadeSprite.scaleX = Menu.screenWidth;
        fadeSprite.scaleY = Menu.screenHeight;
        fadeSprite.color = Color.black;
    }

    public override void RawUpdate(float deltaTime)
    {
        base.RawUpdate(Mathf.Min(deltaTime, 0.05f));
    } 

    public override void Update(float deltaTime)
    {
        transitFactor = Mathf.Clamp01(transitFactor + (transiting ? -deltaTime : deltaTime) / 0.5f);
        if (!(transitFactor > 0f) && currentLoopFlow != requestedLoopFlow)
            SwitchLoopFlow(requestedLoopFlow);

        fadeSprite.alpha = CustomMath.Curve(1f - transitFactor, 1f);

        base.Update(deltaTime);
    }

    public void RequestSwitchLoopFlow(LoopFlow newLoopFlow, float fadeOutSeconds = 0.5f)
    {
        if (!transiting)
        {
            requestedLoopFlow = newLoopFlow;

            transiting = true;
            transitFactor = 1f;

            Futile.stage.AddChild(fadeSprite);
        }
    }

    public void SwitchLoopFlow(LoopFlow newLoopFlow)
    {
        if (currentLoopFlow != null)
            currentLoopFlow.Terminate();

        currentLoopFlow = newLoopFlow;
        AddSubLoopFlow(currentLoopFlow);

        transiting = false;
        transitFactor = 0f;

        Futile.stage.AddChild(fadeSprite);
    }
}