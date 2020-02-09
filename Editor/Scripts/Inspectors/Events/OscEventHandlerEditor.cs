using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Resolink
{
    public abstract class OscEventHandlerEditor<TComponent, TEvent, T> : Editor
        where TComponent : ResolumeShortcutHandler<TEvent, T>
        where TEvent : UnityEvent<T>, new()
    {
        const string k_PathTooltip = "The OSC address we receive messages at associated with this event";

        protected GUIContent m_PathContent;
        
        protected TComponent m_Component;

        protected SerializedProperty m_EventProperty;

        protected GUIStyle m_LabelStyle;

        public void OnEnable()
        {
            m_Component = (TComponent) target;
            m_EventProperty = serializedObject.FindProperty("Event");
            m_PathContent = new GUIContent(m_Component.Shortcut.Output.Path, k_PathTooltip);
        }

        public override void OnInspectorGUI()
        {
            if (m_LabelStyle == null)
                InitHeaderStyle();
            
            serializedObject.UpdateIfRequiredOrScript();
            EditorGUILayout.LabelField(m_PathContent, m_LabelStyle);
            EditorGUILayout.PropertyField(m_EventProperty);
            serializedObject.ApplyModifiedProperties();
        }

        void InitHeaderStyle()
        {
            m_LabelStyle = new GUIStyle(EditorStyles.boldLabel) { wordWrap = true, clipping = TextClipping.Clip };
        }
    }
}