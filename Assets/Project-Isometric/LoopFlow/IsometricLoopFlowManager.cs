using System.Collections.Generic;
using UnityEngine;
using Custom;
using Isometric.Interface;

public class IsometricLoopFlowManager : LoopFlowManager
{
    private FSprite fadeSprite;
    private FLabel fadeLabel;

    private float transitFactor;

    public IsometricLoopFlowManager() : base()
    {
        fadeSprite = new FSprite("pixel");
        fadeSprite.scaleX = MenuFlow.screenWidth;
        fadeSprite.scaleY = MenuFlow.screenHeight;
        fadeSprite.color = Color.black;

        fadeLabel = new FLabel("font", "Loading...");
        fadeLabel.alignment = FLabelAlignment.Right;
        fadeLabel.SetPosition(MenuFlow.rightDown + new Vector2(-10f, 10f));
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

        fadeSprite.alpha = CustomMath.Curve(1f - transitFactor, 1f);

        base.Update(deltaTime);
    }

    public override void RequestSwitchLoopFlow(LoopFlow newLoopFlow, float fadeOutSeconds = 0.5f)
    {
        base.RequestSwitchLoopFlow(newLoopFlow, fadeOutSeconds);

        Futile.stage.AddChild(fadeSprite);
        Futile.stage.AddChild(fadeLabel);
    }

    public override void SwitchLoopFlow(LoopFlow newLoopFlow)
    {
        base.SwitchLoopFlow(newLoopFlow);

        Futile.stage.AddChild(fadeSprite);
        Futile.stage.RemoveChild(fadeLabel);
    }
}