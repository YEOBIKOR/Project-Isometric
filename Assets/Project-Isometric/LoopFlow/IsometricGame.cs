using UnityEngine;
using Isometric.Interface;

using System.IO;

public class IsometricGame : LoopFlow
{
    private World _world;

    private PauseMenu _pauseMenu;

    private FileSerialization<World.Serialized> _worldFile;

    private string _worldFileToLoad;

    public IsometricGame(string worldFileToLoad)
    {
        _worldFileToLoad = worldFileToLoad;
    }

    public override void OnActivate()
    {
        base.OnActivate();

        _world = new World(this, "World_0");

        _pauseMenu = new PauseMenu(this);

        _worldFile = new FileSerialization<World.Serialized>(_worldFileToLoad);

        try
        {
            _world.Deserialize(_worldFile.LoadFile());
        }
        catch (FileNotFoundException)
        {
            Debug.Log("The save file cannot be found, create a new save file.");

            _world.RequestLoadChunk(Vector2Int.zero);
        }
    }

    public override void Update(float deltaTime)
    {
        _world.Update(deltaTime);

        base.Update(deltaTime);
    }

    public override void OnTerminate()
    {
        _worldFile.SaveFile(_world.Serialize());

        _world.OnTerminate();

        base.OnTerminate();
    }

    public override bool OnExecuteEscape()
    {
        AddSubLoopFlow(_pauseMenu);

        return false;
    }
}