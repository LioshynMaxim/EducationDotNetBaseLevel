using DataStructures.Tasks.DoNotChange;
using System;
using System.Collections;
using System.Collections.Generic;

namespace DataStructures.Tasks
{
    public class DoublyLinkedList<T> : IDoublyLinkedList<T>
    {
        private int _count = 0;
        private Node<T> _head;
        private Node<T> _tail;
        public int Length => _count;

        public void Add(T e)
        {
            if (_head == null)
            {
                _head = new Node<T>(e);
                _tail = _head;
            }
            else 
            {
                var newNode = new Node<T>(e)
                {
                    Next = null,
                    Previous = _tail
                };
                _tail.Next = newNode;
                _tail = newNode;
            }
            _count++;
        }

        public void AddAt(int index, T e)
        {
            if (index < 0 || index > _count)
            {
                throw new IndexOutOfRangeException();
            }

            if (index == _count)
            {
                Add(e);
                return;
            }

            if (index == 0)
            {
                var newHead = new Node<T>(e)
                {
                    Next = _head,
                    Previous = null
                };
                _head.Previous = newHead;
                _head = newHead;
                _count++;
                return;
            }

            var node = _head;
            var count = 0;
            while (count < index)
            {
                node = node.Next;
                count++;
            }

            var newNode = new Node<T>(e)
            {
                Next = node.Next,
                Previous = node
            };

            if (node.Previous != null)
                node.Previous.Next = newNode;

            node.Previous = newNode;

            _count++;
        }

        public T ElementAt(int index)
        {
            if (index < 0 || index >= _count)
            {
                throw new IndexOutOfRangeException();
            }

            if (index == 0)
            {
                return _head.Value;
            }

            var node = _head;
            var count = 0;
            while (count < index)
            {
                node = node.Next;
                count++;
            }

            return node.Value;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new DoublyLinkedListEnumerator(this);
        }

        public void Remove(T item)
        {
            var node = _head;
            while (node != null)
            {
                if (node.Value.Equals(item))
                {
                    if (node.Previous != null) 
                    {
                        node.Previous.Next = node.Next;
                    }

                    if (node.Next != null)
                    {
                        node.Next.Previous = node.Previous;
                    }

                    if (node == _head)
                    {
                        _head = node.Next;
                    }

                    if (node == _tail) 
                    {
                        _tail = node.Previous;
                    }
                        
                    _count--;
                    return;
                }
                node = node.Next;
            }
        }

        public T RemoveAt(int index)
        {
            if (index < 0 || index >= _count)
            {
                throw new IndexOutOfRangeException();
            }

            var node = _head;
            var count = 0;
            while (count < index)
            {
                node = node.Next;
                count++;
            }

            if (node.Previous != null)
            {
                node.Previous.Next = node.Next;
            }

            if (node.Next != null)
            {
                node.Next.Previous = node.Previous;
            }

            _count--;
            return node.Value;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private class Node<vT>(vT value)
        {
            public vT Value { get; set; } = value;
            public Node<vT> Next { get; set; }
            public Node<vT> Previous { get; set; }
        }

        private class DoublyLinkedListEnumerator : IEnumerator<T>
        {
            private readonly DoublyLinkedList<T> _list;
            private Node<T> _current;
            private bool _started;

            public DoublyLinkedListEnumerator(DoublyLinkedList<T> list)
            {
                _list = list;
                _current = null;
                _started = false;
            }

            public T Current => _current.Value;

            object IEnumerator.Current => Current;

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                if (!_started) 
                {
                    _current = _list._head;
                    _started = true;
                }
                else
                {
                    _current = _current.Next;
                }
                return _current != null;
            }

            public void Reset()
            {
                _current = null;
                _started = false;
            }
        }
    }
}
