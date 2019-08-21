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

        public Dictionary<long, IntEvent> IdToIntEvent = new Dictionary<long, IntEvent>();
        public Dictionary<long, FloatEvent> IdToFloatEvent = new Dictionary<long, FloatEvent>();

        [SerializeField]
        protected List<IntEvent> m_IntEvents = new List<IntEvent>();
        [SerializeField]
        protected List<FloatEvent> m_FloatEvents = new List<FloatEvent>();

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
            PopulateEvents();
        }

        public void PopulateEvents()
        {
            if (m_Map == null)
                return;
            
            foreach (var shortcut in m_Map.Shortcuts)
            {
                var id = shortcut.UniqueId;
                if (!IdToIntEvent.ContainsKey(id))
                    IdToIntEvent.Add(id, new IntEvent());
                else if (!IdToFloatEvent.ContainsKey(id))
                    IdToFloatEvent.Add(id, new FloatEvent());
            }
            
            Debug.LogFormat("{0} blank event handlers populated", Count);
        }
    }
}