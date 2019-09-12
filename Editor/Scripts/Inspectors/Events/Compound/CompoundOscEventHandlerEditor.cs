using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Resolink
{
    public abstract class CompoundOscEventHandlerEditor<TComponent, TSubHandler, TEvent, TCombinedData, TComponentData> 
        : Editor
        where TComponent: CompoundOscEventHandler<TSubHandler, TEvent, TCombinedData, TComponentData>
        where TSubHandler : OscActionHandler<TComponentData>
        where TEvent : UnityEvent<TCombinedData>, new()
    {
        const string k_PathTooltip = "The OSC address we receive messages at associated with this event";
        const string k_MethodTooltip = "The method on this script called when a message is received at this address";

        protected GUIContent[] m_PathContents;
        protected GUIContent[] m_EventContents;

        protected TComponent m_Component;
        
        protected SerializedProperty m_EventProperty;
        protected SerializedProperty m_ValueProperty;

        protected GUIStyle m_LabelStyle;
        protected GUIStyle m_MethodNameStyle;

        protected bool m_HandlersFoldoutState = true;

        public void OnEnable()
        {
            m_Component = (TComponent) target;
            m_EventProperty = serializedObject.FindProperty("Event");
            m_ValueProperty = serializedObject.FindProperty("Value");

            var handlers = m_Component.Handlers;
            if (handlers == null)
            {
                m_Component.Setup();
                return;
            }

            m_PathContents = new GUIContent[handlers.Length];
            m_EventContents = new GUIContent[handlers.Length];
            for (var i = 0; i < handlers.Length; i++)
            {
                var handler = handlers[i];
                var hasShortcut = handler.Shortcut?.Output != null;
                var label = hasShortcut ? handler.Shortcut.Output.Path : "not set";
                m_PathContents[i] = new GUIContent(label, k_PathTooltip);
                m_EventContents[i] = new GUIContent(handler.Event.Method.Name, k_MethodTooltip);
            }
        }

        public override void OnInspectorGUI()
        {
            if (m_LabelStyle == null || m_MethodNameStyle == null)
                InitStyles();
            
            serializedObject.UpdateIfRequiredOrScript();

            m_HandlersFoldoutState = EditorGUILayout.Foldout(m_HandlersFoldoutState, "Address Handlers");
            if (m_HandlersFoldoutState)
            {
                for (var i = 0; i < m_PathContents.Length; i++)
                    DrawPathWithActionName(m_PathContents[i], m_EventContents[i]);
            }

            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.PropertyField(m_ValueProperty);
            }
            
            EditorGUILayout.PropertyField(m_EventProperty);

            serializedObject.ApplyModifiedProperties();
        }

        void DrawPathWithActionName(GUIContent pathContent, GUIContent actionContent)
        {
            using (new EditorGUILayout.HorizontalScope(GUILayout.ExpandWidth(true)))
            {
                EditorGUILayout.LabelField(pathContent, m_LabelStyle, GUILayout.ExpandWidth(true));
                EditorGUILayout.LabelField(actionContent, m_MethodNameStyle, GUILayout.Width(84));
            }
        }

        void InitStyles()
        {
            m_LabelStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                stretchWidth = true
            };
            m_MethodNameStyle = new GUIStyle(EditorStyles.miniLabel)
            {
                fixedWidth = 84f, 
                alignment = TextAnchor.MiddleRight
            };
        }
    }
}