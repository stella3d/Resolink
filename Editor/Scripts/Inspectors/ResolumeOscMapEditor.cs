using UnityEditor;
using UnityEngine;

namespace Resolink
{
    [CustomEditor(typeof(ResolumeOscMap))]
    public class ResolumeOscMapEditor : Editor
    {
        const string k_ShowTargetsTooltip = "If enabled, shows any sub-targets found for every shortcut. " +
                                            " (not really used yet)";
        const string k_ShowUniqueIdsTooltip = 
            "If enabled, shows the unique identifier for the shortcut within the source map";
        const string k_InputPathTooltip = 
            "'Input' here means input into Resolume, not Unity.  This is the path we use to know " +
            "which unique Resolume control this path is associated with.";
        
        const string k_TypeTooltip = "The type of data associated with this address' event";
        const string k_IdTooltip = "The unique (within this map) identifier for this shortcut.";
        const string k_SubTargetTooltip = "Sub-targets are different option values for a single path, " +
                                          "associated with distinct controls in Resolume";

        const string k_DataTypePrefix = "data type: ";
        
        ResolumeOscMap m_Map;

        byte[] m_FoldoutStates;
        bool[] m_ColorGroupFoldoutStates;
        bool[] m_Vec2FoldoutStates;
        bool[] m_Vec3FoldoutStates;

        string[] m_Labels;
        
        GUIContent[] m_LabelsWithTooltips;
        GUIContent[] m_ColorGroupContents;
        GUIContent[] m_Vector2GroupContents;
        GUIContent[] m_Vector3GroupContents;

        bool m_ShowUniqueIds;
        bool m_ShowSubTargets;

        readonly GUIContent m_ShowSubTargetsContent = new GUIContent("Show Sub-Targets", k_ShowTargetsTooltip);
        readonly GUIContent m_ShowIdsContent = new GUIContent("Show Unique IDs", k_ShowUniqueIdsTooltip);
        
        readonly GUIContent m_IdContent = new GUIContent("ID", k_IdTooltip);
        readonly GUIContent m_InputPathContent = new GUIContent("Input Path", k_InputPathTooltip);
        readonly GUIContent m_TypeContent = new GUIContent("Type", k_TypeTooltip);
        readonly GUIContent m_SubTargetContent = new GUIContent("Sub-Target", k_SubTargetTooltip);
        readonly GUIContent m_SubTargetsContent = new GUIContent("Sub-Targets", k_SubTargetTooltip);

        readonly GUILayoutOption m_LeftColumnWidth = GUILayout.Width(112);

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
            m_ColorGroupFoldoutStates = new bool[m_Map.ColorGroups.Count];
            m_Vec2FoldoutStates = new bool[m_Map.Vector2Groups.Count];
            m_Vec3FoldoutStates = new bool[m_Map.Vector3Groups.Count];
            GenerateLabels();
        }

        public void GenerateLabels()
        {
            if (m_Labels == null || m_Labels.Length != m_Map.Shortcuts.Count)
                m_Labels = new string[m_Map.Shortcuts.Count];

            m_LabelsWithTooltips = new GUIContent[m_Map.Shortcuts.Count];
            for (int i = 0; i < m_Labels.Length; i++)
            {
                var shortcut = m_Map.Shortcuts[i];
                m_Labels[i] = shortcut.Output.Path;
                m_LabelsWithTooltips[i] = new GUIContent(shortcut.Output.Path, k_DataTypePrefix + shortcut.TypeName);
            }

            m_ColorGroupContents = new GUIContent[m_Map.ColorGroups.Count];
            for (int i = 0; i < m_ColorGroupContents.Length; i++)
            {
                // TODO - titles based on prefix
                m_ColorGroupContents[i] = new GUIContent("Color Group",
                    "Four RGBA float events that make up a Color");
            }

            m_Vector2GroupContents = new GUIContent[m_Map.Vector2Groups.Count];
            for (int i = 0; i < m_Vector2GroupContents.Length; i++)
            {
                m_Vector2GroupContents[i] = new GUIContent("Vector2 Group", 
                    "Two XY float events that make up a Vector2");
            }

            m_Vector3GroupContents = new GUIContent[m_Map.Vector3Groups.Count];
            for (int i = 0; i < m_Vector3GroupContents.Length; i++)
            {
                m_Vector3GroupContents[i] = new GUIContent("Vector3 Group", 
                    "Three XYZ float events that make up a Vector2");
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

        void DrawColorGroups()
        {
            for (var i = 0; i < m_Map.ColorGroups.Count; i++)
            {
                var foldoutState = m_ColorGroupFoldoutStates[i];
                var afterState = EditorGUILayout.BeginFoldoutHeaderGroup(foldoutState, m_LabelsWithTooltips[i], 
                    HeaderFoldoutStyle);

                EditorGUILayout.Separator();
            }
        }

        void DrawOptions()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.PrefixLabel(m_ShowSubTargetsContent);
                m_ShowSubTargets = EditorGUILayout.Toggle(m_ShowSubTargets);
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.PrefixLabel(m_ShowIdsContent);
                m_ShowUniqueIds = EditorGUILayout.Toggle(m_ShowUniqueIds);
            }
            
            GUILayout.Box("", GUILayout.Height(1), GUILayout.ExpandWidth(true));
        }

        public void DrawShortcut(int index)
        {
            var foldoutState = m_FoldoutStates[index] != byte.MinValue;
            var afterState = EditorGUILayout.BeginFoldoutHeaderGroup(foldoutState, m_LabelsWithTooltips[index], 
                HeaderFoldoutStyle);
            
            m_FoldoutStates[index] = afterState ? byte.MaxValue : byte.MinValue;
            if (!afterState)
            {
                EditorGUILayout.EndFoldoutHeaderGroup();
                return;
            }

            var shortcut = m_Map.Shortcuts[index];

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField(m_TypeContent, m_LeftColumnWidth);
                EditorGUILayout.LabelField(shortcut.TypeName, EditorStyles.miniLabel);
            }

            if (m_ShowUniqueIds)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField(m_IdContent, m_LeftColumnWidth);
                    EditorGUILayout.LabelField(shortcut.UniqueId.ToString(), EditorStyles.miniLabel);
                }
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField(m_InputPathContent, m_LeftColumnWidth);
                EditorGUILayout.LabelField(shortcut.Input.Path, EditorStyles.miniLabel);
            }

            if (shortcut.SubTargets == null || shortcut.SubTargets.Length == 0)
            {
                EditorGUILayout.EndFoldoutHeaderGroup();
                return;
            }

            if(m_ShowSubTargets)
                DrawSubTargetsIfAny(shortcut.SubTargets);
            
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        void DrawSubTarget(SubTarget subTarget)
        {
            EditorGUILayout.LabelField(m_SubTargetContent);
            using (new EditorGUI.IndentLevelScope())
            {
                EditorGUILayout.LabelField("Option Index", subTarget.OptionIndex.ToString());
                EditorGUILayout.LabelField("Type", subTarget.Type.ToString());
            }
        }
        
        void DrawSubTargets(SubTarget[] subTargets)
        {
            EditorGUILayout.LabelField(m_SubTargetsContent);
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
        
        void DrawSubTargetsIfAny(SubTarget[] subTargets)
        {
            if(subTargets.Length == 1)
                DrawSubTarget(subTargets[0]);
            else
                DrawSubTargets(subTargets);
        }
    }
}