using UnityEditor;

namespace Resolink
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(OscRouter))]
    public class OscRouterEditor : Editor
    {
        const string k_HelpText = "This component handles routing OSC messages to your events, based on address.\n" +
                                  "It also handles queueing event callbacks to happen on the main thread.";
        
        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox(k_HelpText, MessageType.Info);
        }
    }
}