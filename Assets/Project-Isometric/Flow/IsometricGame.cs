using UnityEngine;
using System.Collections;
using Isometric.Interface;

public class IsometricGame : LoopFlow
{
    private World world;

    private PauseMenu pauseMenu;

    public override void OnActivate()
    {
        base.OnActivate();

        world = new World(this);
        pauseMenu = new PauseMenu(this);
    }

    public override void RawUpdate(float deltaTime)
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !pauseMenu.activated && !flowManager.popup)
            AddSubLoopFlow(pauseMenu);
        if (Input.GetKeyDown(KeyCode.Home))
            flowManager.RequestSwitchLoopFlow(new IsometricGame());

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