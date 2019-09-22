using System;
using OscJack;
using UnityEngine;
using UnityEngine.Events;

namespace Resolink
{
    public class OscEventHandler : MonoBehaviour
    {
        public ResolumeOscShortcut Shortcut;
        
        public virtual void SendValue() { }
    }

    [Serializable]
    [HelpURL(HelpLinks.TableOfContents)]
    [ExecuteAlways]
    public abstract class OscEventHandler<TEvent, T> : OscEventHandler
        where TEvent: UnityEvent<T>, new()
    {
        public TEvent Event;

        protected byte m_Registered;
        protected bool m_OutputDirty;

        protected T m_Value;

        public T Value
        {
            get => m_Value;
            set 
            { 
                m_Value = value;
                //m_OutputDirty = true;
                SendValue();
            }
        }
        
        public void OnEnable()
        {
            if (Event == null)
                Event = new TEvent();

            if (OscRouter.Instance != null && m_Registered > byte.MinValue)
                Register();
        }

        void Start()
        {
            if (m_Registered == byte.MinValue)
                Register();
        }

        public void OnDisable()
        {
            UnRegister();
        }
        
        protected void Register()
        {
            OscRouter.AddCallbacks(Shortcut.Output.Path, ReadData, Invoke);
            m_Registered = byte.MaxValue;
        }

        protected void UnRegister()
        {
            OscRouter.RemoveCallbacks(Shortcut.Output.Path);
            m_Registered = byte.MinValue;
        }

        /// <summary>
        /// Extract a typed value from a data handle. 
        /// </summary>
        /// <param name="dataHandle">The handle to extract from</param>
        /// <returns>The message value</returns>
        protected abstract T GetMessageValue(OscDataHandle dataHandle);

        public void ReadData(OscDataHandle handle)
        {
            m_Value = GetMessageValue(handle);
        }
        
        public void Invoke()
        {
            Event.Invoke(m_Value);
        }

        public void Update() { }
    }
}