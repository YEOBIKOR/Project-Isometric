using System.Collections.Generic;
using UnityEngine;
using Custom;
using Isometric.Interface;

public class FlowManager : LoopFlow
{
    private LoopFlow currentLoopFlow;
    private LoopFlow requestedLoopFlow;

    private FSprite fadeSprite;

    private LinkedList<PopupMenu> popupMenus;

    private bool transiting;
    private float transitFactor;

    private bool _popup;
    public bool popup
    {
        get
        { return _popup; }
    }

    public FlowManager(IsometricMain main) : base()
    {
        fadeSprite = new FSprite("pixel");
        fadeSprite.scaleX = Menu.screenWidth;
        fadeSprite.scaleY = Menu.screenHeight;
        fadeSprite.color = Color.black;

        popupMenus = new LinkedList<PopupMenu>();
        _popup = false;
    }

    public override void RawUpdate(float deltaTime)
    {
        if (_popup && !(popupMenus.Count > 0))
            _popup = false;
        else if (!_popup && (popupMenus.Count > 0))
            _popup = true;

        if (Input.GetKeyDown(KeyCode.Escape))
            ClosePopup();

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

    public void AddPopup(PopupMenu popupMenu)
    {
        popupMenus.AddLast(popupMenu);
    }

    public void RemovePopup(PopupMenu popupMenu)
    {
        popupMenus.Remove(popupMenu);
    }

    public void ClosePopup()
    {
        if (popupMenus.Count > 0)
        {
            PopupMenu close = popupMenus.Last.Value;

            close.RequestTerminate();

            popupMenus.RemoveLast();
        }
    }
}