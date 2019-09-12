using System;
using OscJack;
using UnityEngine;
using UnityEngine.Events;

namespace Resolink
{
    public abstract class CompoundOscEventHandler : MonoBehaviour { }
    
    /// <summary>
    /// Groups together primitive events into an event of a more complex data type
    /// </summary>
    /// <typeparam name="THandler">The type of Action-based address handler for each sub-event</typeparam>
    /// <typeparam name="TEvent">The type of UnityEvent-based handler for the single complex event</typeparam>
    /// <typeparam name="TCompoundData">The complex type of data TEvent takes</typeparam>
    /// <typeparam name="TComponentData">The primitive type of data sub-events take</typeparam>
    [HelpURL(HelpLinks.TableOfContents)]
    public abstract class CompoundOscEventHandler<THandler, TEvent, TCompoundData, TComponentData> : 
        CompoundOscEventHandler
        where TEvent: UnityEvent<TCompoundData>, new()
        where THandler: OscActionHandler<TComponentData>
    {
        /// <summary>The value that will be passed to the UnityEvent</summary>
        [Tooltip("The current value that will be passed to the event")]
        public TCompoundData Value;
                
        /// <summary>The value before any messages that change it are received</summary>
        [Tooltip("The value before any messages that change it are received")]
        [SerializeField] TCompoundData m_DefaultValue;
        public TCompoundData DefaultValue => m_DefaultValue;
        
        /// <summary>The UnityEvent that takes the complex data type</summary>
        public TEvent Event;
        
        /// <summary>All handlers for sub-events, associated with a Resolume shortcut</summary>
        public THandler[] Handlers;

        protected bool m_Registered;
        protected bool m_Dirty;
        
        public void OnEnable()
        {
            Setup();
            Value = m_DefaultValue;
            
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
        
        // We use LateUpdate instead of Update here so that we can be sure it runs after the OscRouter.
        // The OscRouter runs all of the callbacks that might set the dirty flag in Update.
        public void LateUpdate()
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
        /// <param name="handler">The event handler to wrap</param>
        /// <returns>A method that calls the wrapped function and sets the dirty flag</returns>
        Action<OscDataHandle> GetWrapperForComponentEvent(OscActionHandler<TComponentData> handler)
        {
            return handle =>
            {
                // invoking the sub-handler should modify our Value
                handler.InvokeFromHandle(handle);
                m_Dirty = true;
            };
        }

        public void AssignDefaultValue() { Value = m_DefaultValue; }

        public abstract void Setup();
    }
}