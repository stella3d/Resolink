using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Resolink
{
    public abstract class OscEventHandlerEditor<TComponent, TEvent, T> : Editor
        where TComponent : OscEventHandler<TEvent, T>
        where TEvent : UnityEvent<T>, new()
        where T: IEquatable<T>
    {
        const string k_PathTooltip = "The OSC address we receive messages at associated with this event";
        const string k_InputPathTooltip = "The OSC address Resolume receives messages at for this control";

        protected GUIContent m_PathContent;
        protected GUIContent m_InputPathContent;
        
        protected TComponent m_Component;

        protected SerializedProperty m_EventProperty;

        protected GUIStyle m_LabelStyle;

        public void OnEnable()
        {
            m_Component = (TComponent) target;
            m_EventProperty = serializedObject.FindProperty("Event");
            m_PathContent = new GUIContent(m_Component.Shortcut.Output.Path, k_PathTooltip);
            m_InputPathContent = new GUIContent(m_Component.Shortcut.Input?.Path, k_InputPathTooltip);
        }

        public override void OnInspectorGUI()
        {
            if (m_LabelStyle == null)
                InitHeaderStyle();
            
            serializedObject.UpdateIfRequiredOrScript();
            
            EditorGUILayout.LabelField(m_PathContent, m_LabelStyle);
            EditorGUILayout.PropertyField(m_EventProperty);

            EditorUtils.DrawBoxLine();
            
            EditorGUILayout.LabelField(m_InputPathContent, EditorStyles.largeLabel);

            DrawValue();

            if(GUILayout.Button("Send Debug Value"))
                m_Component.SendValue();
            
            serializedObject.ApplyModifiedProperties();
        }

        void InitHeaderStyle()
        {
            m_LabelStyle = new GUIStyle(EditorStyles.boldLabel) { wordWrap = true, clipping = TextClipping.Clip };
        }

        protected abstract void DrawValue();
    }
}