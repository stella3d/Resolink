using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Resolink
{
    class ResolinkEditorSettings : ScriptableObject
    {
        static ResolinkEditorSettings s_Instance;
        
        public static ResolinkEditorSettings Instance
        {
            get
            {
                if (s_Instance == null)
                    s_Instance = GetOrCreateSettings();

                return s_Instance;
            }
            private set => s_Instance = value;
        }
        
#pragma warning disable 649
        [SerializeField] bool m_ShowHelp;
        [SerializeField] bool m_WarnOnUnknownType;
        
        [SerializeField] bool m_GroupColors;
        [SerializeField] bool m_GroupVector2s;
        [SerializeField] bool m_GroupVector3s;
#pragma warning restore 649

        public bool ShowHelp => m_ShowHelp;
        public bool WarnOnUnknownType => m_WarnOnUnknownType;
        
        public bool GroupColors => m_GroupColors;
        public bool GroupVector2s => m_GroupVector2s;
        public bool GroupVector3s => m_GroupVector3s;

        public void OnEnable()
        {
            Instance = this;
        }

        internal static ResolinkEditorSettings GetOrCreateSettings()
        {
            var settings = GetSettingsFromAssets();
            if (settings == null)
            {
                const string settingsFallbackPath = "Assets/ResolinkEditorSettings.asset";
                settings = CreateInstance<ResolinkEditorSettings>();
                AssetDatabase.CreateAsset(settings, settingsFallbackPath);
                AssetDatabase.SaveAssets();
            }
            return settings;
        }

        internal static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(GetOrCreateSettings());
        }

        static ResolinkEditorSettings GetSettingsFromAssets()
        {
            const string search = "t: ResolinkEditorSettings";
            var guids = AssetDatabase.FindAssets(search);
            var firstPath = AssetDatabase.GUIDToAssetPath(guids[0]);
            return AssetDatabase.LoadAssetAtPath<ResolinkEditorSettings>(firstPath);
        }
    }

    static class ResolinkSettingsIMGUIRegister
    {
        static readonly GUIContent ShowHelpContent = new GUIContent("Show Help", 
            "If enabled, help boxes are shown on most Resolink components");

        static readonly GUIContent TypeWarningContent = new GUIContent("Warn on Unknown Type", 
            "If enabled, warnings will be shown in the console for Resolume shortcuts we don't know the data type " +
            "for when we parse the OSC map.");

        const string k_FloatSuffix = " event handler, as opposed to being individual float handlers.";
        
        static readonly GUIContent GroupColorsContent = new GUIContent("Group Colors", 
            "If enabled, r/g/b/a messages belonging to the same Color in Resolume get grouped into a Color" 
            + k_FloatSuffix);
        static readonly GUIContent GroupVector2sContent = new GUIContent("Group Vector2s", 
            "If enabled, x/y messages belonging to the same 2d vector in Resolume get grouped into a Vector2 " 
            + k_FloatSuffix);
        static readonly GUIContent GroupVector3sContent = new GUIContent("Group Vector3s", 
            "If enabled, x/y/z messages belonging to the same 3d vector in Resolume get grouped into a Vector3" 
            + k_FloatSuffix);

        static bool s_GroupingFoldoutState = true;

        [SettingsProvider]
        public static SettingsProvider CreateResolinkSettingsProvider()
        {
            return new SettingsProvider("Project/Resolink", SettingsScope.User)
            {
                label = "Resolink",
                guiHandler = (searchContext) =>
                {
                    var settings = ResolinkEditorSettings.GetSerializedSettings();
                    settings.Update();
                    EditorGUILayout.PropertyField(settings.FindProperty("m_ShowHelp"), ShowHelpContent);
                    EditorGUILayout.PropertyField(settings.FindProperty("m_WarnOnUnknownType"), TypeWarningContent);

                    s_GroupingFoldoutState = EditorGUILayout.Foldout(s_GroupingFoldoutState, "Complex Data Types");
                    if (s_GroupingFoldoutState)
                    {
                        using (new EditorGUI.IndentLevelScope())
                        {
                            var colorProp = settings.FindProperty("m_GroupColors");
                            EditorGUILayout.PropertyField(colorProp, GroupColorsContent);
                            var vec2Prop = settings.FindProperty("m_GroupVector2s");
                            EditorGUILayout.PropertyField(vec2Prop, GroupVector2sContent);
                            var vec3Prop = settings.FindProperty("m_GroupVector3s");
                            EditorGUILayout.PropertyField(vec3Prop, GroupVector3sContent);
                        }
                    }

                    settings.ApplyModifiedProperties();
                },

                keywords = new HashSet<string>(new[] { "Resolink", "Resolume", "OSC" })
            };
        }
    }
}