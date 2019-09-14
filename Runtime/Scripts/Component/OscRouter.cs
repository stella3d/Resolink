using System;
using System.Collections.Generic;
using OscJack;
using UnityEngine;

namespace Resolink
{
    /// <summary>
    /// Handles routing messages to the callbacks registered for each address
    /// </summary>
    [ExecuteAlways]
    public class OscRouter : MonoBehaviour
    {
        readonly HashSet<OscServer> m_KnownServers = new HashSet<OscServer>();
        
        internal readonly Dictionary<string, List<Action<OscDataHandle>>> m_AddressHandlers = 
            new Dictionary<string, List<Action<OscDataHandle>>>(32);
        
        internal readonly Dictionary<string, Action<OscDataHandle>> m_WildcardAddressHandlers = 
            new Dictionary<string, Action<OscDataHandle>>(8);

        /// <summary>
        /// Every incoming osc address we tried to find a template handler for and failed
        /// </summary>
        readonly HashSet<string> m_AddressesToIgnore = new HashSet<string>();
        
        readonly ActionInvocationBuffer<OscDataHandle> m_ActionInvocationBuffer = 
            new ActionInvocationBuffer<OscDataHandle>();

        bool m_PrimaryCallbackAdded;
        int m_PreviousServerCount;

        readonly RegexActionMapper m_TemplateChecker = new RegexActionMapper();
        
        public static OscRouter Instance { get; protected set; }

        public Dictionary<string, List<Action<OscDataHandle>>> AddressHandlers => m_AddressHandlers;
        public Dictionary<string, Action<OscDataHandle>> WildcardAddressHandlers => m_WildcardAddressHandlers;

        void OnEnable()
        {
            Instance = this;
            AddPrimaryCallback(PrimaryCallback);
            m_PrimaryCallbackAdded = true;
            HideOscEventReceiver();
        }

        void Awake()
        {
            Instance = this;
            HideOscEventReceiver();
        }

        void Start()
        {
            if (!m_PrimaryCallbackAdded)
            {
                AddPrimaryCallback(PrimaryCallback);
                m_PrimaryCallbackAdded = true;
            }
        }

        void OnDisable()
        {
            m_PrimaryCallbackAdded = false;
            RemovePrimaryCallback(PrimaryCallback);
        }

        void Update()
        {
            // call all Actions buffered in response to messages since last frame
            m_ActionInvocationBuffer.InvokeAll();

            // handle the existence of any new osc servers
            HandleOscServerChanges();
        }
        
        void HandleOscServerChanges()
        {
            if (OscServer.ServerList.Count == m_PreviousServerCount) 
                return;
            
            foreach (var server in OscServer.ServerList)
            {
                if (m_KnownServers.Contains(server))
                    continue;

                server.MessageDispatcher.AddCallback(string.Empty, PrimaryCallback);
                m_KnownServers.Add(server);
            }

            m_PreviousServerCount = OscServer.ServerList.Count;
        }
        
        /// <summary>
        /// Register a new OSC message handler  
        /// </summary>
        /// <param name="address">The URL path to handle messages for</param>
        /// <param name="callback">The action to take when the message is received</param>
        public static void AddCallback(string address, Action<OscDataHandle> callback)
        {
            if (PathUtils.IsWildcardTemplate(address))
            {
                if (Instance.m_WildcardAddressHandlers.ContainsKey(address))
                {
                    Debug.LogWarning($"A wildcard handler for {address} has already been registered");
                    return;
                }

                Instance.m_WildcardAddressHandlers[address] = callback;
            }

            if (!Instance.m_AddressHandlers.TryGetValue(address, out var callbackList))
            {
                callbackList = new List<Action<OscDataHandle>>();
                Instance.m_AddressHandlers[address] = callbackList;
            }
            
            callbackList.Add(callback);
        }
        
        /// <summary>
        /// Remove a previously registered OSC message handler  
        /// </summary>
        /// <param name="address">The URL path to handle messages for</param>
        /// <param name="callback">The action to remove</param>
        public static void RemoveCallback(string address, Action<OscDataHandle> callback)
        {
            if (Instance.m_AddressHandlers.TryGetValue(address, out var callbackList))
            {
                callbackList.Remove(callback);
                if (callbackList.Count == 0)
                    Instance.m_AddressHandlers.Remove(address);
            }
        }

        static void AddPrimaryCallback(OscMessageDispatcher.MessageCallback callback)
        {
            foreach (var server in OscServer.ServerList)
                server.MessageDispatcher.AddCallback(string.Empty, callback);
        }

        static void RemovePrimaryCallback(OscMessageDispatcher.MessageCallback callback)
        {
            foreach (var server in OscServer.ServerList)
                server.MessageDispatcher.RemoveCallback(string.Empty, callback);
        }

        /// <summary>
        /// This is the message handler for every OSC message we receive
        /// </summary>
        /// <param name="address">The URL path where this message was received</param>
        /// <param name="handle">A handle to access the value of the message</param>
        protected void PrimaryCallback(string address, OscDataHandle handle)
        {
#if RESOLINK_DEBUG_OSC || true
            Debug.Log(address + " " + handle.GetElementAsString(0));
#endif
            if (m_AddressesToIgnore.Contains(address))
                return;
            
            if (!m_AddressHandlers.TryGetValue(address, out var callbackList))
            {
                // if we find a match in the template handlers, add a handler, otherwise ignore this address
                if (m_TemplateChecker.Process(address, out var handler))
                {
                    callbackList = new List<Action<OscDataHandle>> { handler };
                    m_AddressHandlers[address] = callbackList;
                }
                else
                {
                    m_AddressesToIgnore.Add(address);
                }

                return;
            }

            // This callback will be called on another thread, but UnityEvents can only be called on the main thread.
            // So we buffer all the actions here and call them at the start of next frame
            foreach (var callback in callbackList)
            {
                m_ActionInvocationBuffer.Add(callback, handle);
            }
        }

        // the event receiver component is basically internal to Resolink,
        // and it's intended to register callbacks through its system, so hide receiver component.
        void HideOscEventReceiver()
        {
            var receiver = GetComponent<OscEventReceiver>();
            if (receiver == null)
                return;

            receiver.hideFlags = HideFlags.HideInInspector;
        }

        /// <summary>
        /// Messages arriving at addresses we can't find handlers for get added to an ignore set.
        /// Call this to clear that - You would do this if handlers were added later at runtime.
        /// This should not be needed otherwise
        /// </summary>
        public static void ClearIgnoredAddresses()
        {
            Instance.m_AddressesToIgnore.Clear();
        }
    }
}