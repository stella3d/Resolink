using System.Linq;
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
            if(ResolinkEditorSettings.Instance.ShowHelp)
                EditorGUILayout.HelpBox(message, type);
        }

        public static T[] LoadAllAssets<T>() where T: Object
        {
            var search = $"t: {typeof(T).Name}";
            var paths = AssetDatabase.FindAssets(search).Select(AssetDatabase.GUIDToAssetPath).ToArray();

            var loaded = new T[paths.Length];
            for (var i = 0; i < paths.Length; i++)
                loaded[i] = AssetDatabase.LoadAssetAtPath<T>(paths[i]);

            return loaded;
        }
    }
}