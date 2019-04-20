using UnityEngine;
using Isometric.Interface;

using System.IO;

public class IsometricGame : LoopFlow
{
    private World world;

    private PauseMenu pauseMenu;

    private FileSerialization<World.Serialized> _worldFile;

    public override void OnActivate()
    {
        base.OnActivate();

        world = new World(this, "World_0");

        pauseMenu = new PauseMenu(this);

        _worldFile = new FileSerialization<World.Serialized>("SaveData/" + world.worldName + ".dat");

        try
        {
            world.Deserialize(_worldFile.LoadFile());
        }
        catch (FileNotFoundException)
        {
            Debug.Log("The save file cannot be found, create a new save file.");

            world.RequestLoadChunk(Vector2Int.zero);
        }
    }

    public override void Update(float deltaTime)
    {
        world.Update(deltaTime);

        base.Update(deltaTime);
    }

    public override void OnTerminate()
    {
        _worldFile.SaveFile(world.Serialize());

        world.OnTerminate();

        base.OnTerminate();
    }

    public override bool OnExecuteEscape()
    {
        AddSubLoopFlow(pauseMenu);

        return false;
    }
}