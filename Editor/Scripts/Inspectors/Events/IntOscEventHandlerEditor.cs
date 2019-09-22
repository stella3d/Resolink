using UnityEditor;

namespace Resolink
{
    [CustomEditor(typeof(IntOscEventHandler))]
    public class IntOscEventHandlerEditor : OscEventHandlerEditor<IntOscEventHandler, IntUnityEvent, int> 
    {
        protected override void DrawValue()
        {
            m_Component.Value = EditorGUILayout.IntField(m_Component.Value);
        }
    }
}