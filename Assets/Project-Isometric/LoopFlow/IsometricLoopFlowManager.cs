using System.Collections.Generic;
using UnityEngine;
using Custom;
using Isometric.Interface;

public class IsometricLoopFlowManager : LoopFlowManager
{
    private FSprite _fadeSprite;
    private FLabel _fadeLabel;

    private float _transitFactor;

    public IsometricLoopFlowManager() : base()
    {
        _fadeSprite = new FSprite("pixel");
        _fadeSprite.scaleX = MenuFlow.screenWidth;
        _fadeSprite.scaleY = MenuFlow.screenHeight;
        _fadeSprite.color = Color.black;

        _fadeLabel = new FLabel("font", "Loading...");
        _fadeLabel.alignment = FLabelAlignment.Right;
        _fadeLabel.SetPosition(MenuFlow.rightDown + new Vector2(-10f, 10f));
    }

    public override void RawUpdate(float deltaTime)
    {
        base.RawUpdate(Mathf.Min(deltaTime, 0.05f));

        if (Input.GetKeyDown(KeyCode.Escape))
            HandleExecuteEscape();
    }

    public override void Update(float deltaTime)
    {
        _transitFactor = Mathf.Clamp01(_transitFactor + (_transiting ? -deltaTime : deltaTime) / 0.5f);

        _fadeSprite.alpha = CustomMath.Curve(1f - _transitFactor, 1f);

        base.Update(deltaTime);
    }

    public override void RequestSwitchLoopFlow(LoopFlow newLoopFlow, float fadeOutSeconds = 0.5f)
    {
        base.RequestSwitchLoopFlow(newLoopFlow, fadeOutSeconds);

        Futile.stage.AddChild(_fadeSprite);
        Futile.stage.AddChild(_fadeLabel);
    }

    public override void SwitchLoopFlow(LoopFlow newLoopFlow)
    {
        base.SwitchLoopFlow(newLoopFlow);

        Futile.stage.AddChild(_fadeSprite);
        Futile.stage.RemoveChild(_fadeLabel);
    }
}