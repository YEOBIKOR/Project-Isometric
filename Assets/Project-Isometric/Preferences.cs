using System;

public class Preferences : ISerializable
{
    private bool _bgmVolume;
    private bool _sfxVolume;

    public Preferences()
    {

    }

    public void Load(string filePath)
    {

    }

    public void Deserialize(byte[] bytes)
    {

    }

    public byte[] Serialize()
    {
        throw new NotImplementedException();
    }
}