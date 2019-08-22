using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UnityResolume
{
    //[CustomEditor(typeof(OscEventHandler))]
    public class OscEventEditor<TEvent, T> : Editor
    {
        TEvent m_Event;
        ResolumeOscShortcut m_Shortcut;

        public void OnEnable()
        {
            var genericEvent = (OscEventHandler) target;
        }

        public override void OnInspectorGUI()
        {
            DrawShortcut(m_Shortcut);
        }

        public void DrawShortcut(ResolumeOscShortcut shortcut)
        {
            EditorGUILayout.LabelField("Input Path", shortcut.Input.Path);
            EditorGUILayout.LabelField("Output Path", shortcut.Output.Path);

            if (shortcut.SubTargets == null || shortcut.SubTargets.Length == 0)
            {
                EditorGUILayout.EndFoldoutHeaderGroup();
                return;
            }

            DrawSubTargetsIfAny(shortcut.SubTargets);
            DrawEvents(shortcut);
            
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        void DrawEvents(ResolumeOscShortcut shortcut)
        {
            if (shortcut.DataType == typeof(int))
            {
            }
            else if (shortcut.DataType == typeof(float))
            {
                
            }
        }
        
        static void DrawSubTarget(SubTarget subTarget)
        {
            EditorGUILayout.LabelField("Sub Target");
            using (new EditorGUI.IndentLevelScope())
            {
                EditorGUILayout.LabelField("Option Index", subTarget.OptionIndex.ToString());
                EditorGUILayout.LabelField("Type", subTarget.Type.ToString());
            }
        }
        
        static void DrawSubTargets(SubTarget[] subTargets)
        {
            EditorGUILayout.LabelField("Sub Targets");
            using (new EditorGUI.IndentLevelScope())
            {
                foreach (var subTarget in subTargets)
                {
                    EditorGUILayout.PrefixLabel("Option Index " + subTarget.OptionIndex);
                    using (new EditorGUI.IndentLevelScope())
                    {
                        EditorGUILayout.LabelField("Type", subTarget.Type.ToString());
                    }
                }
            }
        }
        
        static void DrawSubTargetsIfAny(SubTarget[] subTargets)
        {
            if(subTargets.Length == 1)
                DrawSubTarget(subTargets[0]);
            else
                DrawSubTargets(subTargets);
        }
    }
}