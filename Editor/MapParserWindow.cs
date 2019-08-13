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

        public string OutputPath = "Assets/Resolume-Unity/Map.asset";

        ResolumeOscMap m_MapToUpdate;
        ResolumeType m_ResolumeType;

        public void OnGUI()
        {
            using (new GUILayout.HorizontalScope())
            {
                EditorGUILayout.PrefixLabel("Resolume Type");
                m_ResolumeType = (ResolumeType) EditorGUILayout.EnumPopup(m_ResolumeType);
            }

            OutputPath = EditorGUILayout.TextField("Asset Path", OutputPath);

            if (GUILayout.Button("New OSC Map Asset"))
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
            
        }
    }
}