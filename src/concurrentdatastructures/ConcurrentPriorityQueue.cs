using System.Collections.Concurrent;

namespace ConcurrentDataStructures;

/// <summary>
///     A concurrent FIFO priority queue.
/// </summary>
/// <typeparam name="T"></typeparam>
public class ConcurrentPriorityQueue<T>
{
    private const int MIN_NR_PRIO = 3;
    private const int MAX_NR_PRIO = 99;

    private readonly object _lock = new();
    private readonly int _nrOfPriorities;
    private int _headPrio = int.MaxValue;
    private ConcurrentDictionary<int, ConcurrentLinkedList<T>> _queueTable;

    /// <summary>
    ///     Creates a concurrent FIFO priority queue.
    /// </summary>
    /// <param name="nrOfPriorities">Number of available priorities, higher priority = lower number and vice versa (min MIN_NR_PRIO, max MAX_NR_PRIO)</param>
    public ConcurrentPriorityQueue(int nrOfPriorities = MIN_NR_PRIO)
    {
        if (nrOfPriorities < MIN_NR_PRIO)
            nrOfPriorities = MIN_NR_PRIO;
        else if (nrOfPriorities > MAX_NR_PRIO)
            nrOfPriorities = MAX_NR_PRIO;

        _queueTable = new ConcurrentDictionary<int, ConcurrentLinkedList<T>>();
        _nrOfPriorities = nrOfPriorities;

        for (var i = 1; i <= _nrOfPriorities; i++)
        {
            _queueTable.TryAdd(i, new ConcurrentLinkedList<T>());
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

                for (var i = _headPrio; i <= _nrOfPriorities; i++)
                {
                    if (_queueTable.ContainsKey(i))
                        count += _queueTable[i].Count;
                }

                return count;
            }
        }
    }

    /// <summary>
    ///     Get first item in queue.
    /// </summary>
    public T? First
    {
        get
        {
            lock (_lock)
            {
                if (_queueTable.ContainsKey(_headPrio) && _queueTable[_headPrio].Count == 0)
                    AdjustHeadPrio();

                return _queueTable[_headPrio].First != null ? _queueTable[_headPrio].First.Value : default;
            }
        }
    }

    /// <summary>
    ///     Remote an item from the queue.
    /// </summary>
    /// <param name="item">The item</param>
    /// <returns>True on success, else false</returns>
    public bool Remove(T item)
    {
        lock (_lock)
        {
            for (var i = _headPrio; i <= _nrOfPriorities; i++)
            {
                if (_queueTable.ContainsKey(i) && _queueTable[i].Remove(item))
                    return true;
            }

            return false;
        }
    }

    /// <summary>
    ///     Checks if queue contains item.
    /// </summary>
    /// <param name="item">The item</param>
    /// <returns>True if it exists, else false</returns>
    public bool Contains(T item)
    {
        lock (_lock)
        {
            for (var i = _headPrio; i <= _nrOfPriorities; i++)
            {
                if (_queueTable.ContainsKey(i) && _queueTable[i].Contains(item))
                    return true;
            }

            return false;
        }
    }

    /// <summary>
    ///     Clear queue.
    /// </summary>
    public void Clear()
    {
        lock (_lock)
        {
            for (var i = _headPrio; i <= _nrOfPriorities; i++)
            {
                if (_queueTable.ContainsKey(i))
                    _queueTable[i].Clear();
            }
        }
    }

    /// <summary>
    ///     Get queue copy.
    /// </summary>
    /// <param name="prio">Priority</param>
    /// <returns>Queue copy (empty if priority not found)</returns>
    public LinkedList<T> GetQueueCopy(int prio)
    {
        lock (_lock)
        {
            if (_queueTable.ContainsKey(prio))
                return new LinkedList<T>(_queueTable[prio]);

            return new LinkedList<T>();
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

            if (priority < _headPrio)
                _headPrio = priority;

            if (_queueTable.ContainsKey(priority))
                _queueTable[priority].AddLast(item);
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
            if (_queueTable.ContainsKey(_headPrio))
            {
                if (_queueTable[_headPrio].Count == 0)
                    AdjustHeadPrio();

                if (_queueTable[_headPrio].Count != 0)
                {
                    var node = _queueTable[_headPrio].First;
                    _queueTable[_headPrio].RemoveFirst();

                    return node.Value;
                }
            }

            return default;
        }
    }

    /// <summary>
    ///     Adjusts the head prio to the "highest possible".
    /// </summary>
    private void AdjustHeadPrio()
    {
        for (var i = _headPrio; i <= NrOfPriorities; i++)
        {
            if (_queueTable.ContainsKey(i) && _queueTable[i].Count != 0)
            {
                _headPrio = i;
                return;
            }
        }
    }

    /// <summary>
    ///     Adjust priority.
    /// </summary>
    /// <param name="prio">Wanted priority</param>
    private void AdjustPriority(ref int prio)
    {
        if (prio < 1)
            prio = 1;
        else if (prio > _nrOfPriorities)
            prio = _nrOfPriorities;
    }

}
