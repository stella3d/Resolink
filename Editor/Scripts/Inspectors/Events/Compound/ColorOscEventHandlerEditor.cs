using UnityEditor;
using UnityEngine;

namespace Resolink
{
    [CustomEditor(typeof(ColorOscEventHandler))]
    public class ColorOscEventHandlerEditor : CompoundOscEventHandlerEditor
        <ColorOscEventHandler, FloatOscActionHandler, ColorUnityEvent, Color, float>
    {
        protected override void DrawDebugUI()
        {
            using (new EditorGUI.DisabledScope(true))
            {
                var color = m_Component.Value;
                EditorGUILayout.FloatField("R", ToFloat255(color.r));
                EditorGUILayout.FloatField("G", ToFloat255(color.g));
                EditorGUILayout.FloatField("B", ToFloat255(color.b));
                EditorGUILayout.FloatField("A", ToFloat255(color.a));
            }
        }
    }
}