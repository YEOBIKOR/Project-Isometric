using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class FileSerialization <T> where T : struct
{
    private string fileName;

    public FileSerialization(string fileName)
    {
        this.fileName = fileName;
    }

    public void SaveFile(T serial)
    {
        FileStream stream = new FileStream(fileName, FileMode.Create);

        BinaryFormatter formatter = new BinaryFormatter();

        formatter.Serialize(stream, serial);

        stream.Close();
    }

    public T LoadFile()
    {
        FileStream stream = new FileStream(fileName, FileMode.Open);

        BinaryFormatter formatter = new BinaryFormatter();

        T serial = (T)formatter.Deserialize(stream);

        stream.Close();

        return serial;
    }
}
