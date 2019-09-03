using System;

namespace Resolink
{
    /// <summary>
    /// Stores our buffered actions queued from callbacks
    /// </summary>
    /// <typeparam name="T">The data type the action takes</typeparam>
    public class ActionInvocationBuffer<T>
    {
        const int k_DefaultCapacity = 32;
            
        public Action<T>[] Actions;
        public T[] Values;

        public int Count { get; protected set; }

        public ActionInvocationBuffer(int capacity = k_DefaultCapacity)
        {
            Actions = new Action<T>[capacity];
            Values = new T[capacity];
        }

        public void Add(Action<T> action, T value)
        {
            if (Count >= Actions.Length)
            {
                Array.Resize(ref Actions, Actions.Length * 2);
                Array.Resize(ref Values, Values.Length * 2);
            }

            Actions[Count] = action;
            Values[Count] = value;
            Count++;
        }

        public void InvokeAll()
        {
            for (int i = 0; i < Count; i++)
                Actions[i](Values[i]);

            Count = 0;
        }
    }
}