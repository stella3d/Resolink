using UnityEditor;
using UnityEngine;

namespace Resolunity
{
    [CustomEditor(typeof(EventComponentMapping))]
    public class EventComponentMappingEditor : Editor
    {

        EventComponentMapping m_Target;
        
        public void OnEnable()
        {
            m_Target = (EventComponentMapping) target;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Generate Mapping"))
            {
                m_Target.PopulateEvents();
            }
        }
    }
}