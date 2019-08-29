using System;
using OscJack;
using UnityEngine;
using UnityEngine.Events;

namespace Resolunity
{
    public class OscEventHandler : MonoBehaviour { }

    [Serializable]
    public abstract class OscEventHandler<TEvent, T> : OscEventHandler
        where TEvent: UnityEvent<T>, new()
    {
        public TEvent Event;

        public ResolumeOscShortcut Shortcut;
        
        public void OnEnable()
        {
            if (Event == null)
                Event = new TEvent();
            
            OscBrain.AddCallback(Shortcut.Output.Path, InvokeFromHandle);
        }

        public void OnDisable()
        {
            OscBrain.RemoveCallback(Shortcut.Output.Path, InvokeFromHandle);
        }

        protected abstract T GetMessageValue(OscDataHandle dataHandle);

        public void InvokeFromHandle(OscDataHandle dataHandle)
        {
            Event.Invoke(GetMessageValue(dataHandle));
        }

        // the empty update function is here so the inspector has the disable checkbox
        public void Update() { }
    }
}