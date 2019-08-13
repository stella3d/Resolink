using System.Collections.Generic;
using OscJack;
using UnityEngine;

namespace UnityResolume
{
    public class OscBrain : MonoBehaviour
    {
        readonly HashSet<OscServer> m_KnownServers = new HashSet<OscServer>();

        readonly Dictionary<string, OscMessageDispatcher.MessageCallback> m_Handlers =
            new Dictionary<string, OscMessageDispatcher.MessageCallback>();

        static OscBrain Instance;

        void OnEnable()
        {
            Instance = this;
        }

        void Update()
        {
            SetCallbacks();
        }

        void SetCallbacks()
        {
            foreach (var server in OscServer.ServerList)
            {
                if (m_KnownServers.Contains(server))
                    continue;

                foreach (var kvp in m_Handlers)
                {
                    server.MessageDispatcher.AddCallback(kvp.Key, kvp.Value);
                }
            }

            m_KnownServers.Clear();
            foreach (var server in OscServer.ServerList)
            {
                m_KnownServers.Add(server);
            }
        }

        public static void AddCallback(string address, OscMessageDispatcher.MessageCallback callback)
        {
            if (Instance.m_Handlers.TryGetValue(address, out var existingCallback))
            {
                foreach (var server in OscServer.ServerList)
                {
                    server.MessageDispatcher.RemoveCallback(address, existingCallback);
                    server.MessageDispatcher.AddCallback(address, callback);
                }
            }
            else
            {
                Instance.m_Handlers[address] = callback;
                foreach (var server in OscServer.ServerList)
                {
                    server.MessageDispatcher.AddCallback(address, callback);
                }
            }
        }

        public static bool RemoveCallback(string address)
        {
            if (!Instance.m_Handlers.TryGetValue(address, out var existingCallback))
                return false;

            foreach (var server in OscServer.ServerList)
            {
                server.MessageDispatcher.RemoveCallback(address, existingCallback);
            }

            return true;
        }
    }
}