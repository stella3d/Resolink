using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Resolink
{
    [DisallowMultipleComponent]
    [ExecuteAlways]
    public class EventComponentMapping : MonoBehaviour
    {
        static readonly List<IntResolumeShortcutHandler> k_IntHandlerComponents = new List<IntResolumeShortcutHandler>();
        static readonly List<FloatResolumeShortcutHandler> k_FloatHandlerComponents = new List<FloatResolumeShortcutHandler>();
        static readonly List<BooleanResolumeShortcutHandler> k_BoolHandlerComponents = new List<BooleanResolumeShortcutHandler>();
        static readonly List<StringResolumeShortcutHandler> k_StringHandlerComponents = new List<StringResolumeShortcutHandler>();
        
        static readonly List<ColorOscEventHandler> k_ColorHandlerComponents = new List<ColorOscEventHandler>();
        static readonly List<Vector2OscEventHandler> k_Vector2HandlerComponents = new List<Vector2OscEventHandler>();
        static readonly List<Vector3OscEventHandler> k_Vector3HandlerComponents = new List<Vector3OscEventHandler>();
        static readonly List<RotationOscEventHandler> k_RotationHandlerComponents = new List<RotationOscEventHandler>();
        
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
#if UNITY_EDITOR
            HandleObjectDisableStates();
#endif
        }

        public void PopulateEvents()
        {
            if (OscMap == null)
            {
                Debug.LogWarning("Cannot generate components without an OSC map asset!");
                return;
            }

            RemoveUnusedPrevious();

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
            }
            
            foreach (var group in OscMap.ColorGroups)
            {
                var go = ObjectForShortcut(group.Red);
                if (go == null) continue;
                AddColorComponentIfAbsent(go, group, k_ColorHandlerComponents);
            }
            
            foreach (var group in OscMap.Vector2Groups)
            {
                var go = ObjectForShortcut(group.X);
                if (go == null) continue;
                AddVector2ComponentIfAbsent(go, group, k_Vector2HandlerComponents);
            }
            
            foreach (var group in OscMap.Vector3Groups)
            {
                var go = ObjectForShortcut(group.X);
                if (go == null) continue;
                
                // translate Vector3 rotation controls into Quaternion ones
                if (Regexes.Rotations.All.MatchesAny(group.X.Input.Path))
                    AddRotationComponentIfAbsent(go, group, k_RotationHandlerComponents);
                else
                    AddVector3ComponentIfAbsent(go, group, k_Vector3HandlerComponents);
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
        
        static void AddShortcutComponentIfAbsent<T>(GameObject go, ResolumeOscShortcut shortcut, List<T> components) 
            where T: ResolumeShortcutHandler
        {
            go.GetComponents(components);
            var found = false;
            foreach (var c in components)
            {
                if (c.Shortcut.Input.Path != shortcut.Input.Path)
                    continue;

                c.Shortcut = shortcut;
                found = true;
                break;
            }

            if (!found)
            {
                go.SetActive(false);
                var component = go.AddComponent<T>();
                component.Shortcut = shortcut;
                go.SetActive(true);
            }
        }
        
        static void AddColorComponentIfAbsent(GameObject go, ColorShortcutGroup group, 
            List<ColorOscEventHandler> components) 
        {
            go.GetComponents(components);
            var found = false;
            foreach (var c in components)
            {
                if (c.Handlers == null) continue;
                if (c.Handlers[0].Shortcut != group.Red) continue;
                if (c.Handlers[1].Shortcut != group.Green) continue;
                if (c.Handlers[2].Shortcut != group.Blue) continue;
                if (c.Handlers[3].Shortcut != group.Alpha) continue;
                found = true;
                break;
            }

            if (!found)
            {
                var component = go.AddComponent<ColorOscEventHandler>();
                component.Setup();
                component.SetHandlers(group);
            }
        }
        
        static void AddVector2ComponentIfAbsent(GameObject go, Vector2ShortcutGroup group, 
            List<Vector2OscEventHandler> components) 
        {
            go.GetComponents(components);
            var found = false;
            foreach (var c in components)
            {
                if (c.Handlers == null) continue;
                if (c.Handlers[0].Shortcut != group.X) continue;
                if (c.Handlers[1].Shortcut != group.Y) continue;
                found = true;
                break;
            }

            if (!found)
            {
                var component = go.AddComponent<Vector2OscEventHandler>();
                component.Setup();
                component.SetHandlers(group);
            }
        }
        
        static void AddVector3ComponentIfAbsent(GameObject go, Vector3ShortcutGroup group, 
            List<Vector3OscEventHandler> components) 
        {
            go.GetComponents(components);
            var found = false;
            foreach (var c in components)
            {
                if (c.Handlers == null) continue;
                if (c.Handlers[0].Shortcut != group.X) continue;
                if (c.Handlers[1].Shortcut != group.Y) continue;
                if (c.Handlers[2].Shortcut != group.Z) continue;
                found = true;
                break;
            }

            if (!found)
            {
                var component = go.AddComponent<Vector3OscEventHandler>();
                component.Setup();
                component.SetHandlers(group);
            }
        }

        static void AddRotationComponentIfAbsent(GameObject go, Vector3ShortcutGroup group, 
            List<RotationOscEventHandler> components) 
        {
            go.GetComponents(components);
            var found = false;
            foreach (var c in components)
            {
                if (c.Handlers == null) continue;
                if (c.Handlers[0].Shortcut != group.X) continue;
                if (c.Handlers[1].Shortcut != group.Y) continue;
                if (c.Handlers[2].Shortcut != group.Z) continue;
                found = true;
                break;
            }

            if (!found)
            {
                var component = go.AddComponent<RotationOscEventHandler>();
                component.Setup();
                component.SetHandlers(group);
            }
        }
        
        static void DisableIfNoHandlers(GameObject go)
        {
            if (go.GetComponent<ResolumeShortcutHandler>() != null)
                go.SetActive(true);
            else if(go.GetComponent<CompoundOscEventHandler>() != null)
                go.SetActive(true);
            else
                go.SetActive(false);
        }

        void RemoveUnusedPrevious()
        {
            RemoveUnusedPrevious<IntResolumeShortcutHandler, IntUnityEvent, int>(OscMap, gameObject, k_IntHandlerComponents);
            RemoveUnusedPrevious<FloatResolumeShortcutHandler, FloatUnityEvent, float>(OscMap, gameObject, k_FloatHandlerComponents);
            RemoveUnusedPrevious<BooleanResolumeShortcutHandler, BoolUnityEvent, bool>(OscMap, gameObject, k_BoolHandlerComponents);
            RemoveUnusedPrevious<StringResolumeShortcutHandler, StringUnityEvent, string>(OscMap, gameObject, k_StringHandlerComponents);
            
            // TODO - this, for compound events
        }

        static void RemoveUnusedPrevious<THandler, TEvent, TData>(ResolumeOscMap map, 
            GameObject go, List<THandler> components)
            where TEvent : UnityEvent<TData>, new()
            where THandler : ResolumeShortcutHandler<TEvent, TData>
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
            //DisableIfNoHandlers(Composition);
            DisableIfNoHandlers(CompositionDashboard);
            //DisableIfNoHandlers(CompositionLayer);
            DisableIfNoHandlers(CompositionLayerDashboard);
            DisableIfNoHandlers(CompositionLayerEffects);
            DisableIfNoHandlers(ApplicationUI);
        }
    }
}