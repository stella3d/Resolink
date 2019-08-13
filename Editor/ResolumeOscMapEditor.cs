using System;
using UnityEditor;
using UnityEngine;

namespace UnityResolume
{
    [CustomEditor(typeof(ResolumeOscMap))]
    public class ResolumeOscMapEditor : Editor
    {
        ResolumeOscMap m_Map;

        public void OnEnable()
        {
            m_Map = (ResolumeOscMap) target;
            if(m_Map == null)
                Debug.LogWarning("Failed to get Resolume map from target");
        }

        public override void OnInspectorGUI()
        {
            foreach (var shortcut in m_Map.Shortcuts)
            {
                DrawShortcut(shortcut);
                EditorGUILayout.Separator();
            }
        }

        public static void DrawShortcut(ResolumeOscShortcut shortcut)
        {
            EditorGUILayout.LabelField(shortcut.UniqueId.ToString());
            using (new EditorGUI.IndentLevelScope())
            {
                EditorGUILayout.LabelField("Input Path", shortcut.Input.Path);
                EditorGUILayout.LabelField("Output Path", shortcut.Output.Path);
            }
        }
    }
}