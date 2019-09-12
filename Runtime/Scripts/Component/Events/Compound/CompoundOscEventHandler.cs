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

        public void OnDisable()
        {
            UnRegister();
        }
        
        protected void Register()
        {
            foreach (var handler in Handlers)
            {
                var action = GetCombinedInvokeForComponentEvent(handler);
                OscRouter.AddCallback(handler.Shortcut.Output.Path, action);
            }

            m_Registered = true;
        }

        protected void UnRegister()
        {
            foreach (var handler in Handlers)
            {
                var action = GetCombinedInvokeForComponentEvent(handler);
                OscRouter.RemoveCallback(handler.Shortcut.Output.Path, action);
            }

            m_Registered = false;
        }

        /// <summary>
        /// When one of the component handlers fires, we also want to fire the combined handler.
        /// This way, we can fire events that take non-primitive types or vectors
        /// when we change one of their primitive components via OSC
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        Action<OscDataHandle> GetCombinedInvokeForComponentEvent(OscActionEventHandler<TComponentData> handler)
        {
            return handle =>
            {
                handler.InvokeFromHandle(handle);
                Event.Invoke(Value);
            };
        }

        public abstract void Setup();

        // the empty update function is here so the inspector has the disable checkbox
        public void Update() { }
    }
}