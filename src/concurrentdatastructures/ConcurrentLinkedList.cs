using System.Collections;

namespace ConcurrentDataStructures
{
    /// <summary>
    ///     A simple concurrent linked list implementation based on .NET LinkedList.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ConcurrentLinkedList<T> : IEnumerable<T>
    {
        private readonly LinkedList<T> _linkedList = new();
        private readonly object _listLock = new();

        /// <summary>
        ///     Gets the number of items in the list.
        /// </summary>
        public int Count
        {
            get
            {
                lock (_listLock)
                {
                    return _linkedList.Count;
                }
            }
        }

        /// <summary>
        ///     Gets the first node in the list.
        /// </summary>
        public LinkedListNode<T> First
        {
            get
            {
                lock (_listLock)
                {
                    return _linkedList.First;
                }
            }
        }

        /// <summary>
        ///     Gets the last node in the list.
        /// </summary>
        public LinkedListNode<T> Last
        {
            get
            {
                lock (_listLock)
                {
                    return _linkedList.Last;
                }
            }
        }

        /// <summary>
        ///     Get enumerator.
        /// </summary>
        /// <returns>Enumerator</returns>
        public IEnumerator<T> GetEnumerator()
        {
            lock (_listLock)
            {
                foreach (var obj in _linkedList)
                    yield return obj;
            }
        }

        /// <summary>
        ///     Get enumerator.
        /// </summary>
        /// <returns>Enumerator</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            lock (_listLock)
            {
                return _linkedList.GetEnumerator();
            }
        }

        /// <summary>
        ///     Add an item to the end of the list.
        /// </summary>
        /// <param name="obj">Item</param>
        public void AddLast(T obj)
        {
            lock (_listLock)
            {
                _linkedList.AddLast(obj);
            }
        }


        /// <summary>
        ///     Adds the elements of the specified collection to the end of the ConcurrentLinkedList.
        /// </summary>
        /// <param name="collection">Item</param>
        public void AddRange(IEnumerable<T> collection)
        {
            lock (_listLock)
            {
                foreach (var obj in collection) _linkedList.AddLast(obj);
            }
        }

        /// <summary>
        ///     Add an item to the head of the list.
        /// </summary>
        /// <param name="obj">Item</param>
        public void AddFirst(T obj)
        {
            lock (_listLock)
            {
                _linkedList.AddFirst(obj);
            }
        }

        /// <summary>
        ///     Removes the first item in the list.
        /// </summary>
        public void RemoveFirst()
        {
            lock (_listLock)
            {
                if (_linkedList.Count != 0)
                    _linkedList.RemoveFirst();
            }
        }

        /// <summary>
        ///     Removes the last item in the list.
        /// </summary>
        public void RemoveLast()
        {
            lock (_listLock)
            {
                if (_linkedList.Count != 0)
                    _linkedList.RemoveLast();
            }
        }

        /// <summary>
        ///     Removes an item from the list.
        /// </summary>
        /// <param name="obj">Item</param>
        /// <returns>True on success, else false</returns>
        public bool Remove(T obj)
        {
            lock (_listLock)
            {
                return _linkedList.Remove(obj);
            }
        }

        /// <summary>
        ///     Finds the first node in the list that contains the specific value.
        /// </summary>
        /// <param name="obj">Item</param>
        /// <returns>The node in the list that contains the specific value</returns>
        public LinkedListNode<T> Find(T obj)
        {
            lock (_listLock)
            {
                return _linkedList.Find(obj);
            }
        }

        /// <summary>
        ///     Finds the last node in the list that contains the specific value.
        /// </summary>
        /// <param name="obj">Item</param>
        /// <returns>The node in the list that contains the specific value</returns>
        public LinkedListNode<T> FindLast(T obj)
        {
            lock (_listLock)
            {
                return _linkedList.FindLast(obj);
            }
        }

        /// <summary>
        ///     Checks if the list contains an item.
        /// </summary>
        /// <param name="obj">Item</param>
        /// <returns>True if the list contains the item, else false</returns>
        public bool Contains(T obj)
        {
            lock (_listLock)
            {
                return _linkedList.Contains(obj);
            }
        }

        /// <summary>
        ///     Clears the list.
        /// </summary>
        public void Clear()
        {
            lock (_listLock)
            {
                _linkedList.Clear();
            }
        }
    }
}
