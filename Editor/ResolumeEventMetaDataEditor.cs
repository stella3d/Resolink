using System;
using UnityEditor;
using UnityEngine;

namespace Resolunity
{
    [CustomEditor(typeof(ResolumeEventMetaData))]
    public class ResolumeEventMetaDataEditor : Editor
    {
        ResolumeEventMetaData m_Target;
        
        SerializedProperty m_PathsProperty;
        SerializedProperty m_TypesProperty;

        public void OnEnable()
        {
            m_Target = (ResolumeEventMetaData) target;
            m_PathsProperty = serializedObject.FindProperty("InputPaths");
            m_TypesProperty = serializedObject.FindProperty("Types");
        }

        public override void OnInspectorGUI()
        {
            //DrawDefaultInspector();
            serializedObject.Update();

            var inPaths = m_Target.InputPaths;
            var types = m_Target.Types;
            //if (types.Count == 0 || inPaths.Count == 0 || inPaths.Count != types.Count)
            //    return;
            
            for (int i = 0; i < m_Target.InputPaths.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();

                var pathProperty = m_PathsProperty.GetArrayElementAtIndex(i);
                var path = EditorGUILayout.TextField(pathProperty.stringValue, GUILayout.ExpandWidth(true));
                pathProperty.stringValue = path;

                var typeProperty = m_TypesProperty.GetArrayElementAtIndex(i);
                var typeBefore = (TypeSelectionEnum) typeProperty.enumValueIndex;
                var type = (TypeSelectionEnum) EditorGUILayout.EnumPopup(typeBefore, GUILayout.Width(128));
                typeProperty.enumValueIndex = (int) type;

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();
            }

            EditorGUILayout.Space();
            DrawAddButton();

            serializedObject.ApplyModifiedProperties();
        }

        void DrawAddButton()
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add New"))
                m_Target.AddCapacity();
            
            if (GUILayout.Button("Remove Last", GUILayout.Width(144)))
                m_Target.Trim();

            EditorGUILayout.EndHorizontal();
        }
    }
}