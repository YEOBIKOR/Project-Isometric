using System.Collections.Generic;
using UnityEngine;
using Custom;
using Isometric.Interface;

public class FlowManager : LoopFlow
{
    private LoopFlow currentLoopFlow;
    private LoopFlow requestedLoopFlow;

    private FSprite fadeSprite;
    private FLabel fadeLabel;

    private bool transiting;
    private float transitTime;
    private float transitFactor;

    public FlowManager(IsometricMain main) : base()
    {
        fadeSprite = new FSprite("pixel");
        fadeSprite.scaleX = Menu.screenWidth;
        fadeSprite.scaleY = Menu.screenHeight;
        fadeSprite.color = Color.black;

        fadeLabel = new FLabel("font", "Loading...");
        fadeLabel.alignment = FLabelAlignment.Right;
        fadeLabel.SetPosition(Menu.rightDown + new Vector2(-10f, 10f));
    }

    public override void RawUpdate(float deltaTime)
    {
        base.RawUpdate(Mathf.Min(deltaTime, 0.05f));
        
        if (Input.GetKeyDown(KeyCode.Escape))
            HandleExecuteEscape();
    } 

    public override void Update(float deltaTime)
    {
        transitFactor = Mathf.Clamp01(transitFactor + (transiting ? -deltaTime : deltaTime) / 0.5f);
        if (time - transitTime > 1f && currentLoopFlow != requestedLoopFlow)
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
            transitTime = time;

            Futile.stage.AddChild(fadeSprite);
            Futile.stage.AddChild(fadeLabel);
        }
    }

    public void SwitchLoopFlow(LoopFlow newLoopFlow)
    {
        if (currentLoopFlow != null)
            currentLoopFlow.Terminate();

        requestedLoopFlow = newLoopFlow;

        currentLoopFlow = newLoopFlow;
        AddSubLoopFlow(currentLoopFlow);

        transiting = false;
        transitFactor = 0f;

        Futile.stage.AddChild(fadeSprite);
        Futile.stage.RemoveChild(fadeLabel);
    }

    //public override bool HandleExecuteEscape()
    //{
    //    if (currentLoopFlow.HandleExecuteEscape())
    //    {
    //        return true;
    //    }

    //    return base.HandleExecuteEscape();
    //}
}