using UnityEditor;

namespace Resolunity
{
    [CustomEditor(typeof(BooleanOscEventHandler))]
    public class BoolOscEventHandlerEditor : OscEventHandlerEditor<BooleanOscEventHandler, BoolUnityEvent, bool> { }
}