using UnityEditor;
using UnityEngine;

namespace Resolink
{
    [CustomEditor(typeof(TimeManager))]
    public class TimeManagerEditor : Editor
    {
        const string k_HelpText = "This component, along with the events below it, provide default behaviors for " +
                                  "syncing Unity's time with Resolume's - pause and BPM sync.\n\nThis will display the " +
                                  "BPM once we receive tempo messages from resolume in Play mode";

        SerializedProperty m_BpmProperty;
        
        public void OnEnable()
        {
            m_BpmProperty = serializedObject.FindProperty("m_Bpm");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            EditorUtils.Help(k_HelpText);
            
            var floatValue = m_BpmProperty.floatValue;
            string valueString = Mathf.Approximately(floatValue, 0f) ? "unknown" : floatValue.ToString("F2");
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("BPM");
                EditorGUILayout.LabelField(valueString);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}