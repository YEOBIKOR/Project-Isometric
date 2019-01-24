using System.Collections.Generic;

public class Registry <T>
{
    private List<T> list;
    private Dictionary<string, T> dictionary;

    public Registry()
    {
        list = new List<T>();
        dictionary = new Dictionary<string, T>();
    }

    public void Add(string key, T item)
    {
        list.Add(item);
        dictionary.Add(key, item);
    }

    public T[] GetAll()
    {
        return list.ToArray();
    }

    public T this [int id]
    {
        get
        { return list[id]; }
    }

    public T this [string key]
    {
        get
        { return dictionary[key]; }
    }

    public int Count
    {
        get
        { return list.Count; }
    }
}
