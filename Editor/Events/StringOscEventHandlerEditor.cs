using UnityEditor;

namespace Resolunity
{
    [CustomEditor(typeof(StringOscEventHandler))]
    public class StringOscEventHandlerEditor : OscEventHandlerEditor<StringOscEventHandler, StringUnityEvent, string> { }
}