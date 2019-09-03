using UnityEditor;
using UnityEngine;

namespace Resolink
{
    [CustomEditor(typeof(ResolumeEventMetaData))]
    public class ResolumeEventMetaDataEditor : Editor
    {
        const int k_RightColumnWidth = 100;
        
        ResolumeEventMetaData m_Target;
        
        SerializedProperty m_PathsProperty;
        SerializedProperty m_TypesProperty;

        readonly GUILayoutOption m_RightColumnWidthOption = GUILayout.Width(k_RightColumnWidth);

        public void OnEnable()
        {
            m_Target = (ResolumeEventMetaData) target;
            m_PathsProperty = serializedObject.FindProperty("InputPaths");
            m_TypesProperty = serializedObject.FindProperty("Types");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Input Path", EditorStyles.centeredGreyMiniLabel);
                EditorGUILayout.LabelField("Type", EditorStyles.centeredGreyMiniLabel, m_RightColumnWidthOption);
            }

            for (int i = 0; i < m_Target.InputPaths.Count; i++)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    var pathProperty = m_PathsProperty.GetArrayElementAtIndex(i);
                    var path = EditorGUILayout.TextField(pathProperty.stringValue, GUILayout.ExpandWidth(true));
                    pathProperty.stringValue = path;

                    var typeProperty = m_TypesProperty.GetArrayElementAtIndex(i);
                    var typeBefore = (TypeSelectionEnum) typeProperty.enumValueIndex;
                    var type = (TypeSelectionEnum) EditorGUILayout.EnumPopup(typeBefore, m_RightColumnWidthOption);
                    typeProperty.enumValueIndex = (int) type;
                }
                
                EditorGUILayout.Space();
            }

            EditorUtils.DrawBoxLine();
            EditorGUILayout.Space();
            
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Add New"))
                    m_Target.AddCapacity();
                if (GUILayout.Button("Remove Last", m_RightColumnWidthOption))
                    m_Target.Trim();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}