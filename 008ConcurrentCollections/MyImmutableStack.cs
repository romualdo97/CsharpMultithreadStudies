using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
// By Eric Lippert
namespace ConcurrentCollections
{
    public interface IStack<T> : IEnumerable<T>
    {
        bool IsEmpty { get; }
        IStack<T> Push(T value);
        IStack<T> Pop();
        T Peek();
    }

    public sealed class Stack<T> : IStack<T>
    {
        private sealed class EmptyStack : IStack<T>
        {
            public bool IsEmpty => true;

            public T Peek()
            {
                throw new Exception("Empty stack");
            }

            public IStack<T> Pop()
            {
                throw new Exception("Empty stack");
            }

            public IStack<T> Push(T value)
            {
                return new Stack<T>(value, this);
            }

            public IEnumerator<T> GetEnumerator()
            {
                yield break;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        private readonly static EmptyStack empty = new EmptyStack();
        public static IStack<T> Empty => empty;
        public bool IsEmpty => false;

        private readonly T m_head;
        private readonly IStack<T> m_tail;

        private Stack(T head, IStack<T> tail)
        {
            m_head = head;
            m_tail = tail;
        }

        public T Peek()
        {
            return m_head;
        }

        public IStack<T> Pop()
        {
            return m_tail;
        }

        public IStack<T> Push(T value)
        {
            return new Stack<T>(value, this);
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (IStack<T> stack = this; !stack.IsEmpty; stack = stack.Pop())
            {
                yield return stack.Peek();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
