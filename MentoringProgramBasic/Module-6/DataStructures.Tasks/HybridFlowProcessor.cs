using DataStructures.Tasks.DoNotChange;
using System;
using System.Collections.Specialized;

namespace DataStructures.Tasks
{
    public class HybridFlowProcessor<T> : IHybridFlowProcessor<T>
    {
        private DoublyLinkedList<T> _list = new DoublyLinkedList<T>();
        public T Dequeue()
        {
            if (_list.Length == 0)
            {
                throw new InvalidOperationException("The collection is empty.");
            }
            return _list.RemoveAt(0);
        }

        public void Enqueue(T item)
        {
            _list.AddAt(_list.Length, item);
        }

        public T Pop()
        {
            if (_list.Length == 0)
            {
                throw new InvalidOperationException("The collection is empty.");
            }
            return _list.RemoveAt(_list.Length - 1);
        }

        public void Push(T item)
        {
            _list.AddAt(_list.Length, item);
        }
    }
}
