using System;
using UnityEditor;
using UnityEngine;

namespace Resolink
{
    public class MapParserWindow : EditorWindow
    {
        static string s_OscMapPath;
        string OutputPath = "Assets/Resolunity/Map.asset";

        ResolumeType m_ResolumeType;
        
        readonly GUIContent m_ResolumeTypeContent = new GUIContent("Resolume Type", "Changes the default map path");

        GUILayoutOption m_SmallColumnWidth;

        [MenuItem("Resolume/Map Parser")]
        static void InitWindow()
        {
            s_OscMapPath = OscMapParser.DefaultAvenuePath;
            MapParserWindow window = (MapParserWindow) GetWindow(typeof(MapParserWindow));
            window.Show();
        }

        public void OnGUI()
        {
            m_SmallColumnWidth = GUILayout.Width(100);
            
            using (new GUILayout.HorizontalScope())
            {
                EditorGUILayout.PrefixLabel(m_ResolumeTypeContent);
                m_ResolumeType = (ResolumeType) EditorGUILayout.EnumPopup(m_ResolumeType, m_SmallColumnWidth);
            }
            
            s_OscMapPath = GetMapPath();
            EditorGUILayout.LabelField(s_OscMapPath);
            EditorGUILayout.Space();

            OutputPath = EditorGUILayout.TextField("Asset Creation Path", OutputPath);
            EditorGUILayout.Space();

            using (new EditorGUI.DisabledScope(!s_OscMapPath.EndsWith(".xml")))
            {
                if (GUILayout.Button("Create New OSC Map Asset"))
                {
                    var parser = OscMapParser.LoadAsset();
                    parser.OutputPath = OutputPath;
                    parser.ParseFile(s_OscMapPath);
                }
            }
        }

        public string GetMapPath()
        {
            if (GUILayout.Button("Select Resolume OSC Map File"))
                return EditorUtility.OpenFilePanel("Select Resolume OSC map", OscMapParser.DefaultAvenuePath, "xml");

            var userPath = string.IsNullOrEmpty(s_OscMapPath) ? GetDefaultMapPath() : s_OscMapPath;
            var folder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var replaced = userPath.Replace("~", folder);
            return replaced;
        }
        
        public string GetDefaultMapPath()
        {
            return m_ResolumeType == ResolumeType.Avenue 
                ? $"~{OscMapParser.DefaultAvenuePath}" 
                : $"~{OscMapParser.DefaultArenaPath}";
        }
    }
}