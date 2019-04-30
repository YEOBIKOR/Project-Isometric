using System;
using System.Collections.Generic;
using System.Threading;

public class Task <T> where T : class
{
    private Action<T> _callback;

    private Queue<T> _input;
    private Queue<T> _output;

    public Task(Action<T> callback)
    {
        _callback = callback;

        _input = new Queue<T>();
        _output = new Queue<T>();
    }

    public void AddTask(T item)
    {
        bool running;

        lock (_input)
        {
            running = _input.Count > 0;
            _input.Enqueue(item);
        }

        if (!running)
        {
            Thread loadThread = new Thread(TaskThread);
            loadThread.Start();
        }
    }

    private void TaskThread()
    {
        bool remain;

        do
        {
            T item;

            lock (_input)
            {
                item = _input.Dequeue();
                remain = _input.Count > 0;
            }
            
            _callback(item);

            lock (_output)
            { _output.Enqueue(item); }

        } while (remain);
    }

    public T[] GetCompletedTasks()
    {
        T[] items;

        lock (_output)
        {
            items = _output.ToArray();
            _output.Clear();
        }

        return items;
    }
}
