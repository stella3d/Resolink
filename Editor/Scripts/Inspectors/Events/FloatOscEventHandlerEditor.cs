using UnityEditor;

namespace Resolink
{
    [CustomEditor(typeof(FloatOscEventHandler))]
    public class FloatOscEventHandlerEditor : OscEventHandlerEditor<FloatOscEventHandler, FloatUnityEvent, float> 
    {
        protected override void DrawValue()
        {
            var previousValue = m_Component.Value;
            var newValue = EditorGUILayout.Slider("Value", m_Component.Value, 0f, 1f);
            if (newValue != previousValue)
            {
                m_Component.Value = newValue;
            }
        }
    }
}