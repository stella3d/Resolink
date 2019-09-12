using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Resolink
{
    public abstract class CompoundOscEventHandlerEditor<TComponent, TSubHandler, TEvent, TCombinedData, TComponentData> 
        : Editor
        where TComponent: CompoundOscEventHandler<TSubHandler, TEvent, TCombinedData, TComponentData>
        where TSubHandler : OscActionEventHandler<TComponentData>
        where TEvent : UnityEvent<TCombinedData>, new()
    {
        const string k_PathTooltip = "The OSC address we receive messages at associated with this event";

        protected GUIContent[] m_PathContents;
        protected GUIContent[] m_EventContents;

        protected TComponent m_Component;
        
        protected SerializedProperty m_EventProperty;

        protected GUIStyle m_LabelStyle;
        protected GUIStyle m_MethodNameStyle;

        public void OnEnable()
        {
            m_Component = (TComponent) target;
            m_EventProperty = serializedObject.FindProperty("Event");

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
                m_EventContents[i] = new GUIContent(handler.Event.Method.Name);
            }
        }

        public override void OnInspectorGUI()
        {
            if (m_LabelStyle == null || m_MethodNameStyle == null)
                InitStyles();
            
            serializedObject.UpdateIfRequiredOrScript();
            for (var i = 0; i < m_PathContents.Length; i++)
            {
                var contents = m_PathContents[i];
                var action = m_EventContents[i];
                DrawPathWithActionName(contents, action);
                //EditorGUILayout.LabelField(contents, m_LabelStyle);
            }

            EditorGUILayout.PropertyField(m_EventProperty);
            serializedObject.ApplyModifiedProperties();
        }

        void DrawPathWithActionName(GUIContent pathContent, GUIContent actionContent)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField(pathContent, m_LabelStyle);
                EditorGUILayout.LabelField(actionContent, m_MethodNameStyle);
            }
        }

        void InitStyles()
        {
            m_LabelStyle = new GUIStyle(EditorStyles.boldLabel);
            m_MethodNameStyle = new GUIStyle(EditorStyles.miniLabel)
            {
                fixedWidth = 96f, 
                alignment = TextAnchor.MiddleRight
            };
        }
    }
}