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
        static readonly List<IntOscEventHandler> k_IntHandlerComponents = new List<IntOscEventHandler>();
        static readonly List<FloatOscEventHandler> k_FloatHandlerComponents = new List<FloatOscEventHandler>();
        static readonly List<BooleanOscEventHandler> k_BoolHandlerComponents = new List<BooleanOscEventHandler>();
        static readonly List<StringOscEventHandler> k_StringHandlerComponents = new List<StringOscEventHandler>();
        
        [Tooltip("A map of OSC events parsed from Resolume")]
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

        const string k_Suffix = " in Resolume";
        const string k_EventsRelated = "Events related to ";
        
        [Tooltip(k_EventsRelated + "BPM" + k_Suffix)]
        public GameObject TempoController;
        [Tooltip(k_EventsRelated + "clip transport" + k_Suffix)]
        public GameObject ClipTransport;
        [Tooltip(k_EventsRelated + "clip cue points" + k_Suffix)]
        public GameObject ClipCuepoints;
        [Tooltip(k_EventsRelated + "the Resolume composition, but not associated with a layer")]
        public GameObject Composition;
        [Tooltip(k_EventsRelated + "any or all layers in the Resolume composition")]
        public GameObject CompositionLayer;
        [Tooltip(k_EventsRelated + "the Resolume composition dashboard")]
        public GameObject CompositionDashboard;
        [Tooltip(k_EventsRelated + "the any or all layer-specific dashboards" + k_Suffix)]
        public GameObject CompositionLayerDashboard;
        [Tooltip(k_EventsRelated + "the Resolume application UI, not the composition")]
        public GameObject ApplicationUI;
        
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
            {
                Debug.LogWarning("Cannot generate components without an OSC map asset!");
                return;
            }

            if(IdToIntEvent == null)
                InitializeDictionaries();
            if (m_IntEvents == null)
                InitializeSerializableDictionaries();

            var count = 0;
            foreach (var shortcut in OscMap.Shortcuts)
            {
                var output = shortcut.Output;
                // If the shortcut doesn't output from Resolume, we can't make an event to receive for it.
                if (output == null || string.IsNullOrEmpty(output.Path))
                    continue;
                
                var go = ObjectForShortcut(shortcut);
                if (go == null) 
                    continue;
                
                ComponentForShortcut(go, shortcut);
                count++;
            }
        }

        GameObject ObjectForShortcut(ResolumeOscShortcut shortcut)
        {
            if (shortcut.IsTimeEvent())
                return TempoController;
            if (shortcut.IsClipTransportEvent())
                return ClipTransport;
            if (shortcut.IsCueEvent())
                return ClipCuepoints;
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

        void ComponentForShortcut(GameObject go, ResolumeOscShortcut shortcut)
        {
            if (shortcut.TypeName == typeof(int).Name)
                AddShortcutComponentIfAbsent(go, shortcut, k_IntHandlerComponents);
            else if (shortcut.TypeName == typeof(float).Name)
                AddShortcutComponentIfAbsent(go, shortcut, k_FloatHandlerComponents);
            else if (shortcut.TypeName == typeof(bool).Name)
                AddShortcutComponentIfAbsent(go, shortcut, k_BoolHandlerComponents);
            else if (shortcut.TypeName == typeof(string).Name)
                AddShortcutComponentIfAbsent(go, shortcut, k_StringHandlerComponents);
        }
        
        void AddShortcutComponentIfAbsent<T>(GameObject go, ResolumeOscShortcut shortcut, List<T> components) 
            where T: OscEventHandler
        {
            go.GetComponents(components);
            var found = false;
            T component = null;
            foreach (var c in components)
            {
                if (c.Shortcut.Input.Path != shortcut.Input.Path)
                    continue;

                found = true;
                break;
            }

            if (!found)
                component = go.AddComponent<T>();

            if(component != null)
                component.Shortcut = shortcut;
        }
    }
}