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
            serializedObject.Update();
            
            EditorUtils.Help(k_HelpText);
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField(m_HandlerCountLabelContent);
                string numberLabel = m_Router.Server?.AddressSpace != null
                    ? m_Router.Server.AddressSpace.HandlerCount.ToString() : "0";

                EditorGUILayout.LabelField(numberLabel);
            }

            using (new EditorGUI.DisabledScope(EditorApplication.isPlayingOrWillChangePlaymode))
            {
                EditorGUILayout.PropertyField(m_PortProperty);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}