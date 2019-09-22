using UnityEditor;

namespace Resolink
{
    [CustomEditor(typeof(BooleanOscEventHandler))]
    public class BoolOscEventHandlerEditor : OscEventHandlerEditor<BooleanOscEventHandler, BoolUnityEvent, bool> 
    {
        protected override void DrawValue()
        {
            m_Component.Value = EditorGUILayout.Toggle(m_Component.Value);
        }
    }
}