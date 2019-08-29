using UnityEditor;

namespace Resolunity
{
    [CustomEditor(typeof(FloatOscEventHandler))]
    public class FloatOscEventHandlerEditor : OscEventHandlerEditor<FloatOscEventHandler, FloatUnityEvent, float>
    {
    }
}