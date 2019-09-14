using UnityEditor;
using UnityEngine;

namespace Resolink.Tests
{
    [CustomEditor(typeof(ExpectedParseResult))]
    public class ExpectedParseResultEditor : Editor
    {
        SerializedProperty m_PathProperty;
        SerializedProperty m_ResultProperty;

        public void OnEnable()
        {
            m_PathProperty = serializedObject.FindProperty("XmlPath");
            m_ResultProperty = serializedObject.FindProperty("ExpectedResult");
        }

        public override void OnInspectorGUI()
        {
            //DrawDefaultInspector();
            serializedObject.Update();

            EditorGUILayout.PropertyField(m_ResultProperty);
            
            if (GUILayout.Button("Select XML file"))
            {
                var path = EditorUtility.OpenFilePanel("Select Resolume XML file", Application.dataPath, "xml");
                path = path.Replace(Application.dataPath, "");
                m_PathProperty.stringValue = path;
            }

            EditorGUILayout.LabelField(m_PathProperty.stringValue);
            serializedObject.ApplyModifiedProperties();
        }
    }
}