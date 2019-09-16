#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
using Klak.Spout;
#elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
using Klak.Syphon;
#endif
using UnityEngine;

namespace Resolink
{
    [DisallowMultipleComponent]
    [ExecuteAlways]
    public class CameraRenderSharing : MonoBehaviour
    {
        [Tooltip("The camera to share a render texture to Resolume from. Main Camera if not specified")]
        public Camera CameraToShare;

        void OnEnable()
        {
            GetCameraRef();
            EnsureSendingComponent();
        }

        void Start()
        {
            GetCameraRef();
            EnsureSendingComponent();
        }

        void GetCameraRef()
        {
            if (CameraToShare == null)
                CameraToShare = Camera.main;
            if (CameraToShare == null)
                CameraToShare = Camera.current;
        }

        public void EnsureSendingComponent()
        {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
            if (CameraToShare.GetComponent<SpoutSender>() == null)
                CameraToShare.gameObject.AddComponent<SpoutSender>();
#elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
            if (CameraToShare.GetComponent<SyphonServer>() == null)
                CameraToShare.gameObject.AddComponent<SyphonServer>();    
#endif            
        }
    }
}