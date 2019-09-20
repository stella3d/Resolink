using UnityEditor;
using UnityEngine;

namespace Resolink
{
    [CustomEditor(typeof(OscRouter))]
    public class OscRouterEditor : Editor
    {
        const string k_HelpText = "This component handles routing OSC messages to your events, based on address.";

        readonly GUIContent m_HandlerCountLabelContent = new GUIContent("Registered Handler Count", 
            "The number of addresses that event handlers have been registered for");

        SerializedProperty m_PortProperty;

        OscRouter m_Router;

        public void OnEnable()
        {
            m_Router = (OscRouter) target;
            m_PortProperty = serializedObject.FindProperty("m_Port");
        }

        public override void OnInspectorGUI()
        {
            EditorUtils.Help(k_HelpText);
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField(m_HandlerCountLabelContent);
                EditorGUILayout.LabelField(m_Router.AddressHandlers.Count.ToString());
            }

            using (new EditorGUI.DisabledScope(EditorApplication.isPlayingOrWillChangePlaymode))
            {
                EditorGUILayout.PropertyField(m_PortProperty);
            }
        }
    }
}