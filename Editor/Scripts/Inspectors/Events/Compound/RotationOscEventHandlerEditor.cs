using UnityEngine;
using UnityEditor;

namespace Resolink
{
    [CustomEditor(typeof(RotationOscEventHandler))]
    public class RotationOscEventHandlerEditor : CompoundOscEventHandlerEditor
        <RotationOscEventHandler, FloatOscActionHandler, QuaternionUnityEvent, Quaternion, float>
    {
        bool m_FoldState;
        
        public override void OnEnable()
        {
            base.OnEnable();
            OverridePropertyDrawer = true;
        }

        protected override void DrawDebugUI()
        {
            using (new EditorGUI.DisabledScope(true))
            {
                var rotation = m_Component.Value;
                EditorGUILayout.FloatField("X", rotation.x);
                EditorGUILayout.FloatField("Y", rotation.y);
                EditorGUILayout.FloatField("Z", rotation.z);
                EditorGUILayout.FloatField("W", rotation.w);
            }
        }
        
        protected override void DrawValue(string label, Quaternion rotation)
        {
            m_FoldState = EditorGUILayout.Foldout(m_FoldState, label);
            if (!m_FoldState)
                return;
            
            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.FloatField("X", rotation.x);
                EditorGUILayout.FloatField("Y", rotation.y);
                EditorGUILayout.FloatField("Z", rotation.z);
                EditorGUILayout.FloatField("W", rotation.w);
            }
        }
    }
}