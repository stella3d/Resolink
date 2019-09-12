using UnityEditor;

namespace Resolink
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(CameraRenderSharing))]
    public class CameraRenderSharingEditor : Editor
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        const string CameraComponentName = "SpoutSender";
#elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
        const string CameraComponentName = "SyphonServer";
#else
        const string CameraComponentName = "UNSUPPORTED PLATFORM";
#endif
        
        const string k_HelpText = "This component ensures that a " + CameraComponentName + " component has been " +
                                  "attached to the scene's active Camera object, to share video out of Unity.";

        SerializedProperty m_CameraProperty;
        
        public void OnEnable()
        {
            m_CameraProperty = serializedObject.FindProperty("CameraToShare");
        }
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorUtils.Help(k_HelpText);
            EditorGUILayout.PropertyField(m_CameraProperty);
            serializedObject.ApplyModifiedProperties();
        }
    }
}