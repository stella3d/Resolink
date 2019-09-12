using System;
using OscJack;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Resolink
{
    public class CompoundOscEventHandler : MonoBehaviour { }
    
    [HelpURL(HelpLinks.TableOfContents)]
    public abstract class CompoundOscEventHandler<THandler, TEvent, TCompoundData, TComponentData> : CompoundOscEventHandler
        where TEvent: UnityEvent<TCompoundData>, new()
        where THandler: OscActionEventHandler<TComponentData>
    {
        public TCompoundData Value;
        
        public TEvent Event;
        
        [FormerlySerializedAs("m_Handlers")] 
        public THandler[] Handlers;
        
        protected bool m_Registered;

        protected bool m_Dirty;
        
        public void OnEnable()
        {
            Setup();
            
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
        
        public void Update()
        {
            if (m_Dirty)
            {            
                // if any of the sub-handlers modified the value since last frame,
                // fire the UnityEvent that takes the compound data
                Event.Invoke(Value);
                m_Dirty = false;
            }
        }

        public void OnDisable()
        {
            UnRegister();
        }
        
        protected void Register()
        {
            foreach (var handler in Handlers)
            {
                if (handler.Shortcut == null)
                    continue;
                
                var action = GetWrapperForComponentEvent(handler);
                OscRouter.AddCallback(handler.Shortcut.Output.Path, action);
            }

            m_Registered = true;
        }

        protected void UnRegister()
        {
            foreach (var handler in Handlers)
            {
                if (handler.Shortcut == null)
                    continue;
                
                var action = GetWrapperForComponentEvent(handler);
                OscRouter.RemoveCallback(handler.Shortcut.Output.Path, action);
            }

            m_Registered = false;
        }

        /// <summary>
        /// When one of the component handlers fires, we also want to set the dirty state so the combined event fires.
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        Action<OscDataHandle> GetWrapperForComponentEvent(OscActionEventHandler<TComponentData> handler)
        {
            return handle =>
            {
                // invoking the sub-handler should modify our Value
                handler.InvokeFromHandle(handle);
                m_Dirty = true;
            };
        }

        public abstract void Setup();
    }
}