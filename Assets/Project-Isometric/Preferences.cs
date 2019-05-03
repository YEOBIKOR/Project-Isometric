using System;

public class Preferences : ISerializable <SerializedPreferences>
{
    private bool _bgmVolume;
    private bool _sfxVolume;

    public Preferences()
    {

    }

    public void Load(string filePath)
    {

    }

    public SerializedPreferences Serialize()
    {
        throw new NotImplementedException();
    }

    public void Deserialize(SerializedPreferences data)
    {
        throw new NotImplementedException();
    }
}

[Serializable]
public struct SerializedPreferences
{

}