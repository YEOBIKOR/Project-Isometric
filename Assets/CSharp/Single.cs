using UnityEngine;

public class Single <T> where T : class
{
    private static T m_instance = null;
    public static T Instance { get { return m_instance; } }

    protected Single()
    {
        m_instance = this as T;
    }
}
