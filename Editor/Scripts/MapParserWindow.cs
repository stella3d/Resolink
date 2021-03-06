﻿using System;
using UnityEditor;
using UnityEngine;

namespace Resolink
{
    public class MapParserWindow : EditorWindow
    {
        static readonly string s_UserPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        static string s_DefaultFilePath;
        static string s_OscMapPath;

        string m_OutputDirectory;
        
        static ResolumeType s_ResolumeType;
        static ResolumeType s_PreviousResolumeType;
        
        readonly GUIContent m_ResolumeTypeContent = new GUIContent("Resolume Type", "Changes the default map path");

        readonly GUILayoutOption m_SmallColumnWidth = GUILayout.Width(100);

        [MenuItem("Window/Resolink/Map Parser")]
        static void InitWindow()
        {
            s_OscMapPath = GetDefaultMapPath();
            MapParserWindow window = (MapParserWindow) GetWindow(typeof(MapParserWindow));
            window.titleContent = new GUIContent("Resolink Parser", "Used to create a new OSC Map asset");
            window.Show();
        }

        public void OnGUI()
        {
            using (new GUILayout.HorizontalScope())
            {
                EditorGUILayout.PrefixLabel(m_ResolumeTypeContent);
                s_PreviousResolumeType = s_ResolumeType;
                s_ResolumeType = (ResolumeType) EditorGUILayout.EnumPopup(s_ResolumeType, m_SmallColumnWidth);
                if (s_PreviousResolumeType != s_ResolumeType)
                    s_OscMapPath = "";
            }
            
            s_OscMapPath = GetMapPath();
            s_OscMapFileName = GetFileName(s_OscMapPath);
            
            EditorGUILayout.LabelField(s_OscMapPath);
            EditorGUILayout.Space();

            var valid = s_OscMapPath.EndsWith(".xml");
            using (new EditorGUI.DisabledScope(!valid))
            {
                if (GUILayout.Button("Create OSC Map Asset"))
                {
                    const string label = "Select Asset Creation Path";
                    s_AssetPath = ProjectRelative(
                        EditorUtility.SaveFilePanel(label, Application.dataPath, s_OscMapFileName, "asset"));
                    
                    if (s_AssetPath == "" || !s_AssetPath.Contains("Assets"))
                    {
                        Debug.LogWarning("invalid asset creation path chosen");
                        return;
                    }

                    var parser = OscMapParser.LoadAsset();
                    parser.OutputPath = s_AssetPath;
                    parser.ParseFile(s_OscMapPath);
                }
            }
        }

        string ProjectRelative(string fullPath)
        {
            var assetsIndex = fullPath.IndexOf("/Assets", StringComparison.Ordinal);
            if (assetsIndex == -1) return "";
            assetsIndex += 1;
            return fullPath.Substring(assetsIndex, fullPath.Length - assetsIndex).Replace(" ", "");
        }

        public string GetMapPath()
        {
            if (GUILayout.Button("Select Resolume OSC Map File"))
                return EditorUtility.OpenFilePanel("Select Resolume OSC map", s_DefaultFilePath, "xml");

            return string.IsNullOrEmpty(s_OscMapPath) ? GetDefaultMapPath() : s_OscMapPath;
        }

        static string s_AssetPath;
        
        public static string GetDefaultMapPath()
        {
            s_DefaultFilePath = s_ResolumeType == ResolumeType.Avenue 
                ? $"{s_UserPath}{OscMapParser.DefaultAvenuePath}" 
                : $"{s_UserPath}{OscMapParser.DefaultArenaPath}";

            return s_DefaultFilePath;
        }
        
        static string s_OscMapFileName;

        public static string GetFileName(string fullPath)
        {
            var lastSplit = fullPath.LastIndexOf('/');
            var lastPeriod = fullPath.LastIndexOf('.');
            if (lastSplit == -1 || lastPeriod == -1) return "";
            return fullPath.Substring(lastSplit + 1, lastPeriod - lastSplit - 1).Replace(" ", "");
        }
    }
}