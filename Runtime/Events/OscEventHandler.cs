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

        public void Invoke(T value)
        {
            if (!enabled)
                return;

            Event.Invoke(value);
        }
        
        // the empty update function is here so the inspector has the disable checkbox
        public void Update() { }
    }
}