using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Resolink
{
    public class MapParserWindow : EditorWindow
    {
        static string s_UserPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        static string s_DefaultFilePath;
        static string s_OscMapPath;

        string PreviousOutPath;
        string OutputPath = "Assets/Resolink/DefaultOscMap.asset";

        string m_OutputDirectory;
        bool m_OutputDirExists = true;
        
        static ResolumeType s_ResolumeType;
        static ResolumeType s_PreviousResolumeType;
        
        readonly GUIContent m_ResolumeTypeContent = new GUIContent("Resolume Type", "Changes the default map path");

        readonly GUILayoutOption m_SmallColumnWidth = GUILayout.Width(100);

        [MenuItem("Window/Resolink/Map Parser")]
        static void InitWindow()
        {
            s_OscMapPath = GetDefaultMapPath();
            MapParserWindow window = (MapParserWindow) GetWindow(typeof(MapParserWindow));
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
            EditorGUILayout.LabelField(s_OscMapPath);
            EditorGUILayout.Space();

            PreviousOutPath = OutputPath;
            OutputPath = EditorGUILayout.TextField("Asset Creation Path", OutputPath);
            if (OutputPath != PreviousOutPath)
                m_OutputDirExists = OutputDirectoryExists();
            
            if (!m_OutputDirExists)
            {
                var message = $"output directory {m_OutputDirectory} does not exist!";
                EditorGUILayout.HelpBox(message, MessageType.Error);
            }

            var outPathStartValid = OutputPath.StartsWith("Assets/");
            if (!outPathStartValid && m_OutputDirExists)
            {
                const string message = "Asset creation path must start with Assets/";
                EditorGUILayout.HelpBox(message, MessageType.Warning);
            }

            var outPathEndValid = OutputPath.EndsWith(".asset");
            if (!outPathEndValid)
            {
                const string message = "Asset creation path must end with .asset";
                EditorGUILayout.HelpBox(message, MessageType.Warning);
            }
            
            EditorGUILayout.Space();

            var valid = s_OscMapPath.EndsWith(".xml") && m_OutputDirExists && outPathStartValid && outPathEndValid;
            using (new EditorGUI.DisabledScope(!valid))
            {
                if (GUILayout.Button("Create New OSC Map Asset"))
                {
                    var parser = OscMapParser.LoadAsset();
                    parser.OutputPath = OutputPath;
                    parser.ParseFile(s_OscMapPath);
                }
            }
        }

        bool OutputDirectoryExists()
        {
            var assetsRemoved = OutputPath.Replace("Assets", "");
            var lastSplit = assetsRemoved.LastIndexOf('/');
            var directoryPath = (Application.dataPath + assetsRemoved.Substring(0, lastSplit));
            m_OutputDirectory = directoryPath;
            return Directory.Exists(m_OutputDirectory);
        }

        public string GetMapPath()
        {
            if (GUILayout.Button("Select Resolume OSC Map File"))
                return EditorUtility.OpenFilePanel("Select Resolume OSC map", s_DefaultFilePath, "xml");

            return string.IsNullOrEmpty(s_OscMapPath) ? GetDefaultMapPath() : s_OscMapPath;
        }
        
        public static string GetDefaultMapPath()
        {
            s_DefaultFilePath = s_ResolumeType == ResolumeType.Avenue 
                ? $"{s_UserPath}{OscMapParser.DefaultAvenuePath}" 
                : $"{s_UserPath}{OscMapParser.DefaultArenaPath}";

            return s_DefaultFilePath;
        }
    }
}