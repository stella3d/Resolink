using System;
using OscJack;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

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
    [ExecuteAlways]
    public abstract class CompoundOscEventHandler<THandler, TEvent, TCompoundData, TComponentData> : 
        CompoundOscEventHandler
        where TEvent: UnityEvent<TCompoundData>, new()
        where THandler: OscActionHandler<TComponentData>
    {
        /// <summary>
        /// The value that will be passed to the UnityEvent
        /// </summary>
        [FormerlySerializedAs("Value")] [Tooltip("The current value that will be passed to the event")]
        public TCompoundData m_Value;
        
#pragma warning disable 649           
        /// <summary>
        /// The value before any messages that change it are received
        /// </summary>
        [Tooltip("The value before any messages that change it are received")]
        [SerializeField] protected TCompoundData m_DefaultValue;
#pragma warning restore 649           

        public TCompoundData DefaultValue => m_DefaultValue;
        
        /// <summary>
        /// The UnityEvent that takes the complex data type
        /// </summary>
        public TEvent Event;
        
        /// <summary>
        /// All handlers for sub-events, associated with a Resolume shortcut
        /// </summary>
        [SerializeField]
        public THandler[] Handlers;

        protected bool m_Registered;
        protected bool m_InputDirty;
        protected bool m_OutputDirty;

        public TCompoundData Value
        {
            get => m_Value;
            set
            {
                m_Value = value;
                m_OutputDirty = true;
            }
        }

        public void OnEnable()
        {
            Setup();
            m_Value = m_DefaultValue;
            
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
        
        public virtual void Update()
        {
            if (m_InputDirty)
            {
                ProcessBeforeInvoke();
                // if any of the sub-handlers modified the value since last frame,
                // fire the UnityEvent that takes the compound data
                Event.Invoke(m_Value);
                m_InputDirty = false;
            }

            if (m_OutputDirty)
                SendValue();
        }
        
        public abstract void Setup();

        /// <summary>
        /// Do any processing necessary before invoking the user callback
        /// </summary>
        protected virtual void ProcessBeforeInvoke() { }

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
                
                var action = ReadAndSetDirty(handler);
                OscRouter.AddCallbacks(handler.Shortcut.Output.Path, action, null);
            }

            m_Registered = true;
        }

        protected void UnRegister()
        {
            foreach (var handler in Handlers)
            {
                if (handler.Shortcut == null)
                    continue;
                
                OscRouter.RemoveCallbacks(handler.Shortcut.Output.Path);
            }

            m_Registered = false;
        }
        
        Action<OscDataHandle> ReadAndSetDirty(OscActionHandler<TComponentData> handler)
        {
            return handle =>
            {
                // invoking the sub-handler should modify our Value
                handler.InvokeFromHandle(handle);
                m_InputDirty = true;
            };
        }

        public void AssignDefaultValue() { m_Value = m_DefaultValue; }

        public void SendValue(TCompoundData value)
        {
            m_Value = value;
            SendValue();
        }
        
        public virtual void SendValue() { }
    }
}