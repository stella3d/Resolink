using System;
using OscCore;
using UnityEditor;
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

        protected OscActionPair m_ActionPair;
        
        public void OnEnable()
        {
            if (Event == null)
                Event = new TEvent();

            if (OscRouter.Instance != null && !m_Registered)
                Register();
        }

        void Start()
        {
#if UNITY_EDITOR
            if (!EditorApplication.isPlayingOrWillChangePlaymode)
                return;
#endif
            if (!m_Registered)
                Register();
        }

        public void OnDisable()
        {
            UnRegister();
        }
        
        protected void Register()
        {
            m_ActionPair = new OscActionPair(ReadData, Invoke);
            OscRouter.AddCallbacks(Shortcut.Output.Path, m_ActionPair);
            m_Registered = true;
        }

        protected void UnRegister()
        {
            OscRouter.RemoveCallbacks(Shortcut.Output.Path, m_ActionPair);
            m_Registered = false;
        }

        /// <summary>
        /// Extract a typed value from a data handle. 
        /// </summary>
        /// <param name="dataHandle">The handle to extract from</param>
        /// <returns>The message value</returns>
        protected abstract T GetMessageValue(OscMessageValues dataHandle);

        public void ReadData(OscMessageValues handle)
        {
            Value = GetMessageValue(handle);
        }
        
        public void Invoke()
        {
            Event.Invoke(Value);
        }

        public void InvokeFromHandle(OscMessageValues dataHandle)
        {
            Event.Invoke(GetMessageValue(dataHandle));
        }

        // the empty update function is here so the inspector has the disable checkbox
        public void Update() { }
    }
}