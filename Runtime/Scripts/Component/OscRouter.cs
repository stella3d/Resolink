using System;
using System.Collections.Generic;
using OscJack;
using OscCore;
using UnityEngine;

using OscServer = OscJack.OscServer;
using CoreServer = OscCore.OscServer;

namespace Resolink
{
    /// <summary>
    /// Handles routing messages to the callbacks registered for each address
    /// </summary>
    [ExecuteAlways]
    public class OscRouter : MonoBehaviour
    {
        const int k_DefaultCapacity = 24;
#pragma warning disable 649
        static OscServer s_SharedServer;
        internal CoreServer CoreServer;
#pragma warning restore 649
        static int s_CallbackAddIndex;

        readonly HashSet<OscServer> m_KnownServers = new HashSet<OscServer>();
        
        public readonly Dictionary<string, OscActionPair> AddressHandlers = 
            new Dictionary<string, OscActionPair>(k_DefaultCapacity);
        
        internal static readonly HashSet<string> m_WildcardAddressHandlers = new HashSet<string>();

        /// <summary>
        /// Every incoming osc address we tried to find a template handler for and failed
        /// </summary>
        readonly HashSet<string> m_AddressesToIgnore = new HashSet<string>();

        readonly ActionInvocationBuffer m_ActionInvocationBuffer = new ActionInvocationBuffer();

        bool m_PrimaryCallbackAdded;
        int m_PreviousServerCount;

        readonly RegexDoubleActionMapper m_TemplateChecker = new RegexDoubleActionMapper();
        
        [SerializeField] 
        int m_Port = 9000;

        public int Port => m_Port;
        public static OscRouter Instance { get; protected set; }
        public HashSet<string> WildcardAddressHandlers => m_WildcardAddressHandlers;

        void OnEnable()
        {
            Instance = this;
            AddPrimaryCallback(PrimaryCallback);
            m_PrimaryCallbackAdded = true;
            GetSharedServer();
        }

        void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            GetSharedServer();
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
            // the server list is only defined in the editor
#if UNITY_EDITOR
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
#endif
        }

        /// <summary>
        /// Register a new pair of OSC message handlers
        /// </summary>
        /// <param name="address">The URL path to handle messages for</param>
        /// <param name="actionPair">The value read action & user callback to execute</param>
        public static void AddCallbacks(string address, OscActionPair actionPair)
        {
            
            if (PathUtils.IsWildcardTemplate(address))
            {
                if (Instance.WildcardAddressHandlers.Contains(address))
                {
                    Debug.LogWarning($"A wildcard handler for {address} has already been registered");
                    return;
                }

                Instance.WildcardAddressHandlers.Add(address);
                Instance.m_TemplateChecker.Add(PathUtils.RegexForWildcardPath(address), actionPair);
            }

            // Instance.CoreServer.TryAddMethodPair(address, actionPair);

            Instance.AddressHandlers[address] = actionPair;
        }

        void GetSharedServer()
        {
            //s_SharedServer = OscMaster.GetSharedServer(Port);
            CoreServer = CoreServer.GetOrCreate(m_Port);
        }

        /// <summary>
        /// Register a new pair of OSC message handlers
        /// </summary>
        /// <param name="address">The URL path to handle messages for</param>
        /// <param name="valueRead">The value read action to execute immediately on the worker thread</param>
        /// <param name="userCallback">The user callback to queue for execution on the main thread</param>
        public static void AddCallbacks(string address, Action<OscMessageValues> valueRead, Action userCallback)
        {
            AddCallbacks(address, new OscActionPair(valueRead, userCallback));
        }
        
        /// <summary>
        /// Remove a previously registered OSC message handler  
        /// </summary>
        /// <param name="address">The URL path to stop handling messages for</param>
        public static bool RemoveCallbacks(string address)
        {
            return Instance.AddressHandlers.Remove(address); 
        }

        static void AddPrimaryCallback(OscMessageDispatcher.MessageCallback callback)
        {
#if UNITY_EDITOR
            foreach (var server in OscServer.ServerList)
                server.MessageDispatcher.AddCallback(string.Empty, callback);
#else
            s_SharedServer.MessageDispatcher.AddCallback(string.Empty, callback);
#endif
        }

        static void RemovePrimaryCallback(OscMessageDispatcher.MessageCallback callback)
        {
#if UNITY_EDITOR
            foreach (var server in OscServer.ServerList)
            {
                try { server.MessageDispatcher.RemoveCallback(string.Empty, callback); }
                catch (KeyNotFoundException) { /* it don't matter */ }
            }
#else
            s_SharedServer.MessageDispatcher.RemoveCallback(string.Empty, callback);
#endif
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
            
            if (!AddressHandlers.TryGetValue(address, out var actionPair))
            {
                // if we find a match in the template handlers, add a handler, otherwise ignore this address
                if (m_TemplateChecker.Process(address, out var newActionPair))
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
            // actionPair.ValueRead(handle);
            
            // queue user action here and call them next frame, on the main thread.
            // if the callback is null, that means it's a compound control, which will fire its own user callback
            if(actionPair.MainThreadQueued != null)
                m_ActionInvocationBuffer.Add(actionPair.MainThreadQueued);
        }
        
        // OSCCORE METHOD
        protected void PrimaryCallbackCore(string address, OscMessageValues handle)
        {
#if RESOLINK_DEBUG_OSC
            Debug.Log(address + " " + handle.GetElementAsString(0));
#endif
            
            if (m_AddressesToIgnore.Contains(address))
                return;
            
            if (!AddressHandlers.TryGetValue(address, out var actionPair))
            {
                // if we find a match in the template handlers, add a handler, otherwise ignore this address
                if (m_TemplateChecker.Process(address, out var newActionPair))
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
            if(actionPair.MainThreadQueued != null)
                m_ActionInvocationBuffer.Add(actionPair.MainThreadQueued);
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