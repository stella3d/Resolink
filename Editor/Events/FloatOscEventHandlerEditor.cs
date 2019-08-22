using UnityEditor;

namespace UnityResolume
{
    [CustomEditor(typeof(FloatOscEventHandler))]
    public class FloatOscEventHandlerEditor : OscEventHandlerEditor<FloatOscEventHandler, FloatEvent, float>
    {
    }
}