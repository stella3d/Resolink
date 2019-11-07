using System;
using System.Collections.Generic;
using OscCore;
using UnityEditor;
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

        /// <summary>The underlying OSC server that does the heavy lifting of routing</summary>
        public OscServer Server => s_SharedServer;

        public static OscRouter Instance { get; protected set; }
        
        void OnEnable()
        {
            Instance = this;
            GetSharedServer();
        }

        void OnDestroy()
        {
#if UNITY_EDITOR
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                return;
#endif
            OscServer.Remove(m_Port);
        }

        void OnApplicationQuit()
        {
            OscServer.Remove(m_Port);
            s_SharedServer.Dispose();
        }

        void Update()
        {
            // TODO - move this into its own component ?  not strictly necessary
            s_SharedServer.Update();
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
        /// <param name="address">The OSC address to handle messages for</param>
        /// <param name="actionPair">The value read action & user callback to execute</param>
        public static void AddCallbacks(string address, OscActionPair actionPair)
        {
            s_SharedServer?.TryAddMethodPair(address, actionPair);
        }
        
        /// <summary>
        /// Remove a previously registered OSC message handler  
        /// </summary>
        /// <param name="address">The OSC address to handle messages for</param>
        /// <param name="actionPair">The callbacks associated with this address to remove</param>
        public static bool RemoveCallbacks(string address, OscActionPair actionPair)
        {
            return s_SharedServer != null && 
                   s_SharedServer.RemoveMethodPair(address, actionPair);
        }
    }
}