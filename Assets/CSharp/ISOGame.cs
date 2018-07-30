using UnityEngine;
using System.Collections;
using ISO.UI;

public class ISOGame : LoopFlow
{
    private World world;

    private PauseMenu pauseMenu;

    public ISOGame() : base()
    {

    }

    public override void OnActivate()
    {
        base.OnActivate();

        world = new World(this);
        pauseMenu = new PauseMenu(this);
    }

    public override void RawUpdate(float deltaTime)
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !pauseMenu.activated)
            AddSubLoopFlow(pauseMenu);
        if (Input.GetKeyDown(KeyCode.Home))
            flowManager.RequestSwitchLoopFlow(new ISOGame());

        base.RawUpdate(deltaTime);
    }

    public override void Update(float deltaTime)
    {
        world.Update(deltaTime);

        base.Update(deltaTime);
    }

    public override void OnTerminate()
    {
        world.OnTerminate();

        base.OnTerminate();
    }
}