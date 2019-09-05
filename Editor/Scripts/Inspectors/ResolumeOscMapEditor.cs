using UnityEditor;
using UnityEngine;

namespace Resolink
{
    [CustomEditor(typeof(ResolumeOscMap))]
    public class ResolumeOscMapEditor : Editor
    {
        ResolumeOscMap m_Map;

        byte[] m_FoldoutStates;

        string[] m_Labels;

        bool m_ShowUniqueIds;
        bool m_ShowSubTargets;

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
            m_FoldoutStates = new byte[m_Map.Shortcuts.Count];
            GenerateLabels();
        }

        public void GenerateLabels()
        {
            if (m_Labels == null || m_Labels.Length != m_Map.Shortcuts.Count)
                m_Labels = new string[m_Map.Shortcuts.Count];

            for (int i = 0; i < m_Labels.Length; i++)
            {
                var shortcut = m_Map.Shortcuts[i];
                m_Labels[i] = shortcut.Output.Path;
            }
        }

        public override void OnInspectorGUI()
        {
            DrawOptions();

            for (var i = 0; i < m_Map.Shortcuts.Count; i++)
            {
                DrawShortcut(i);
                EditorGUILayout.Separator();
            }
        }

        void DrawOptions()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.PrefixLabel("Show Sub-Targets");
                m_ShowSubTargets = EditorGUILayout.Toggle(m_ShowSubTargets);
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.PrefixLabel("Show Shortcut IDs");
                m_ShowUniqueIds = EditorGUILayout.Toggle(m_ShowUniqueIds);
            }
            
            GUILayout.Box("", GUILayout.Height(1), GUILayout.ExpandWidth(true));
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

            // TODO - draw data type
            
            if(m_ShowUniqueIds)
                EditorGUILayout.LabelField("ID", shortcut.UniqueId.ToString(), EditorStyles.miniLabel);
            
            EditorGUILayout.LabelField("Input Path", shortcut.Input.Path);
            EditorGUILayout.LabelField("Output Path", shortcut.Output.Path);

            if (shortcut.SubTargets == null || shortcut.SubTargets.Length == 0)
            {
                EditorGUILayout.EndFoldoutHeaderGroup();
                return;
            }

            if(m_ShowSubTargets)
                DrawSubTargetsIfAny(shortcut.SubTargets);
            
            EditorGUILayout.EndFoldoutHeaderGroup();
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