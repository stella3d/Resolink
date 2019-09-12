using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Resolink
{
    [CreateAssetMenu]
    class ResolinkSettings : ScriptableObject
    {
        static ResolinkSettings s_Instance;
        
        public static ResolinkSettings Instance
        {
            get
            {
                if (s_Instance == null)
                    s_Instance = GetOrCreateSettings();

                return s_Instance;
            }
            private set => s_Instance = value;
        }
        
        public const string k_ResolinkSettingsFallbackPath = "Assets/ResolinkSettings.asset";

#pragma warning disable 649
        [SerializeField]
        
        bool m_ShowHelp;
        
        [SerializeField] 
        bool m_WarnOnUnknownType;
#pragma warning restore 649

        public bool ShowHelp => m_ShowHelp;
        public bool WarnOnUnknownType => m_WarnOnUnknownType;

        public void OnEnable()
        {
            Instance = this;
        }

        internal static ResolinkSettings GetOrCreateSettings()
        {
            var settings = GetSettingsFromAssets();
            if (settings == null)
            {
                settings = CreateInstance<ResolinkSettings>();
                AssetDatabase.CreateAsset(settings, k_ResolinkSettingsFallbackPath);
                AssetDatabase.SaveAssets();
            }
            return settings;
        }

        internal static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(GetOrCreateSettings());
        }

        static ResolinkSettings GetSettingsFromAssets()
        {
            const string search = "t: ResolinkSettings";
            var guids = AssetDatabase.FindAssets(search);
            var firstPath = AssetDatabase.GUIDToAssetPath(guids[0]);
            return AssetDatabase.LoadAssetAtPath<ResolinkSettings>(firstPath);
        }
    }

    static class ResolinkSettingsIMGUIRegister
    {
        static readonly GUIContent ShowHelpContent = new GUIContent("Show Help", 
            "If enabled, help boxes are shown on most Resolink components");
        
        static readonly GUIContent TypeWarningContent = new GUIContent("Warn on Unknown Type", 
            "If enabled, warnings will be shown in the console for Resolume shortcuts we don't know the data type for");
        
        [SettingsProvider]
        public static SettingsProvider CreateResolinkSettingsProvider()
        {
            // First parameter is the path in the Settings window.
            var provider = new SettingsProvider("Project/Resolink", SettingsScope.User)
            {
                // By default the last token of the path is used as display name if no label is provided.
                label = "Resolink",
                // Create the SettingsProvider and initialize its drawing (IMGUI) function in place:
                guiHandler = (searchContext) =>
                {
                    var settings = ResolinkSettings.GetSerializedSettings();
                    settings.Update();
                    EditorGUILayout.PropertyField(settings.FindProperty("m_ShowHelp"), ShowHelpContent);
                    EditorGUILayout.PropertyField(settings.FindProperty("m_WarnOnUnknownType"), TypeWarningContent);
                    settings.ApplyModifiedProperties();
                },

                // Populate the search keywords to enable smart search filtering and label highlighting:
                keywords = new HashSet<string>(new[] { "Resolink" })
            };

            return provider;
        }
    }
}