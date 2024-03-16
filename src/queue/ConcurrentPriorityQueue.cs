using System.Collections.Concurrent;

namespace ConcurrentDataStructure;

public class ConcurrentPriorityQueue<T>
{
    private readonly object _lock = new();
    private readonly int _nrOfPriorities;
    private ConcurrentDictionary<int, ConcurrentLinkedList<T>> _queueMap;

    /// <summary>
    ///     Creates a concurrent FIFO priority queue.
    /// </summary>
    /// <param name="nrOfPriorities">Number of available priorities, higher priority = lower number and vice versa (min 3, max 99)</param>
    public ConcurrentPriorityQueue(int nrOfPriorities = 3)
    {
        if (nrOfPriorities < 3)
            nrOfPriorities = 3;
        else if (nrOfPriorities > 99)
            nrOfPriorities = 99;

        _queueMap = new ConcurrentDictionary<int, ConcurrentLinkedList<T>>();
        _nrOfPriorities = nrOfPriorities;

        Init();
    }

    /// <summary>
    ///     Number of priorities.
    /// </summary>
    public int NrOfPriorities
    {
        get
        {
            lock (_lock)
            {
                return _nrOfPriorities;
            }
        }
    }

    /// <summary>
    ///     Number of items in the queue.
    /// </summary>
    public int Count
    {
        get
        {
            lock (_lock)
            {
                var count = 0;

                for (var i = 1; i <= _nrOfPriorities; i++)
                {
                    count += _queueMap[i].Count;
                }

                return count;
            }
        }
    }

    /// <summary>
    ///     Enqueue item.
    /// </summary>
    /// <param name="item">Item</param>
    /// <param name="priority">Priority (default 2)</param>
    public void Enqueue(T item, int priority = 2)
    {
        lock (_lock)
        {
            AdjustPriority(ref priority);
            _queueMap[priority].AddLast(item);
        }
    }

    /// <summary>
    ///     Dequeue an item.
    /// </summary>
    /// <returns>The item (null if empty)</returns>
    public T? Dequeue()
    {
        lock (_lock)
        {
            for (var i = 1; i <= _nrOfPriorities; i++)
            {
                if (_queueMap[i].Count != 0)
                {
                    var node = _queueMap[i].First;
                    _queueMap[i].RemoveFirst();

                    return node.Value;
                }
            }

            return default;
        }
    }

    /// <summary>
    ///     Init priority queue.
    /// </summary>
    private void Init()
    {
        for (var i = 1; i <= _nrOfPriorities; i++)
        {
            _queueMap.TryAdd(i, new ConcurrentLinkedList<T>());
        }
    }

    /// <summary>
    ///     Adjust priority.
    /// </summary>
    /// <param name="prio"></param>
    private void AdjustPriority(ref int prio)
    {
        if (prio < 1)
            prio = 1;
        else if (prio > _nrOfPriorities)
            prio = _nrOfPriorities;
    }

}
