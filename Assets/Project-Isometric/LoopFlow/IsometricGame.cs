using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using UnityEngine;
using Isometric.Interface;

public class IsometricGame : LoopFlow
{
    private World world;

    private PauseMenu pauseMenu;

    public override void OnActivate()
    {
        base.OnActivate();

        world = new World(this, "World_0");
        pauseMenu = new PauseMenu(this);

        try
        {
            LoadWorldData();
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
        SaveWorldData();

        world.OnTerminate();

        base.OnTerminate();
    }

    public override bool OnExecuteEscape()
    {
        AddSubLoopFlow(pauseMenu);

        return false;
    }

    public void SaveWorldData()
    {
        FileStream stream = new FileStream("SaveData/" + world.worldName + ".dat", FileMode.Create);

        BinaryFormatter formatter = new BinaryFormatter();

        formatter.Serialize(stream, world.Serialize());

        stream.Close();
    }

    public void LoadWorldData()
    {
        FileStream stream = new FileStream("SaveData/" + world.worldName + ".dat", FileMode.Open);

        BinaryFormatter formatter = new BinaryFormatter();

        World.Serialized serial = (World.Serialized)formatter.Deserialize(stream);

        world.Deserialize(serial);

        stream.Close();
    }
}