using System.Collections.Generic;

public class Registry <T>
{
    private List<T> _list;
    private Dictionary<string, T> _dictionary;

    public Registry()
    {
        _list = new List<T>();
        _dictionary = new Dictionary<string, T>();
    }

    public void Add(string key, T item)
    {
        _list.Add(item);
        _dictionary.Add(key, item);
    }

    public T[] GetAll()
    {
        return _list.ToArray();
    }

    public T this [int id]
    {
        get
        { return _list[id]; }
    }

    public T this [string key]
    {
        get
        { return _dictionary[key]; }
    }

    public int GetID(T item)
    {
        return _list.IndexOf(item);
    }

    public int Count
    {
        get
        { return _list.Count; }
    }
}
