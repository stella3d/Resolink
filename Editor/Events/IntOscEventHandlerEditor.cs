using UnityEditor;

namespace Resolunity
{
    [CustomEditor(typeof(IntOscEventHandler))]
    public class IntOscEventHandlerEditor : OscEventHandlerEditor<IntOscEventHandler, IntUnityEvent, int>
    {
    }
}