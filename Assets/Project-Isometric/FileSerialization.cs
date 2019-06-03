using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class FileSerialization <T> where T : struct
{
    private string _fileName;

    public FileSerialization(string fileName)
    {
        _fileName = fileName;
    }

    public void SaveFile(T serial)
    {
        FileStream stream = new FileStream(_fileName, FileMode.Create);

        BinaryFormatter formatter = new BinaryFormatter();

        formatter.Serialize(stream, serial);

        stream.Close();
    }

    public T LoadFile()
    {
        if (!File.Exists(_fileName))
        {
            throw new FileNotFoundException();
        }

        FileStream stream = new FileStream(_fileName, FileMode.Open);

        BinaryFormatter formatter = new BinaryFormatter();

        T serial = (T)formatter.Deserialize(stream);

        stream.Close();

        return serial;
    }
}