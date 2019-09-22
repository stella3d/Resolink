using System;
using OscJack;
using UnityEngine;
using UnityEngine.Events;

namespace Resolink
{
    public class OscEventHandler : MonoBehaviour
    {
        public ResolumeOscShortcut Shortcut;
    }

    [Serializable]
    [HelpURL(HelpLinks.TableOfContents)]
    [ExecuteAlways]
    public abstract class OscEventHandler<TEvent, T> : OscEventHandler
        where TEvent: UnityEvent<T>, new()
    {
        public TEvent Event;

        protected T Value;
        
        protected bool m_Registered;
        
        public void OnEnable()
        {
            if (Event == null)
                Event = new TEvent();

            if (OscRouter.Instance != null && !m_Registered)
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
            OscRouter.AddCallbacks(Shortcut.Output.Path, ReadData, Invoke);
            m_Registered = true;
        }

        protected void UnRegister()
        {
            OscRouter.RemoveCallbacks(Shortcut.Output.Path);
            m_Registered = false;
        }

        /// <summary>
        /// Extract a typed value from a data handle. 
        /// </summary>
        /// <param name="dataHandle">The handle to extract from</param>
        /// <returns>The message value</returns>
        protected abstract T GetMessageValue(OscDataHandle dataHandle);

        public void ReadData(OscDataHandle handle)
        {
            Value = GetMessageValue(handle);
        }
        
        public void Invoke()
        {
            Event.Invoke(Value);
        }

        public void InvokeFromHandle(OscDataHandle dataHandle)
        {
            Event.Invoke(GetMessageValue(dataHandle));
        }

        // the empty update function is here so the inspector has the disable checkbox
        public void Update() { }
    }
}