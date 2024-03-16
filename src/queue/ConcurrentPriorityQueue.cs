using System.Collections.Concurrent;

namespace ConcurrentDataStructures;

/// <summary>
///     A concurrent FIFO priority queue.
/// </summary>
/// <typeparam name="T"></typeparam>
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

        for (var i = 1; i <= _nrOfPriorities; i++)
        {
            _queueMap.TryAdd(i, new ConcurrentLinkedList<T>());
        }
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
                    if (_queueMap.ContainsKey(i))
                        count += _queueMap[i].Count;
                }

                return count;
            }
        }
    }

    /// <summary>
    ///     Clear queue.
    /// </summary>
    public void Clear()
    {
        lock (_lock)
        {
            for (var i = 1; i <= _nrOfPriorities; i++)
            {
                if (_queueMap.ContainsKey(i))
                    _queueMap[i].Clear();
            }
        }
    }

    /// <summary>
    ///     Get queue copy.
    /// </summary>
    /// <param name="prio">Priority</param>
    /// <returns>Queue copy</returns>
    public LinkedList<T>? GetQueueCopy(int prio)
    {
        lock (_lock)
        {
            if (_queueMap.ContainsKey(prio))
                return new LinkedList<T>(_queueMap[prio]);

            return null;
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

            if (_queueMap.ContainsKey(priority))
                _queueMap[priority].AddLast(item);
        }
    }

    /// <summary>
    ///     Dequeue an item.
    /// </summary>
    /// <returns>The item (default if empty)</returns>
    public T? Dequeue()
    {
        lock (_lock)
        {
            for (var i = 1; i <= _nrOfPriorities; i++)
            {
                if (_queueMap.ContainsKey(i) && _queueMap[i].Count != 0)
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
