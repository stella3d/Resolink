using UnityEngine;

namespace Resolink
{
    public static class EditorUtils
    {
        public static void DrawBoxLine()
        {
            GUILayout.Box("", GUILayout.Height(1), GUILayout.ExpandWidth(true));
        }
    }
}