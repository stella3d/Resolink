using System;
using UnityEngine;
using UnityEngine.Events;

namespace UnityResolume
{
    public class OscEventHandler : MonoBehaviour { }

    [Serializable]
    public class OscEventHandler<TEvent, T> : OscEventHandler
        where TEvent: UnityEvent<T>, new()
    {
        public TEvent Event;

        public ResolumeOscShortcut Shortcut;

        public OscEventHandler()
        {
            Event = new TEvent();
        }
    }
}