using UnityEditor;
using UnityEngine;

namespace Resolink
{
    [CustomEditor(typeof(EventComponentMapping))]
    public class EventComponentMappingEditor : Editor
    {
        const string k_HelpText = "This takes the map of OSC events we get from Resolume & generates " +
                                  "components with UnityEvents for each address in the map.\n\n" +
                                  "When you change your OSC map in Resolume, update the asset & click this button. " +
                                  "After generation, you can find the component for an event on the appropriate child " +
                                  "object, according to what the event is associated with in Resolume.\n\n" +
                                  "If a component for a given event is already present, it will be skipped.";
        
        protected EventComponentMapping m_Target;

        SerializedProperty m_OscMapProperty;
        
        SerializedProperty m_TempoObjectProperty;
        SerializedProperty m_ClipTransportProperty;
        SerializedProperty m_ClipCuepointsProperty;
        SerializedProperty m_CompositionProperty;
        SerializedProperty m_CompositionLayerProperty;
        SerializedProperty m_CompositionDashboardProperty;
        SerializedProperty m_CompositionLayerDashboardProperty;
        SerializedProperty m_ApplicationUIProperty;

        bool m_EventObjectFoldoutState;
        
        public void OnEnable()
        {
            m_Target = (EventComponentMapping) target;

            m_OscMapProperty = serializedObject.FindProperty("OscMap");

            m_TempoObjectProperty = serializedObject.FindProperty("TempoController");
            m_ClipTransportProperty = serializedObject.FindProperty("ClipTransport");
            m_ClipCuepointsProperty = serializedObject.FindProperty("ClipCuepoints");
            m_CompositionProperty = serializedObject.FindProperty("Composition");
            m_CompositionLayerProperty = serializedObject.FindProperty("CompositionLayer");
            m_CompositionDashboardProperty = serializedObject.FindProperty("CompositionDashboard");
            m_CompositionLayerDashboardProperty = serializedObject.FindProperty("CompositionLayerDashboard");
            m_ApplicationUIProperty = serializedObject.FindProperty("ApplicationUI");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            EditorGUILayout.PropertyField(m_OscMapProperty);
            
            m_EventObjectFoldoutState = EditorGUILayout.Foldout(m_EventObjectFoldoutState, "Event Component Objects");
            if (m_EventObjectFoldoutState)
            {
                EditorGUILayout.PropertyField(m_TempoObjectProperty);
                EditorGUILayout.PropertyField(m_ClipTransportProperty);
                EditorGUILayout.PropertyField(m_ClipCuepointsProperty);
                EditorGUILayout.PropertyField(m_CompositionProperty);
                EditorGUILayout.PropertyField(m_CompositionLayerProperty);
                EditorGUILayout.PropertyField(m_CompositionDashboardProperty);
                EditorGUILayout.PropertyField(m_CompositionLayerDashboardProperty);
                EditorGUILayout.PropertyField(m_ApplicationUIProperty);
            }

            EditorGUILayout.Space();
            EditorUtils.DrawBoxLine();
            
            EditorGUILayout.HelpBox(k_HelpText, MessageType.Info);
            EditorGUILayout.Space();

            var noMap = m_Target.OscMap == null;
            
            using (new EditorGUI.DisabledScope(noMap))
            {
                if (GUILayout.Button("Generate Event Components"))
                    m_Target.PopulateEvents();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}