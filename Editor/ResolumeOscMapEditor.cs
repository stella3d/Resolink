using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace UnityResolume
{
    [CustomEditor(typeof(ResolumeOscMap))]
    public class ResolumeOscMapEditor : Editor
    {
        ResolumeOscMap m_Map;

        byte[] m_FoldoutStates;

        string[] m_Labels;

        bool m_ShowUniqueIds;

        byte[] m_LabelIndexOptions = { 1, 2, 3 };

        enum LabelIndexOptions : byte
        {
            One, 
            Two, 
            Three
        }

        LabelIndexOptions m_LabelOption;
        LabelIndexOptions m_PreviousLabelOption;

        public void OnEnable()
        {
            m_Map = (ResolumeOscMap) target;
            if(m_Map == null)
                Debug.LogWarning("Failed to get Resolume map from target");
            
            m_FoldoutStates = new byte[m_Map.Shortcuts.Count];

            GenerateLabels();
            /*
            m_Labels = new string[m_Map.Shortcuts.Count];
            for (int i = 0; i < m_Labels.Length; i++)
            {
                m_Labels[i] = GetNiceShortcutLabel(m_Map.Shortcuts[i]);
            }
            */
        }

        public void GenerateLabels()
        {
            if (m_Labels == null || m_Labels.Length != m_Map.Shortcuts.Count)
                m_Labels = new string[m_Map.Shortcuts.Count];

            for (int i = 0; i < m_Labels.Length; i++)
            {
                var outPath = m_Map.Shortcuts[i].Output.Path;
                switch (m_LabelOption)
                {
                    case LabelIndexOptions.One:
                        m_Labels[i] = GetNiceLabel1Chunk(outPath);
                        break;
                    case LabelIndexOptions.Two:
                        m_Labels[i] = GetNiceLabel2Chunks(outPath);
                        break;
                    case LabelIndexOptions.Three:
                        m_Labels[i] = GetNiceLabel3Chunks(outPath);
                        break;
                }

                //m_Labels[i] = GetNiceShortcutLabel(m_Map.Shortcuts[i]);
            }
        }

        public override void OnInspectorGUI()
        {
            DrawOptions();
            
            for (var i = 0; i < m_Map.Shortcuts.Count; i++)
            {
                DrawShortcut(i, m_Map.Shortcuts);
                EditorGUILayout.Separator();
            }
        }

        void DrawOptions()
        {
            m_PreviousLabelOption = m_LabelOption;
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.PrefixLabel("Header Label Path Chunks");
                m_LabelOption = (LabelIndexOptions) EditorGUILayout.EnumPopup(m_LabelOption);
            }
            
            if(m_PreviousLabelOption != m_LabelOption)
                GenerateLabels();

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.PrefixLabel("Show Shortcut IDs");
                m_ShowUniqueIds = EditorGUILayout.Toggle(m_ShowUniqueIds);
            }

            
            GUILayout.Box("", GUILayout.Height(1), GUILayout.Width(360));
        }

        static string GetNiceShortcutLabel(ResolumeOscShortcut shortcut)
        {
            var outPath = shortcut.Output.Path;
            int lastIndex = outPath.LastIndexOf('/');
            int secondLastIndex = lastIndex > 0 ? outPath.LastIndexOf('/', lastIndex - 1) : -1;
            var labelIndex = (secondLastIndex > 0 ? secondLastIndex : lastIndex) + 1;
            return char.ToUpper(outPath[labelIndex]) + outPath.Substring(labelIndex + 1);
        }
        
        static string GetNiceLabel1Chunk(string outPath)
        {
            var labelIndex = outPath.LastIndexOf('/') + 1;
            return char.ToUpper(outPath[labelIndex]) + outPath.Substring(labelIndex + 1);
        }
        
        static string GetNiceLabel2Chunks(string outPath)
        {
            int lastIndex = outPath.LastIndexOf('/');
            int secondLastIndex = lastIndex > 0 ? outPath.LastIndexOf('/', lastIndex - 1) : -1;
            var labelIndex = (secondLastIndex > 0 ? secondLastIndex : lastIndex) + 1;
            return char.ToUpper(outPath[labelIndex]) + outPath.Substring(labelIndex + 1);
        }
        
        static string GetNiceLabel3Chunks(string outPath)
        {
            int lastIndex = outPath.LastIndexOf('/');
            int secondLastIndex = lastIndex > 0 ? outPath.LastIndexOf('/', lastIndex - 1) : -1;
            int thirdLastIndex = secondLastIndex > 0 ? outPath.LastIndexOf('/', secondLastIndex - 1) : -1;

            int labelIndex;
            if (thirdLastIndex > 0)
                labelIndex = thirdLastIndex + 1;
            else if(secondLastIndex > 0)
                labelIndex = secondLastIndex + 1;
            else
                labelIndex = lastIndex + 1;

            return char.ToUpper(outPath[labelIndex]) + outPath.Substring(labelIndex + 1);
        }

        public void DrawShortcut(int index, List<ResolumeOscShortcut> shortcuts)
        {
            var foldoutState = m_FoldoutStates[index] != byte.MinValue;
            var afterState = EditorGUILayout.Foldout(foldoutState, m_Labels[index], EditorStyles.foldoutHeader);
            m_FoldoutStates[index] = afterState ? byte.MaxValue : byte.MinValue;
            if (!afterState)
                return;

            var shortcut = m_Map.Shortcuts[index];

            if(m_ShowUniqueIds)
                EditorGUILayout.LabelField("ID", shortcut.UniqueId.ToString(), EditorStyles.miniLabel);
            
            EditorGUILayout.LabelField("Input Path", shortcut.Input.Path);
            EditorGUILayout.LabelField("Output Path", shortcut.Output.Path);
            
            if (shortcut.SubTarget == null || !shortcut.SubTarget.IsValid()) 
                return;

            DrawSubTarget(shortcut.SubTarget);
        }

        static void DrawSubTarget(SubTarget subTarget)
        {
            EditorGUILayout.LabelField("Sub Target");
            using (new EditorGUI.IndentLevelScope())
            {
                EditorGUILayout.LabelField("Type", subTarget.Type.ToString());
                EditorGUILayout.LabelField("Option Index", subTarget.OptionIndex.ToString());
            }
        }
    }
}