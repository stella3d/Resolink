using System;
using OscJack;
using UnityEngine;
using UnityEngine.Events;

namespace Resolunity
{
    public class OscEventHandler : MonoBehaviour
    {
        public ResolumeOscShortcut Shortcut;
    }

    [Serializable]
    public abstract class OscEventHandler<TEvent, T> : OscEventHandler
        where TEvent: UnityEvent<T>, new()
    {
        public TEvent Event;

        protected bool m_Registered;
        
        public void OnEnable()
        {
            if (Event == null)
                Event = new TEvent();

            if (OscBrain.Instance != null && !m_Registered)
                Register();
        }

        void Start()
        {
            if (!m_Registered)
                Register();
        }

        public void OnDisable()
        {
            UnRegister();
        }
        
        protected void Register()
        {
            OscBrain.AddCallback(Shortcut.Output.Path, InvokeFromHandle);
            m_Registered = true;
        }

        protected void UnRegister()
        {
            OscBrain.RemoveCallback(Shortcut.Output.Path, InvokeFromHandle);
            m_Registered = false;
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