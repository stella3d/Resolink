using UnityEditor;

namespace UnityResolume
{
    [CustomEditor(typeof(IntOscEventHandler))]
    public class IntOscEventHandlerEditor : OscEventHandlerEditor<IntOscEventHandler, IntEvent, int>
    {
    }
}