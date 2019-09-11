using UnityEditor;

namespace Resolink
{
    [CustomEditor(typeof(OscRouter))]
    public class OscRouterEditor : Editor
    {
        const string k_HelpText = "This component handles routing OSC messages to your events, based on address, " +
                                  "as well queueing them on the main thread.";
        
        public override void OnInspectorGUI()
        {
            EditorUtils.Help(k_HelpText);
        }
    }
}