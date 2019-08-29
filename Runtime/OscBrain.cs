using System;
using System.Collections.Generic;
using OscJack;
using UnityEngine;

namespace Resolunity
{
    [ExecuteAlways]
    public class OscBrain : MonoBehaviour
    {
        public class ActionBuffer<T>
        {
            const int k_DefaultCapacity = 32;
            
            public Action<T>[] Actions;
            public T[] Values;

            public int Count { get; protected set; }

            public ActionBuffer(int capacity = k_DefaultCapacity)
            {
                Actions = new Action<T>[capacity];
                Values = new T[capacity];
            }

            public void Add(Action<T> action, T value)
            {
                if (Count >= Actions.Length)
                {
                    Array.Resize(ref Actions, Actions.Length * 2);
                    Array.Resize(ref Actions, Actions.Length * 2);
                }

                Actions[Count] = action;
                Values[Count] = value;
                Count++;
            }

            public void InvokeAll()
            {
                for (int i = 0; i < Count; i++)
                    Actions[i](Values[i]);

                Count = 0;
            }
        }

        readonly HashSet<OscServer> m_KnownServers = new HashSet<OscServer>();
        
        internal readonly Dictionary<string, List<Action<OscDataHandle>>> m_AddressHandlers = 
            new Dictionary<string, List<Action<OscDataHandle>>>();

        static OscBrain Instance;
        
        ActionBuffer<OscDataHandle> m_ActionBuffer = new ActionBuffer<OscDataHandle>();

        bool m_PrimaryCallbackAdded;
        public bool EnableLogging;

        void OnEnable()
        {
            Instance = this;
            
            AddPrimaryCallback(PrimaryCallback);
            m_PrimaryCallbackAdded = true;
        }

        void Start()
        {
            if (!m_PrimaryCallbackAdded)
            {
                AddPrimaryCallback(PrimaryCallback);
                m_PrimaryCallbackAdded = false;
            }
        }

        void OnDisable()
        {
            m_PrimaryCallbackAdded = false;
            RemovePrimaryCallback(PrimaryCallback);
        }

        void Update()
        {
            if (m_ActionBuffer.Count > 0)
                m_ActionBuffer.InvokeAll();

            SetCallbacks();
        }

        void SetCallbacks()
        {
            foreach (var server in OscServer.ServerList)
            {
                if (m_KnownServers.Contains(server))
                    continue;

                server.MessageDispatcher.AddCallback(string.Empty, PrimaryCallback);
                m_KnownServers.Add(server);
            }
        }
        
        public static void AddCallback(string address, Action<OscDataHandle> callback)
        {
            if (!Instance.m_AddressHandlers.TryGetValue(address, out var callbackList))
            {
                callbackList = new List<Action<OscDataHandle>>();
                Instance.m_AddressHandlers[address] = callbackList;
            }
            
            callbackList.Add(callback);
        }
        
        public static void RemoveCallback(string address, Action<OscDataHandle> callback)
        {
            if (Instance.m_AddressHandlers.TryGetValue(address, out var callbackList))
                callbackList.Remove(callback);
        }

        public static void AddPrimaryCallback(OscMessageDispatcher.MessageCallback callback)
        {
            foreach (var server in OscServer.ServerList)
            {
                server.MessageDispatcher.AddCallback(string.Empty, callback);
            }
        }

        public static void RemovePrimaryCallback(OscMessageDispatcher.MessageCallback callback)
        {
            foreach (var server in OscServer.ServerList)
            {
                server.MessageDispatcher.RemoveCallback(string.Empty, callback);
            }
        }

        // This is the message handler for every OSC message we receive. 
        protected void PrimaryCallback(string address, OscDataHandle handle)
        {
            if(EnableLogging)
                Debug.Log(address + " " + handle.GetElementAsString(0));
            
            if (m_AddressHandlers.TryGetValue(address, out var callbackList))
            {
                foreach (var callback in callbackList)
                {
                    m_ActionBuffer.Add(callback, handle);
                }
            }
        }

        public void SetTimeScale(float scale)
        {
            Debug.Log("set timescale");
            Time.timeScale = scale * 4f;
        }
    }
}