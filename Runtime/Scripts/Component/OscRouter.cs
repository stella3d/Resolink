using System;
using OscCore;
using UnityEngine;
using OscServer = OscCore.OscServer; 

namespace Resolink
{
    /// <summary>
    /// Handles routing messages to the callbacks registered for each address
    /// </summary>
    [ExecuteAlways]
    public class OscRouter : MonoBehaviour
    {
#pragma warning disable 649
        static OscServer s_SharedServer;
#pragma warning restore 649
        static int s_CallbackAddIndex;

        [SerializeField] 
        int m_Port = 9000;
        
        public int Port => m_Port;

        public int HandlerCount;

        public OscServer Server => s_SharedServer;
        
        public static OscRouter Instance { get; protected set; }
        
        void OnEnable()
        {
            Instance = this;
            GetSharedServer();
            s_SharedServer.Start();
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
            s_SharedServer.Dispose();
        }
        
        void Update()
        {
            // TODO - move this into its own component
            s_SharedServer.Update();
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
            
            s_SharedServer.TryAddMethod(address, actionPair);
        }

        static void GetSharedServer()
        {
            if (Instance == null)
                return;
                
            s_SharedServer = OscServer.GetOrCreate(Instance.Port);
        }

        /// <summary>
        /// Register a new pair of OSC message handlers
        /// </summary>
        /// <param name="address">The URL path to handle messages for</param>
        /// <param name="valueRead">The value read action to execute immediately on the worker thread</param>
        /// <param name="userCallback">OPTIONAL - The user callback to queue for execution on the main thread</param>
        public static void AddCallbacks(string address, Action<OscMessageValues> valueRead, Action userCallback = null)
        {
            AddCallbacks(address, new OscActionPair(valueRead, userCallback));
        }
        
        /// <summary>
        /// Remove a previously registered OSC message handler  
        /// </summary>
        /// <param name="address">The URL path to stop handling messages for</param>
        /// <param name="valueRead">The value read action to execute immediately on the worker thread</param>
        /// <param name="userCallback">The user callback to queue for execution on the main thread</param>
        public static bool RemoveCallbacks(string address, Action<OscMessageValues> valueRead, Action userCallback = null)
        {
            return s_SharedServer != null && 
                   s_SharedServer.RemoveMethod(address, new OscActionPair(valueRead, userCallback));
        }
        
        /// <summary>
        /// Remove a previously registered OSC message handler  
        /// </summary>
        /// <param name="address">The URL path to stop handling messages for</param>
        /// <param name="actionPair">The callbacks associated with this address to remove</param>
        public static bool RemoveCallbacks(string address, OscActionPair actionPair)
        {
            return s_SharedServer != null && 
                   s_SharedServer.RemoveMethod(address, actionPair);
        }
    }
}