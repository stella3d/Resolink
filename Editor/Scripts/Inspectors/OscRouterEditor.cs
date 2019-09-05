using UnityEditor;

namespace Resolink
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(OscRouter))]
    public class OscRouterEditor : Editor
    {
        const string k_HelpText = "This component handles routing OSC messages to your events, based on address, " +
                                  "as well queueing them on the main thread.\n" +
                                  "The EventReceiver component on this object just gets the OSC connection going.";
        
        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox(k_HelpText, MessageType.Info);
        }
    }
}