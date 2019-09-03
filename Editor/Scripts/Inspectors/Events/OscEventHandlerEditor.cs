using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Resolink
{
    public abstract class OscEventHandlerEditor<TComponent, TEvent, T> : Editor
        where TComponent : OscEventHandler<TEvent, T>
        where TEvent : UnityEvent<T>, new()
    {
        protected TComponent m_Component;

        protected SerializedProperty m_EventProperty;

        protected GUIStyle m_LabelStyle;

        public void OnEnable()
        {
            m_Component = (TComponent) target;
            m_EventProperty = serializedObject.FindProperty("Event");
        }

        public override void OnInspectorGUI()
        {
            if (m_LabelStyle == null)
                InitHeaderStyle();
            
            serializedObject.UpdateIfRequiredOrScript();
            EditorGUILayout.LabelField(m_Component.Shortcut.Output.Path, m_LabelStyle);
            EditorGUILayout.PropertyField(m_EventProperty);
            serializedObject.ApplyModifiedProperties();
        }

        void InitHeaderStyle()
        {
            m_LabelStyle = new GUIStyle(EditorStyles.boldLabel);
        }
    }
}