using System;
using System.Text.RegularExpressions;
using OscJack;
using UnityEngine;

namespace Resolink
{
    public sealed class RegexActionMapper
    {
        public Regex[] Regexes;
        public Action<OscDataHandle>[] Handlers;

        public int Count { get; private set; }

        public RegexActionMapper(int capacity = 8)
        {
            Regexes = new Regex[capacity];
            Handlers = new Action<OscDataHandle>[capacity];
        }

        public void AddRegexHandler(Regex regex, Action<OscDataHandle> handler)
        {
            if (Count >= Handlers.Length)
            {
                Array.Resize(ref Regexes, Regexes.Length * 2);
                Array.Resize(ref Handlers, Handlers.Length * 2);
            }

            Regexes[Count] = regex;
            Handlers[Count] = handler;
            Count++;
        }

        public bool Process(string address, out Action<OscDataHandle> handler)
        {
            for (var i = 0; i < Regexes.Length; i++)
            {
                var regex = Regexes[i];
                if (regex.IsMatch(address))
                {
#if RESOLINK_DEBUG_REGEX || true
                    Debug.Log($"{regex} matched {address}");
#endif
                    handler = Handlers[i];
                    return true;
                }
            }

            handler = null;
            return false;
        }
    }
}