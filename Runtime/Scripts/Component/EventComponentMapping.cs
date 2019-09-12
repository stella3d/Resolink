using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Resolink
{
    [DisallowMultipleComponent]
    [ExecuteAlways]
    public class EventComponentMapping : MonoBehaviour
    {
        static readonly List<IntOscEventHandler> k_IntHandlerComponents = new List<IntOscEventHandler>();
        static readonly List<FloatOscEventHandler> k_FloatHandlerComponents = new List<FloatOscEventHandler>();
        static readonly List<BooleanOscEventHandler> k_BoolHandlerComponents = new List<BooleanOscEventHandler>();
        static readonly List<StringOscEventHandler> k_StringHandlerComponents = new List<StringOscEventHandler>();
        
        static readonly List<ColorOscEventHandler> k_ColorHandlerComponents = new List<ColorOscEventHandler>();
        
        [Tooltip("A map of OSC events parsed from Resolume")]
        public ResolumeOscMap OscMap;

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
        [Tooltip(k_EventsRelated + "effects on any or all layers in the Resolume composition")]
        public GameObject CompositionLayerEffects;
        [Tooltip(k_EventsRelated + "the Resolume composition dashboard")]
        public GameObject CompositionDashboard;
        [Tooltip(k_EventsRelated + "the any or all layer-specific dashboards" + k_Suffix)]
        public GameObject CompositionLayerDashboard;
        [Tooltip(k_EventsRelated + "the Resolume application UI, not the composition")]
        public GameObject ApplicationUI;
        
        public void OnEnable()
        {
            HandleObjectDisableStates();
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

            RemoveUnusedPrevious();

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
            
            Debug.Log($"{OscMap.ColorGroups.Count} color groups in map");
            foreach (var group in OscMap.ColorGroups)
            {
                var red = group.Red;
                var go = ObjectForShortcut(red);
                if (go == null) 
                    continue;
                
                AddColorComponentIfAbsent(go, group, k_ColorHandlerComponents);
                count += 4;
            }


            HandleObjectDisableStates();
        }

        GameObject ObjectForShortcut(ResolumeOscShortcut shortcut)
        {
            if (shortcut.IsTimeEvent())
                return TempoController;
            if (shortcut.IsClipTransportEvent())
                return ClipTransport;
            if (shortcut.IsCueEvent())
                return ClipCuepoints;
            if (shortcut.IsLayerEffectEvent())
                return CompositionLayerEffects;
            if (shortcut.IsLayerEvent())
                return CompositionLayer;
            if (shortcut.IsCompositionDashboardEvent())
                return CompositionDashboard;
            if (shortcut.IsCompositionLayerDashboardEvent())
                return CompositionLayerDashboard;
            if (shortcut.IsCompositionEvent())
                return Composition;
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
        
        void AddColorComponentIfAbsent(GameObject go, ColorShortcutGroup group, 
            List<ColorOscEventHandler> components) 
        {
            go.GetComponents(components);
            var found = false;
            ColorOscEventHandler component = null;

            foreach (var c in components)
            {
                if (c.Handlers == null)
                    continue;
                if (c.Handlers[0].Shortcut != group.Red)
                    continue;
                if (c.Handlers[1].Shortcut != group.Green)
                    continue;
                if (c.Handlers[2].Shortcut != group.Blue)
                    continue;
                if (c.Handlers[3].Shortcut != group.Alpha)
                    continue;

                found = true;
                break;
            }

            if (!found)
            {
                component = go.AddComponent<ColorOscEventHandler>();
                component.Setup();
            }

            component.Handlers[0].Shortcut = group.Red;
            component.Handlers[1].Shortcut = group.Green;
            component.Handlers[2].Shortcut = group.Blue;
            component.Handlers[3].Shortcut = group.Alpha;
            
            Debug.Log($"successfully setup color component on object {go.name}");
        }
        
        static void DisableIfNoHandlers(GameObject go)
        {
            go.SetActive(go.GetComponent<OscEventHandler>() != null);
        }

        void RemoveUnusedPrevious()
        {
            RemoveUnusedPrevious<IntOscEventHandler, IntUnityEvent, int>(OscMap, gameObject, k_IntHandlerComponents);
            RemoveUnusedPrevious<FloatOscEventHandler, FloatUnityEvent, float>(OscMap, gameObject, k_FloatHandlerComponents);
            RemoveUnusedPrevious<BooleanOscEventHandler, BoolUnityEvent, bool>(OscMap, gameObject, k_BoolHandlerComponents);
            RemoveUnusedPrevious<StringOscEventHandler, StringUnityEvent, string>(OscMap, gameObject, k_StringHandlerComponents);
        }

        static void RemoveUnusedPrevious<THandler, TEvent, TData>(ResolumeOscMap map, 
            GameObject go, List<THandler> components)
            where TEvent : UnityEvent<TData>, new()
            where THandler : OscEventHandler<TEvent, TData>
        {
            go.GetComponents(components);
            foreach (var component in components)
            {
                var found = false;
                foreach (var shortcut in map.Shortcuts)
                {
                    if (shortcut != component.Shortcut)
                        continue;

                    found = true;
                    break;
                }

                // only destroy events not found in the current map that have 0 event listeners
                if (!found && component.Event.GetPersistentEventCount() == 0) 
                    Destroy(component);
            }
        }

        void HandleObjectDisableStates()
        {
            DisableIfNoHandlers(TempoController);
            DisableIfNoHandlers(ClipTransport);
            DisableIfNoHandlers(ClipCuepoints);
            DisableIfNoHandlers(Composition);
            DisableIfNoHandlers(CompositionDashboard);
            DisableIfNoHandlers(CompositionLayer);
            DisableIfNoHandlers(CompositionLayerDashboard);
            DisableIfNoHandlers(CompositionLayerEffects);
            DisableIfNoHandlers(ApplicationUI);
        }
    }
}