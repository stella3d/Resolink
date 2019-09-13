using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Resolink
{
    [CustomEditor(typeof(ResolumeOscMap))]
    public class ResolumeOscMapEditor : Editor
    {
        const string k_ControlGroupsTip = "Groups of event recievers that combine to form more complex data types";
        const string k_ColorGroupTip = "4 RGBA float events that make up a Color";
        const string k_Vec2GroupTip = "2 XY float events that make up a Vector2";
        const string k_Vec3GroupTip = "3 XYZ float events that make up a Vector3";
        
        const string k_GroupsOf = "All groups of ";
        const string k_ColorGroupsFoldTip = k_GroupsOf + k_ColorGroupTip;
        const string k_Vec2GroupsFoldTip = k_GroupsOf + k_Vec2GroupTip;
        const string k_Vec3GroupsFoldTip = k_GroupsOf + k_Vec3GroupTip;

        const string k_ShowTargetsTip = "If enabled, shows any sub-targets found for every shortcut. " +
                                            " (not really used yet)";
        const string k_ShowUniqueIdsTip = 
            "If enabled, shows the unique identifier for the shortcut within the source map";
        const string k_InputPathTip = 
            "'Input' here means input into Resolume, not Unity.  This is the path we use to know " +
            "which unique Resolume control this path is associated with.";
        
        const string k_TypeTip = "The type of data associated with this address' event";
        const string k_IdTip = "The unique (within this map) identifier for this shortcut.";
        const string k_SubTargetTip = "Sub-targets are different option values for a single path, " +
                                          "associated with distinct controls in Resolume";

        const string k_DataTypePrefix = "data type: ";
        
        ResolumeOscMap m_Map;

        bool[] m_FoldoutStates;
        bool[] m_ColorGroupFoldoutStates;
        bool[] m_Vec2FoldoutStates;
        bool[] m_Vec3FoldoutStates;

        string[] m_Labels;
        
        GUIContent[] m_LabelsWithTips;
        GUIContent[] m_ColorGroupContents;
        GUIContent[] m_Vector2GroupContents;
        GUIContent[] m_Vector3GroupContents;

        bool m_AnyGroupsInMap;
        bool m_ShowUniqueIds;
        bool m_ShowSubTargets;

        bool m_ColorGroupTopFoldout;
        bool m_Vec2GroupTopFoldout;
        bool m_Vec3GroupTopFoldout;
        bool m_DrawSeparatorsForGrouped;
        bool m_ShowOptions;

        readonly GUIContent m_OptionsFoldContent = new GUIContent("View Options", "Controls for showing more fields");
        
        const string k_AddressFor = "The message address for ";
        const string k_VectorAddress = k_AddressFor + "the vector's ";
        readonly GUIContent m_XLabel = new GUIContent("X", k_VectorAddress + "X component");
        readonly GUIContent m_YLabel = new GUIContent("Y", k_VectorAddress + "Y component");
        readonly GUIContent m_ZLabel = new GUIContent("Z", k_VectorAddress + "Z component");
        
        const string k_ColorAddress = k_AddressFor + "the color's ";
        readonly GUIContent m_RedLabel = new GUIContent("Red", k_ColorAddress + "red component");
        readonly GUIContent m_GreenLabel = new GUIContent("Green", k_ColorAddress + "green component");
        readonly GUIContent m_BlueLabel = new GUIContent("Blue", k_ColorAddress + "blue component");
        readonly GUIContent m_AlphaLabel = new GUIContent("Alpha", k_ColorAddress + "alpha component");
        
        readonly GUIContent m_ControlGroupsHeaderContent = new GUIContent("Control Groups", k_ControlGroupsTip);
        readonly GUIContent m_ColorGroupsFoldContent = new GUIContent("Color", k_ColorGroupsFoldTip);
        readonly GUIContent m_Vec2GroupsFoldContent = new GUIContent("Vector2", k_Vec2GroupsFoldTip);
        readonly GUIContent m_Vec3GroupsFoldContent = new GUIContent("Vector3", k_Vec3GroupsFoldTip);
        
        const string k_ControlShortcutTip = 
            "'Shortcut' is how Resolume refers to a control associated with an OSC address";
        readonly GUIContent m_ShortcutsHeaderContent = new GUIContent("Control Shortcuts", k_ControlShortcutTip);
        
        readonly GUIContent m_ShowSubTargetsContent = new GUIContent("Show Sub-Targets", k_ShowTargetsTip);
        readonly GUIContent m_ShowIdsContent = new GUIContent("Show Unique IDs", k_ShowUniqueIdsTip);
        readonly GUIContent m_IdContent = new GUIContent("ID", k_IdTip);
        readonly GUIContent m_InputPathContent = new GUIContent("Input Path", k_InputPathTip);
        readonly GUIContent m_TypeContent = new GUIContent("Type", k_TypeTip);
        readonly GUIContent m_SubTargetContent = new GUIContent("Sub-Target", k_SubTargetTip);
        readonly GUIContent m_SubTargetsContent = new GUIContent("Sub-Targets", k_SubTargetTip);

        readonly GUILayoutOption m_LeftColumnWidth = GUILayout.Width(110);

        GUIStyle m_HeaderFoldout;
        
        GUIStyle HeaderFoldoutStyle 
        {
            get
            {
                if (m_HeaderFoldout != null) 
                    return m_HeaderFoldout;
                m_HeaderFoldout = new GUIStyle(EditorStyles.foldoutHeader) { stretchWidth = true };
                return m_HeaderFoldout;
            }
        }

        public void OnEnable()
        {
            m_Map = (ResolumeOscMap) target;
            m_FoldoutStates = new bool[m_Map.Shortcuts.Count];
            m_ColorGroupFoldoutStates = new bool[m_Map.ColorGroups.Count];
            m_Vec2FoldoutStates = new bool[m_Map.Vector2Groups.Count];
            m_Vec3FoldoutStates = new bool[m_Map.Vector3Groups.Count];
            m_AnyGroupsInMap = AnyGroupsInMap();
            GenerateLabels();
        }

        public void GenerateLabels()
        {
            if (m_Labels == null || m_Labels.Length != m_Map.Shortcuts.Count)
                m_Labels = new string[m_Map.Shortcuts.Count];

            m_LabelsWithTips = new GUIContent[m_Map.Shortcuts.Count];
            for (int i = 0; i < m_Labels.Length; i++)
            {
                var shortcut = m_Map.Shortcuts[i];
                m_Labels[i] = shortcut.Output.Path;
                m_LabelsWithTips[i] = new GUIContent(shortcut.Output.Path, k_DataTypePrefix + shortcut.TypeName);
            }

            m_ColorGroupContents = new GUIContent[m_Map.ColorGroups.Count];
            for (int i = 0; i < m_ColorGroupContents.Length; i++)
            {
                var group = m_Map.ColorGroups[i];
                var prefix = OscMapParser.PrefixFromShortcut(group.Red);
                m_ColorGroupContents[i] = new GUIContent(prefix, k_ColorGroupTip);
            }

            m_Vector2GroupContents = new GUIContent[m_Map.Vector2Groups.Count];
            for (int i = 0; i < m_Vector2GroupContents.Length; i++)
            {
                var group = m_Map.Vector2Groups[i];
                var prefix = OscMapParser.RemoveLastChar(group.X.Input.Path);
                m_Vector2GroupContents[i] = new GUIContent(prefix, k_Vec2GroupTip);
            }

            m_Vector3GroupContents = new GUIContent[m_Map.Vector3Groups.Count];
            for (int i = 0; i < m_Vector3GroupContents.Length; i++)
            {
                var group = m_Map.Vector3Groups[i];
                var prefix = OscMapParser.RemoveLastChar(group.X.Input.Path);
                m_Vector3GroupContents[i] = new GUIContent(prefix, k_Vec3GroupTip);
            }
        }

        public override void OnInspectorGUI()
        {
            DrawOptions();
            
            if (m_AnyGroupsInMap)
            {
                EditorGUILayout.LabelField(m_ControlGroupsHeaderContent, EditorStyles.boldLabel);
                DrawColorGroups();
                DrawVector2Groups();
                DrawVector3Groups();
                EditorUtils.DrawBoxLine();
            }

            EditorGUILayout.LabelField(m_ShortcutsHeaderContent, EditorStyles.boldLabel);
            for (var i = 0; i < m_Map.Shortcuts.Count; i++)
                DrawShortcutIndex(i);
        }
        
        void DrawOptions()
        {
            m_ShowOptions = EditorGUILayout.Foldout(m_ShowOptions, m_OptionsFoldContent);
            if (!m_ShowOptions)
            {
                EditorUtils.DrawBoxLine();
                return;
            }

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
            
            m_DrawSeparatorsForGrouped = m_ShowSubTargets || m_ShowUniqueIds;
            EditorUtils.DrawBoxLine();
        }

        static void DrawGroup<T>(List<T> list, GUIContent foldContent, GUIContent[] contents, 
            bool[] foldStates, ref bool topFoldState, Action<T> drawSingle)
        {
            if (list.Count == 0)
                return;

            topFoldState = EditorGUILayout.Foldout(topFoldState, foldContent);
            if (!topFoldState)
                return;

            using (new EditorGUI.IndentLevelScope())
            {
                for (var i = 0; i < list.Count; i++)
                {
                    var foldoutState = foldStates[i];
                    foldoutState = EditorGUILayout.BeginFoldoutHeaderGroup(foldoutState,
                        contents[i]);

                    foldStates[i] = foldoutState;
                    if (!foldoutState)
                    {
                        EditorGUILayout.EndFoldoutHeaderGroup();
                        continue;
                    }

                    drawSingle(list[i]);
                    EditorGUILayout.EndFoldoutHeaderGroup();
                }
            }

            EditorGUILayout.Separator();
        }
        
        void DrawColorGroups()
        {
            DrawGroup(m_Map.ColorGroups, m_ColorGroupsFoldContent, m_ColorGroupContents, m_ColorGroupFoldoutStates,
                ref m_ColorGroupTopFoldout, DrawColorGroup);
        }

        void DrawColorGroup(ColorShortcutGroup group)
        {
            DrawGroupedShortcut(group.Red, m_RedLabel);
            MaybeSeparator();
            DrawGroupedShortcut(group.Green, m_GreenLabel);
            MaybeSeparator();
            DrawGroupedShortcut(group.Blue, m_BlueLabel);
            MaybeSeparator();
            DrawGroupedShortcut(group.Alpha, m_AlphaLabel);
        }

        void DrawVector2Groups()
        {
            DrawGroup(m_Map.Vector2Groups, m_Vec2GroupsFoldContent, m_Vector2GroupContents, m_Vec2FoldoutStates,
                ref m_Vec2GroupTopFoldout, DrawVector2Group);
        }

        void DrawVector2Group(Vector2ShortcutGroup group)
        {
            DrawGroupedShortcut(group.X);
            MaybeSeparator();
            DrawGroupedShortcut(group.Y);
        }

        void DrawVector3Groups()
        {
            DrawGroup(m_Map.Vector3Groups, m_Vec3GroupsFoldContent, m_Vector3GroupContents, m_Vec3FoldoutStates,
                ref m_Vec3GroupTopFoldout, DrawVector3Group);
        }
        
        void DrawVector3Group(Vector3ShortcutGroup group)
        {
            DrawGroupedShortcut(group.X, m_XLabel);
            MaybeSeparator();
            DrawGroupedShortcut(group.Y, m_YLabel);
            MaybeSeparator();
            DrawGroupedShortcut(group.Z, m_ZLabel);
        }

        void MaybeSeparator()
        {
            if(m_DrawSeparatorsForGrouped)
                EditorGUILayout.Separator();
        }

        void DrawShortcutIndex(int index)
        {
            var foldoutState = m_FoldoutStates[index];
            foldoutState = EditorGUILayout.BeginFoldoutHeaderGroup(foldoutState, m_LabelsWithTips[index], 
                HeaderFoldoutStyle);
            
            m_FoldoutStates[index] = foldoutState;
            if (!foldoutState)
            {
                EditorGUILayout.EndFoldoutHeaderGroup();
                return;
            }

            var shortcut = m_Map.Shortcuts[index];
            DrawShortcut(shortcut);
        }

        public void DrawShortcut(ResolumeOscShortcut shortcut)
        {
            DrawType(shortcut);

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField(m_InputPathContent, m_LeftColumnWidth);
                EditorGUILayout.LabelField(shortcut.Input.Path, EditorStyles.miniLabel);
            }
            
            DrawUniqueId(shortcut);

            if (shortcut.SubTargets == null || shortcut.SubTargets.Length == 0)
            {
                EditorGUILayout.EndFoldoutHeaderGroup();
                return;
            }

            if(m_ShowSubTargets)
                DrawSubTargetsIfAny(shortcut.SubTargets);
            
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        public void DrawGroupedShortcut(ResolumeOscShortcut shortcut, GUIContent customLabel = null)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField(customLabel ?? m_InputPathContent, m_LeftColumnWidth);
                EditorGUILayout.LabelField(shortcut.Input.Path, EditorStyles.miniLabel);
            }
            
            DrawUniqueId(shortcut);

            if (shortcut.SubTargets == null || shortcut.SubTargets.Length == 0)
            {
                EditorGUILayout.EndFoldoutHeaderGroup();
                return;
            }

            if(m_ShowSubTargets)
                DrawSubTargetsIfAny(shortcut.SubTargets);
            
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        void DrawUniqueId(ResolumeOscShortcut shortcut)
        {
            if (m_ShowUniqueIds)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField(m_IdContent, m_LeftColumnWidth);
                    EditorGUILayout.LabelField(shortcut.UniqueId.ToString(), EditorStyles.miniLabel);
                }
            }
        }

        void DrawType(ResolumeOscShortcut shortcut)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField(m_TypeContent, m_LeftColumnWidth);
                EditorGUILayout.LabelField(shortcut.TypeName, EditorStyles.miniLabel);
            }
        }
        
        void DrawSubTarget(SubTarget subTarget)
        {
            EditorGUILayout.LabelField(m_SubTargetContent);
            using (new EditorGUI.IndentLevelScope())
            {
                EditorGUILayout.LabelField("Index", subTarget.OptionIndex.ToString());
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

        bool AnyGroupsInMap()
        {
            return m_Map.ColorGroups.Count > 0 || m_Map.Vector2Groups.Count > 0 || m_Map.Vector3Groups.Count > 0;
        }
    }
}