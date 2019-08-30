using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEditor;
using UnityEngine;

namespace Resolunity
{
    public class MapParserWindow : EditorWindow
    {
        [MenuItem("Resolume/Map Parser")]
        static void InitWindow()
        {
            s_OscMapPath = OscMapParser.DefaultAvenuePath;
            MapParserWindow window = (MapParserWindow) GetWindow(typeof(MapParserWindow));
            window.Show();
        }

        static string s_OscMapPath;
        string OutputPath = "Assets/Unity-Resolume/Map.asset";
        string HandlerOutputPath = "Assets/Unity-Resolume/ExampleEventHandler.asset";

        ResolumeOscMap m_MapToUpdate;
        ResolumeType m_ResolumeType;

        public void OnGUI()
        {
            using (new GUILayout.HorizontalScope())
            {
                EditorGUILayout.PrefixLabel("Resolume Type");
                m_ResolumeType = (ResolumeType) EditorGUILayout.EnumPopup(m_ResolumeType);
            }
            using (new GUILayout.HorizontalScope())
            {
                EditorGUILayout.PrefixLabel("Resolume OSC File");
                s_OscMapPath = GetMapPath();
            }
            
            EditorGUILayout.LabelField(s_OscMapPath);
            EditorGUILayout.Space();

            OutputPath = EditorGUILayout.TextField("Asset Creation Path", OutputPath);
            EditorGUILayout.Space();

            if (GUILayout.Button("Create New OSC Map Asset"))
            {
                var parserGuids = AssetDatabase.FindAssets("t: OscMapParser");
                var parserPath = AssetDatabase.GUIDToAssetPath(parserGuids[0]);
                var parser = AssetDatabase.LoadAssetAtPath<OscMapParser>(parserPath);
                
                parser.OutputPath = OutputPath;
                parser.ParseDefaultFile();
            }
            
            EditorGUILayout.Space();
            EditorGUILayout.Space();

#pragma warning disable 618
            m_MapToUpdate = (ResolumeOscMap) EditorGUILayout.ObjectField(m_MapToUpdate, typeof(ResolumeOscMap));
#pragma warning restore 618

            if (GUILayout.Button("Update OSC Map Asset"))
            {
                Debug.Log("updating not implemented yet");
            }
        }

        public string GetMapPath()
        {
            if (GUILayout.Button("Select File"))
                return EditorUtility.OpenFilePanel("Select Resolume OSC map", OscMapParser.DefaultAvenuePath, "xml");

            return string.IsNullOrEmpty(s_OscMapPath)? GetDefaultMapPath() : s_OscMapPath;
        }
        
        public string GetDefaultMapPath()
        {
            return m_ResolumeType == ResolumeType.Avenue 
                ? $"~{OscMapParser.DefaultAvenuePath}" 
                : $"~{OscMapParser.DefaultArenaPath}";
        }
    }
}