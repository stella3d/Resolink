using UnityEditor;

namespace Resolink
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(CameraRenderSharing))]
    public class CameraRenderSharingEditor : Editor
    {
        const string k_HelpText = "This component ensures that the right component to share video out of Unity " +
                                  "has been attached to the Camera object.";

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