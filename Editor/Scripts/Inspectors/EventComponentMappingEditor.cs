using UnityEditor;
using UnityEngine;

namespace Resolink
{
    [CustomEditor(typeof(EventComponentMapping))]
    public class EventComponentMappingEditor : Editor
    {
        const string k_HelpText = "This component takes the map of OSC events we get from Resolume & generates " +
                                  "components with UnityEvents for each address in the map.\n\n";
        
        protected EventComponentMapping m_Target;
        
        public void OnEnable()
        {
            m_Target = (EventComponentMapping) target;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            EditorGUILayout.Space();
            EditorUtils.DrawBoxLine();

            if (GUILayout.Button("Generate Mapping"))
            {
                m_Target.PopulateEvents();
            }
        }
    }
}