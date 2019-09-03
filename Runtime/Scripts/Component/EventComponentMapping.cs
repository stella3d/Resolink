using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace Resolink
{
    [Serializable]
    public abstract class SerializableDictionary<TKey, TValue>
    {
        public List<TKey> Keys = new List<TKey>();
        public List<TValue> Values = new List<TValue>();

        public void Add(TKey key, TValue value)
        {
            Keys.Add(key);
            Values.Add(value);
        }

        public void Clear()
        {
            Keys.Clear();
            Values.Clear();
        }

        Dictionary<TKey, TValue> ToDictionary()
        {
            var dictionary = new Dictionary<TKey, TValue>();
            for (var i = 0; i < Keys.Count; i++)
                dictionary.Add(Keys[i], Values[i]);

            return dictionary;
        }
    }

    public class SerializableEventDictionary<TKey, TUnityEvent> : SerializableDictionary<TKey, TUnityEvent>
        where TUnityEvent : UnityEventBase
    {
    }

    public class UniqueIdEventDictionary<TEvent> : SerializableEventDictionary<string, TEvent> 
        where TEvent : UnityEventBase
    {
        public static UniqueIdEventDictionary<TEvent> FromDictionary(Dictionary<string, TEvent> dictionary)
        {
            var self = new UniqueIdEventDictionary<TEvent>();
            foreach (var kvp in dictionary)
                self.Add(kvp.Key, kvp.Value);

            return self;
        }
    }

    [Serializable]
    public class EventComponentMapping : MonoBehaviour
    {
        
        public ResolumeOscMap OscMap;

        public Dictionary<string, IntOscEventHandler> IdToIntEvent;
        public Dictionary<string, FloatOscEventHandler> IdToFloatEvent;
        public Dictionary<string, BooleanOscEventHandler> IdToBoolEvent;

        [SerializeField]
        protected UniqueIdEventDictionary<IntUnityEvent> m_IntEvents = new UniqueIdEventDictionary<IntUnityEvent>();
        [SerializeField]
        protected UniqueIdEventDictionary<FloatUnityEvent> m_FloatEvents = new UniqueIdEventDictionary<FloatUnityEvent>();
        [SerializeField]
        protected UniqueIdEventDictionary<BoolUnityEvent> m_BoolEvents = new UniqueIdEventDictionary<BoolUnityEvent>();

        [HideInInspector] public GameObject TimeEventsObject;
        [HideInInspector] public GameObject IntEventsObject;
        [HideInInspector] public GameObject FloatEventsObject;
        [HideInInspector] public GameObject BoolEventsObject;
        
        [Header("Event Component Objects")]
        public GameObject TempoController;
        public GameObject ClipTransport;
        public GameObject Composition;
        public GameObject CompositionLayer;
        public GameObject CompositionDashboard;
        public GameObject CompositionLayerDashboard;
        public GameObject ApplicationUI;
        
        public static Dictionary<string, Type> InputPathToEventType { get; set; }
        
        public int Count => IdToFloatEvent.Count + IdToIntEvent.Count + IdToBoolEvent.Count;

        public void OnEnable()
        {
            InitializeDictionaries();
        }

        void InitializeDictionaries()
        {
            IdToIntEvent = new Dictionary<string, IntOscEventHandler>();
            IdToFloatEvent = new Dictionary<string, FloatOscEventHandler>();
            IdToBoolEvent = new Dictionary<string, BooleanOscEventHandler>();
        }

        void InitializeSerializableDictionaries()
        {
            m_IntEvents = new UniqueIdEventDictionary<IntUnityEvent>();
            m_FloatEvents = new UniqueIdEventDictionary<FloatUnityEvent>();
            m_BoolEvents = new UniqueIdEventDictionary<BoolUnityEvent>();
        }

        public EventComponentMapping(ResolumeOscMap map)
        {
            OscMap = map;
            PopulateEvents();
        }

        public void PopulateEvents()
        {
            if (OscMap == null)
                return;
            if(IdToIntEvent == null)
                InitializeDictionaries();
            if (m_IntEvents == null)
                InitializeSerializableDictionaries();

            var count = 0;
            foreach (var shortcut in OscMap.Shortcuts)
            {
                var go = ObjectForShortcut(shortcut);
                if (go == null) 
                    continue;
                
                ComponentForShortcut(go, shortcut);
                count++;
            }
            
            // Debug.LogFormat("{0} event handlers populated", count);
        }

        GameObject ObjectForShortcut(ResolumeOscShortcut shortcut)
        {
            if (shortcut.IsTimeEvent())
                return TempoController;
            // TODO - if is clip transport event
            // TODO - if is cuepoint 
            if (shortcut.IsCompositionEvent())
                return Composition;
            if (shortcut.IsLayerEvent())
                return CompositionLayer;
            if (shortcut.IsCompositionDashboardEvent())
                return CompositionDashboard;
            if (shortcut.IsCompositionLayerDashboardEvent())
                return CompositionLayerDashboard;
            if (shortcut.IsApplicationUiEvent())
                return ApplicationUI;

            return null;
        }

        OscEventHandler ComponentForShortcut(GameObject go, ResolumeOscShortcut shortcut)
        {
            OscEventHandler component = null;
            if (shortcut.TypeName == typeof(int).Name)
                component = go.AddComponent<IntOscEventHandler>();
            else if (shortcut.TypeName == typeof(float).Name)
                component = go.AddComponent<FloatOscEventHandler>();
            else if (shortcut.TypeName == typeof(bool).Name)
                component = go.AddComponent<BooleanOscEventHandler>();
            else if (shortcut.TypeName == typeof(string).Name)
                component = go.AddComponent<StringOscEventHandler>();

            if (component != null)
                component.Shortcut = shortcut;

            return component;
        }
    }
}