using System;
using System.Collections.Generic;
using OscJack;
using UnityEngine;

namespace Resolink
{
    public class OscActionPair
    {
        public Action<OscDataHandle> ValueRead;
        public Action UserCallback;

        public OscActionPair(Action<OscDataHandle> valueRead, Action userCallback)
        {
            ValueRead = valueRead;
            UserCallback = userCallback;
        }
    }

    /// <summary>
    /// Handles routing messages to the callbacks registered for each address
    /// </summary>
    [ExecuteAlways]
    public class OscRouter : MonoBehaviour
    {
        readonly HashSet<OscServer> m_KnownServers = new HashSet<OscServer>();

        const int k_DefaultCapacity = 24;
        
        public readonly Dictionary<string, OscActionPair> NewAddressHandlers = 
            new Dictionary<string, OscActionPair>(k_DefaultCapacity);

        static int s_CallbackAddIndex;
        
        internal readonly Dictionary<string, List<Action<OscDataHandle>>> m_AddressHandlers = 
            new Dictionary<string, List<Action<OscDataHandle>>>(k_DefaultCapacity);
        
        internal readonly Dictionary<string, Action<OscDataHandle>> m_WildcardAddressHandlers = 
            new Dictionary<string, Action<OscDataHandle>>(8);

        /// <summary>
        /// Every incoming osc address we tried to find a template handler for and failed
        /// </summary>
        readonly HashSet<string> m_AddressesToIgnore = new HashSet<string>();
        
        readonly ActionInvocationBuffer<OscDataHandle> m_ActionInvocationBuffer = 
            new ActionInvocationBuffer<OscDataHandle>();
        
        readonly ActionInvocationBuffer m_NewActionInvocationBuffer = new ActionInvocationBuffer();

        bool m_PrimaryCallbackAdded;
        int m_PreviousServerCount;

        readonly RegexActionMapper m_TemplateChecker = new RegexActionMapper();
        readonly RegexDoubleActionMapper m_NewTemplateChecker = new RegexDoubleActionMapper();
        
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
                //AddPrimaryCallback(PrimaryCallback);
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
            m_NewActionInvocationBuffer.InvokeAll();

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

        public static void AddCallbacks(string address, OscActionPair actionPair)
        {
            if (PathUtils.IsWildcardTemplate(address))
            {
                if (Instance.m_WildcardAddressHandlers.ContainsKey(address))
                {
                    Debug.LogWarning($"A wildcard handler for {address} has already been registered");
                    return;
                }

                Instance.m_WildcardAddressHandlers[address] = actionPair.ValueRead;
            }

            Instance.NewAddressHandlers[address] = actionPair;
        }

        public static void AddCallbacks(string address, Action<OscDataHandle> valueRead, Action userCallback)
        {
            var actionPair = new OscActionPair(valueRead, userCallback);
            AddCallbacks(address, actionPair);
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
        
        public static bool RemoveCallbacks(string address)
        {
            return Instance.NewAddressHandlers.Remove(address);
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
#if RESOLINK_DEBUG_OSC
            Debug.Log(address + " " + handle.GetElementAsString(0));
#endif
            if (m_AddressesToIgnore.Contains(address))
                return;
            
            if (!NewAddressHandlers.TryGetValue(address, out var actionPair))
            {
                // if we find a match in the template handlers, add a handler, otherwise ignore this address
                if (m_NewTemplateChecker.Process(address, out var newActionPair))
                {
                    AddCallbacks(address, newActionPair);
                    actionPair = newActionPair;
                }
                else
                {
                    m_AddressesToIgnore.Add(address);
                    return;
                }
            }

            // immediately read the value from the OSC buffer to prevent values going to the wrong controls
            actionPair.ValueRead(handle);
            
            // queue user action here and call them next frame, on the main thread.
            // if the callback is null, that means it's a compound control, which will fire its own user callback
            if(actionPair.UserCallback != null)
                m_NewActionInvocationBuffer.Add(actionPair.UserCallback);
        }

        // the event receiver component from OscJack is basically internal to Resolink, so we hide it
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