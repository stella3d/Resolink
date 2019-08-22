using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace UnityResolume
{
    [Serializable]
    public class OscMapEvents
    {
        protected ResolumeOscMap m_Map;

        public Dictionary<long, IntOscEventHandler> IdToIntEvent = new Dictionary<long, IntOscEventHandler>();
        public Dictionary<long, FloatOscEventHandler> IdToFloatEvent = new Dictionary<long, FloatOscEventHandler>();

        [SerializeField]
        protected List<IntEvent> m_IntEvents = new List<IntEvent>();
        [SerializeField]
        protected List<FloatEvent> m_FloatEvents = new List<FloatEvent>();

        public GameObject gameObject;
        public GameObject IntEventsObject;
        public GameObject FloatEventsObject;
        
        public int Count
        {
            get
            {
                var count = 0;
                if (IdToFloatEvent != null)
                    count += IdToFloatEvent.Count;
                if (IdToIntEvent != null)
                    count += IdToIntEvent.Count;
                
                return count;
            }
        }

        public OscMapEvents(ResolumeOscMap map)
        {
            m_Map = map;
            Init();
            PopulateEvents();
        }

        void Init()
        {
            if (gameObject == null)
            {
                gameObject = new GameObject("OSC Event Handlers");
            }

            IdToIntEvent = new Dictionary<long, IntOscEventHandler>();
            IdToFloatEvent = new Dictionary<long, FloatOscEventHandler>();
        }

        public void PopulateEvents()
        {
            if (m_Map == null)
                return;
            
            if(IdToFloatEvent == null || IdToIntEvent == null || gameObject == null)
                Init();
            
            foreach (var shortcut in m_Map.Shortcuts)
            {
                var id = shortcut.UniqueId;
                if (!IdToIntEvent.ContainsKey(id))
                {
                    var intComponent = gameObject.AddComponent<IntOscEventHandler>();
                    intComponent.Shortcut = shortcut;
                    IdToIntEvent.Add(id, intComponent);
                }
                else if (!IdToFloatEvent.ContainsKey(id))
                {
                    var floatComponent = gameObject.AddComponent<FloatOscEventHandler>();
                    floatComponent.Shortcut = shortcut;
                    IdToFloatEvent.Add(id, floatComponent);
                }
            }
            
            Debug.LogFormat("{0} blank event handlers populated", Count);
        }
    }
}