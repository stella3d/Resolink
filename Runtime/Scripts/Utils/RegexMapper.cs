using System;
using System.Text.RegularExpressions;
using OscJack;
using UnityEngine;

namespace Resolink
{
    public class RegexMapper<T>
    {
        public Regex[] Regexes;
        public T[] Values;

        public int Count { get; private set; }

        public RegexMapper(int capacity = 8)
        {
            Regexes = new Regex[capacity];
            Values = new T[capacity];
        }

        public void AddRegexToValue(Regex regex, T value)
        {
            if (Count >= Values.Length)
            {
                Array.Resize(ref Regexes, Regexes.Length * 2);
                Array.Resize(ref Values, Values.Length * 2);
            }

            Regexes[Count] = regex;
            Values[Count] = value;
            Count++;
        }

        public bool Process(string address, out T value)
        {
            for (var i = 0; i < Count; i++)
            {
                var regex = Regexes[i];
                if (regex.IsMatch(address))
                {
#if RESOLINK_DEBUG_REGEX 
                    Debug.Log($"{regex} matched {address}");
#endif
                    value = Values[i];
                    return true;
                }
            }

            value = default;
            return false;
        }

        public void Clear()
        {
            for (var i = 0; i < Count; i++)
            {
                Regexes[i] = null;
                Values[i] = default;
            }

            Count = 0;
        }
    }
    
    public class RegexActionMapper : RegexMapper<Action<OscDataHandle>> { }
        
    public class RegexTypeMapper : RegexMapper<Type> { }
}