using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnityResolume
{
    public class MapParserWindow : EditorWindow
    {
        [MenuItem("Resolume/Map Parser")]
        static void InitWindow()
        {
            MapParserWindow window = (MapParserWindow) GetWindow(typeof(MapParserWindow));
            window.Show();
        }

        string m_OscMapPath = OscMapParser.DefaultAvenuePath;
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
                m_OscMapPath = GetMapPath();
            }
            
            EditorGUILayout.LabelField(m_OscMapPath);
            EditorGUILayout.Space();

            OutputPath = EditorGUILayout.TextField("Asset Creation Path", OutputPath);
            EditorGUILayout.Space();

            if (GUILayout.Button("Create New OSC Map Asset"))
            {
                var parser = new OscMapParser { OutputPath = OutputPath };
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
            
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            
            HandlerOutputPath = EditorGUILayout.TextField("Event Asset Creation Path", HandlerOutputPath);

            if (GUILayout.Button("Create Example shortcut event"))
            {
                var eventObj = new GameObject("OSC Event Handlers");
                var component = eventObj.AddComponent<FloatOscEventHandler>();
            }
        }

        public string GetMapPath()
        {
            if (GUILayout.Button("Select File"))
                return EditorUtility.OpenFilePanel("Select Resolume OSC map", OscMapParser.DefaultAvenuePath, ".xml");

            return m_ResolumeType == ResolumeType.Avenue 
                ? $"~{OscMapParser.DefaultAvenuePath}" 
                : $"~{OscMapParser.DefaultArenaPath}";
        }
    }
}