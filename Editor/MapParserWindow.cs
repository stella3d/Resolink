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

        public void OnGUI()
        {
            OutputPath = EditorGUILayout.TextField("Asset Path", OutputPath);
            
            if (GUILayout.Button("parse default file"))
            {
                var parser = new OscMapParser { OutputPath = OutputPath };
                parser.ParseDefaultFile();
            }
        }
    }
}