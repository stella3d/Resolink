using UnityEditor;
using UnityEngine;

namespace Resolink
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(RenderSharing))]
    public class RenderSharingEditor : Editor
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        const string SpoutSyphonComponentName = "SpoutSender";
#elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
        const string SpoutSyphonComponentName = "SyphonServer";
#else
        const string SpoutSyphonComponentName = "";
#endif

        const string NdiComponentName = "NDI Sender";
        
        SerializedProperty m_CameraProperty;
        SerializedProperty m_ProtocolProperty;

        VideoSharingProtocol m_PreviousProtocol;

        static string s_HelpText;

        RenderSharing m_Component;
        
        public void OnEnable()
        {
            m_Component = (RenderSharing) target;
            m_CameraProperty = serializedObject.FindProperty("CameraToShare");
            m_ProtocolProperty = serializedObject.FindProperty("m_VideoProtocol");
            s_HelpText = GetHelpText();
        }
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorUtils.Help(s_HelpText);
            
            EditorGUILayout.PropertyField(m_ProtocolProperty);
            var protocol = (VideoSharingProtocol) m_ProtocolProperty.enumValueIndex;
            if (protocol != m_PreviousProtocol)
            {
                Debug.Log("change protocols ?");
                s_HelpText = GetHelpText();
                serializedObject.ApplyModifiedProperties();
                m_Component.EnsureSendingComponent();
            }

            EditorGUILayout.PropertyField(m_CameraProperty);
            
            serializedObject.ApplyModifiedProperties();
            m_PreviousProtocol = (VideoSharingProtocol) m_ProtocolProperty.enumValueIndex;
        }

        string GetHelpText()
        {
            const string HelpText = "This component ensures that a {0} component has been " +
                                    "attached to the scene's active Camera object, to share video out of Unity.";
            
            var protocol = (VideoSharingProtocol) m_ProtocolProperty.enumValueIndex;
            var componentLabel = protocol == VideoSharingProtocol.NDI ? NdiComponentName : SpoutSyphonComponentName;
            return string.Format(HelpText, componentLabel);
        }
    }
}