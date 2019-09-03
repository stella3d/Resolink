using Klak.Spout;
using UnityEngine;

namespace Resolink
{
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
            var foundComponent = CameraToShare.GetComponent<SpoutSender>();
            if (foundComponent == null)
            {
                CameraToShare.gameObject.AddComponent<SpoutSender>();
            }
#endif
        }
    }
}