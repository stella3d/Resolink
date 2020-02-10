using System;
using OscCore;
using UnityEngine;
using UnityEngine.Events;

namespace Resolink
{
    public class ResolumeShortcutHandler : MonoBehaviour
    {
        public ResolumeOscShortcut Shortcut;
    }

    [Serializable]
    [HelpURL(HelpLinks.TableOfContents)]
    [ExecuteAlways]
    public abstract class ResolumeShortcutHandler<TEvent, T> : ResolumeShortcutHandler
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
            OscRouter.AddCallbacks(Shortcut.Output.Path, ReadDataCore, Invoke);
            m_Registered = true;
        }

        protected void UnRegister()
        {
            // OscRouter.RemoveCallbacks(Shortcut.Output.Path);
            OscRouter.RemoveCallbacksCore(Shortcut.Output.Path);
            m_Registered = false;
        }

        /// <summary>
        /// Extract a typed value from a data handle. 
        /// </summary>
        /// <param name="messageValues">The message value handle to extract from</param>
        /// <returns>The message value</returns>
        protected abstract T GetMessageValueCore(OscMessageValues messageValues);

        public void ReadDataCore(OscMessageValues handle)
        {
            Value = GetMessageValueCore(handle);
        }        
        
        public void Invoke()
        {
            Event.Invoke(Value);
        }

        // the empty update function is here so the inspector has the disable checkbox
        public void Update() { }
    }
}