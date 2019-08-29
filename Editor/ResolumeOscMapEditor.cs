using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Resolunity
{
    [CustomEditor(typeof(ResolumeOscMap))]
    public class ResolumeOscMapEditor : Editor
    {
        enum LabelIndexOptions : byte
        {
            One, 
            Two, 
            Three
        }
        
        LabelIndexOptions m_LabelOption;
        LabelIndexOptions m_PreviousLabelOption;
        
        ResolumeOscMap m_Map;

        byte[] m_FoldoutStates;

        string[] m_Labels;

        bool m_ShowUniqueIds;

        GUIStyle m_HeaderFoldout;
        
        GUIStyle HeaderFoldoutStyle 
        {
            get
            {
                if (m_HeaderFoldout != null) 
                    return m_HeaderFoldout;

                m_HeaderFoldout = new GUIStyle(EditorStyles.foldoutHeader) {stretchWidth = true};
                return m_HeaderFoldout;
            }
        }

        public void OnEnable()
        {
            m_Map = (ResolumeOscMap) target;
            if( m_Map.Events?.IdToFloatEvent == null)
                m_Map.Events = new EventComponentMapping(m_Map);
            
            m_Map.Events.PopulateEvents();
            m_FoldoutStates = new byte[m_Map.Shortcuts.Count];
            GenerateLabels();
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
            }
        }

        public override void OnInspectorGUI()
        {
            DrawOptions();

            if (m_Map.Events.Count == 0)
            {
                Debug.Log("populating events");
                m_Map.Events.PopulateEvents();
            }

            for (var i = 0; i < m_Map.Shortcuts.Count; i++)
            {
                DrawShortcut(i);
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

        public void DrawShortcut(int index)
        {
            var foldoutState = m_FoldoutStates[index] != byte.MinValue;
            var afterState = EditorGUILayout.BeginFoldoutHeaderGroup(foldoutState, m_Labels[index], HeaderFoldoutStyle);
            m_FoldoutStates[index] = afterState ? byte.MaxValue : byte.MinValue;
            if (!afterState)
            {
                EditorGUILayout.EndFoldoutHeaderGroup();
                return;
            }

            var shortcut = m_Map.Shortcuts[index];

            if(m_ShowUniqueIds)
                EditorGUILayout.LabelField("ID", shortcut.UniqueId.ToString(), EditorStyles.miniLabel);
            
            EditorGUILayout.LabelField("Input Path", shortcut.Input.Path);
            EditorGUILayout.LabelField("Output Path", shortcut.Output.Path);

            if (shortcut.SubTargets == null || shortcut.SubTargets.Length == 0)
            {
                EditorGUILayout.EndFoldoutHeaderGroup();
                return;
            }

            DrawSubTargetsIfAny(shortcut.SubTargets);
            DrawEvents(m_Map, shortcut);
            
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        void DrawEvents(ResolumeOscMap map, ResolumeOscShortcut shortcut)
        {
            if (shortcut.DataType == typeof(int))
            {
            }
            else if (shortcut.DataType == typeof(float))
            {
                
            }
        }

        static void DrawSubTarget(SubTarget subTarget)
        {
            EditorGUILayout.LabelField("Sub Target");
            using (new EditorGUI.IndentLevelScope())
            {
                EditorGUILayout.LabelField("Option Index", subTarget.OptionIndex.ToString());
                EditorGUILayout.LabelField("Type", subTarget.Type.ToString());
            }
        }
        
        static void DrawSubTargets(SubTarget[] subTargets)
        {
            EditorGUILayout.LabelField("Sub Targets");
            using (new EditorGUI.IndentLevelScope())
            {
                foreach (var subTarget in subTargets)
                {
                    EditorGUILayout.PrefixLabel("Option Index " + subTarget.OptionIndex);
                    using (new EditorGUI.IndentLevelScope())
                    {
                        EditorGUILayout.LabelField("Type", subTarget.Type.ToString());
                    }
                }
            }
        }
        
        static void DrawSubTargetsIfAny(SubTarget[] subTargets)
        {
            if(subTargets.Length == 1)
                DrawSubTarget(subTargets[0]);
            else
                DrawSubTargets(subTargets);
        }
    }
}