using System;
using UnityEngine;
using UnityEngine.Events;

namespace UnityResolume
{
    [Serializable]
    public class OscEvent<T> : MonoBehaviour
        where T: UnityEventBase, new()
    {
        public T Event;

        public ResolumeOscShortcut Shortcut;

        public OscEvent()
        {
            Event = new T();
        }
    }
}