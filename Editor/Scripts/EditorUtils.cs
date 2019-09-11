using UnityEditor;
using UnityEngine;

namespace Resolink
{
    public static class EditorUtils
    {
        public static void DrawBoxLine()
        {
            GUILayout.Box("", GUILayout.Height(1), GUILayout.ExpandWidth(true));
        }

        public static void Help(string message, MessageType type = MessageType.Info)
        {
            if(ResolinkSettings.Instance.ShowHelp)
                EditorGUILayout.HelpBox(message, type);
        }
    }
}