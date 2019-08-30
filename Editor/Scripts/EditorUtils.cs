using UnityEngine;

namespace Resolunity
{
    public static class EditorUtils
    {
        public static void DrawBoxLine()
        {
            GUILayout.Box("", GUILayout.Height(1), GUILayout.ExpandWidth(true));
        }
    }
}