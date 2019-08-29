using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Resolunity
{
    [Serializable]
    public class EventComponentMapping : MonoBehaviour
    {
        protected ResolumeOscMap m_Map;

        public Dictionary<long, IntOscEventHandler> IdToIntEvent = new Dictionary<long, IntOscEventHandler>();
        public Dictionary<long, FloatOscEventHandler> IdToFloatEvent = new Dictionary<long, FloatOscEventHandler>();
        public Dictionary<long, BooleanOscEventHandler> IdToBoolEvent = new Dictionary<long, BooleanOscEventHandler>();

        [SerializeField]
        protected List<IntUnityEvent> m_IntEvents = new List<IntUnityEvent>();
        [SerializeField]
        protected List<FloatUnityEvent> m_FloatEvents = new List<FloatUnityEvent>();
        [SerializeField]
        protected List<FloatUnityEvent> m_BoolEvents = new List<FloatUnityEvent>();

        public GameObject gameObject;
        public GameObject IntEventsObject;
        public GameObject FloatEventsObject;
        public GameObject BoolEventsObject;
        
        public int Count => IdToFloatEvent.Count + IdToIntEvent.Count + IdToBoolEvent.Count;

        public EventComponentMapping(ResolumeOscMap map)
        {
            m_Map = map;
            Init();
            PopulateEvents();
        }

        void Init()
        {
            if (gameObject == null)
                gameObject = new GameObject("OSC Event Handlers");
        }

        public void PopulateEvents()
        {
            if (m_Map == null)
                return;
            if(gameObject == null)
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