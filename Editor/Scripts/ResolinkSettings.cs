using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Resolink
{
    [CreateAssetMenu]
    class ResolinkSettings : ScriptableObject
    {
        public static ResolinkSettings Instance { get; private set; }
        
        public const string k_ResolinkSettingsPath = "Assets/ResolinkSettings.asset";

        [SerializeField]
        bool m_ShowHelp;

        public bool ShowHelp => m_ShowHelp;

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
                AssetDatabase.CreateAsset(settings, k_ResolinkSettingsPath);
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
                    EditorGUILayout.PropertyField(settings.FindProperty("m_ShowHelp"), ShowHelpContent);
                },

                // Populate the search keywords to enable smart search filtering and label highlighting:
                keywords = new HashSet<string>(new[] { "Resolink" })
            };

            return provider;
        }
    }
}