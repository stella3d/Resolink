using System;
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
        where TCombinedData : IEquatable<TCombinedData>
    {
        const string k_PathTooltip = "The OSC address we receive messages at associated with this event";
        const string k_MethodTooltip = "The method called when a message is received at this address";
        const string k_HandlersTooltip = "What each address that makes up this control does";

        protected GUIContent[] m_PathContents;
        protected GUIContent[] m_EventContents;

        protected TComponent m_Component;
        
        protected SerializedProperty m_EventProperty;
        protected SerializedProperty m_ValueProperty;
        protected SerializedProperty m_DefaultValueProperty;

        protected GUIStyle m_LabelStyle;
        protected GUIStyle m_MethodNameStyle;

        protected readonly GUIContent m_AddressHandlersContent = new GUIContent("Address Handlers", k_HandlersTooltip);

        protected bool m_HandlersFoldoutState = true;

        protected TCombinedData m_PreviousDefaultValue;
        protected TCombinedData m_CurrentDefaultValue;

        public void OnEnable()
        {
            m_Component = (TComponent) target;
            m_EventProperty = serializedObject.FindProperty("Event");
            m_ValueProperty = serializedObject.FindProperty("Value");
            m_DefaultValueProperty = serializedObject.FindProperty("m_DefaultValue");
            m_Component.Setup();
            InitContents();
        }

        void InitContents()
        {
            if (m_Component == null)
                return;
            
            m_PathContents = new GUIContent[m_Component.Handlers.Length];
            m_EventContents = new GUIContent[m_Component.Handlers.Length];
            for (var i = 0; i < m_Component.Handlers.Length; i++)
            {
                var handler = m_Component.Handlers[i];
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
            if(m_PathContents == null || m_EventContents == null)
                InitContents();
            
            serializedObject.Update();

            m_HandlersFoldoutState = EditorGUILayout.Foldout(m_HandlersFoldoutState, m_AddressHandlersContent);
            if (m_HandlersFoldoutState)
            {
                for (var i = 0; i < m_PathContents.Length; i++)
                    DrawPathWithActionName(m_PathContents[i], m_EventContents[i]);
                
                EditorUtils.DrawBoxLine();
            }

            var allowDefaultEdit = !EditorApplication.isPlayingOrWillChangePlaymode;
            using (new EditorGUI.DisabledScope(!allowDefaultEdit))
            {
                m_PreviousDefaultValue = m_CurrentDefaultValue;
                EditorGUILayout.PropertyField(m_DefaultValueProperty);
                if (allowDefaultEdit)
                {
                    m_CurrentDefaultValue = m_Component.DefaultValue;
                    if (!m_CurrentDefaultValue.Equals(m_PreviousDefaultValue))
                        m_Component.AssignDefaultValue();
                }
            }

            using (new EditorGUI.DisabledScope(true))
                EditorGUILayout.PropertyField(m_ValueProperty);
            
#if RESOLINK_DEBUG_COMPOUND_EVENTS || true
            DrawDebugUI();
#endif
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
        
        protected static float ToFloat255(float float01)
        {
            return Mathf.Lerp(0, 255f, float01);
        }

        /// <summary>
        /// Implement this in a derived class and define RESOLINK_DEBUG_COMPOUND_EVENTS to draw debug ui
        /// </summary>
        protected virtual void DrawDebugUI() { }
    }
}