using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using OscCore;
using OscJack;
using UnityEngine;
using OscServer = OscCore.OscServer; 

namespace Resolink
{
    /// <summary>
    /// Handles routing messages to the callbacks registered for each address
    /// </summary>
    //[ExecuteAlways]
    public class OscRouter : MonoBehaviour
    {
        const int k_DefaultCapacity = 24;
#pragma warning disable 649
        static OscServer s_SharedServer;
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
            //AddPrimaryCallback(PrimaryCallback);
            m_PrimaryCallbackAdded = true;
            GetSharedServer();
        }

        void Awake()
        {
            Instance = this;
            GetSharedServer();
        }

        void Start()
        {
            // TODO - start server on its own elsewhere
            s_SharedServer.Start();
        }

        void OnDisable()
        {
            OscServer.Remove(m_Port);
        }

        void Update()
        {
            // this is a hack
            s_SharedServer.Update();
            
            // call all Actions buffered in response to messages since last frame
            m_ActionInvocationBuffer.InvokeAll();
        }

        /// <summary>
        /// Register a new pair of OSC message handlers
        /// </summary>
        /// <param name="address">The URL path to handle messages for</param>
        /// <param name="actionPair">The value read action & user callback to execute</param>
        public static void AddCallbacks(string address, OscActionPair actionPair)
        {
            if (s_SharedServer == null)
                GetSharedServer();
            if (s_SharedServer == null)
                return;
            
            s_SharedServer.AddressSpace.TryAddMethod(address, actionPair);
        }

        static void GetSharedServer()
        {
            s_SharedServer = OscServer.GetOrCreate(Instance.Port);
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
            return true;
            //return s_SharedServer != null && s_SharedServer.RemoveMethod(address, );
        }
        
        static void AddPrimaryCallback(MonitorCallback callback) { }
        static void RemovePrimaryCallback(MonitorCallback callback) { }
        
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