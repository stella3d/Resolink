using UnityEditor;

namespace Resolink
{
    [CustomEditor(typeof(StringOscEventHandler))]
    public class StringOscEventHandlerEditor : OscEventHandlerEditor<StringOscEventHandler, StringUnityEvent, string> 
    {
        protected override void DrawValue()
        {
            m_Component.Value = EditorGUILayout.DelayedTextField(m_Component.Value);
        }
    }
}