using System;

public interface ISerializable <T> where T : struct
{
    T Serialize();
    void Deserialize(T data);
}
