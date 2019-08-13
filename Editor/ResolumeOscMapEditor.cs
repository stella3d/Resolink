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
            var outPath = shortcut.Output.Path;
            var labelIndex = outPath.LastIndexOf('/') + 1;
            var label = char.ToUpper(outPath[labelIndex]) + outPath.Substring(labelIndex + 1);

            using (new GUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
                EditorGUILayout.LabelField(shortcut.UniqueId.ToString(), EditorStyles.miniLabel);
            }

            using (new EditorGUI.IndentLevelScope())
            {
                EditorGUILayout.LabelField("Input Path", shortcut.Input.Path);
                EditorGUILayout.LabelField("Output Path", shortcut.Output.Path);
                /*
                if (shortcut.SubTarget == null) 
                    return;

                DrawSubTarget(shortcut.SubTarget);
                */
            }
        }

        static void DrawSubTarget(SubTarget subTarget)
        {
            using (new EditorGUI.IndentLevelScope())
            {
                EditorGUILayout.LabelField("Type", subTarget.Type.ToString());
                EditorGUILayout.LabelField("Option Index", subTarget.OptionIndex.ToString());
            }
        }
    }
}