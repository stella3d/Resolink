#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
using Klak.Spout;
#elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
using Klak.Syphon;
#endif
using Klak.Ndi;
using UnityEngine;

namespace Resolink
{
    [DisallowMultipleComponent]
    [ExecuteAlways]
    public class CameraRenderSharing : MonoBehaviour
    {
        [Tooltip("The camera to share a render texture to Resolume from. Main Camera if not specified")]
        public Camera CameraToShare;

#pragma warning disable 649
        [Tooltip("The protocol to use to transmit video out of Unity")]
        [SerializeField] 
        VideoSharingProtocol m_VideoProtocol;
#pragma warning restore 649
        
        VideoSharingProtocol VideoProtocol => m_VideoProtocol;

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
            switch (m_VideoProtocol)
            {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
                case VideoSharingProtocol.Spout:
                {
                    CameraToShare.gameObject.RemoveComponent<NdiSender>();
                    CameraToShare.AddComponentIfAbsent<SpoutSender>();
                    break;
                }
                case VideoSharingProtocol.NDI:
                {
                    CameraToShare.gameObject.RemoveComponent<SpoutSender>();
                    CameraToShare.AddComponentIfAbsent<NdiSender>();
                    break;
                }
#elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
                case VideoSharingProtocol.Syphon:
                {
                    CameraToShare.gameObject.RemoveComponent<NdiSender>();
                    CameraToShare.AddComponentIfAbsent<SyphonServer>();
                    break;
                }
                case VideoSharingProtocol.NDI:
                {
                    CameraToShare.gameObject.RemoveComponent<SyphonServer>();
                    CameraToShare.AddComponentIfAbsent<NdiSender>();
                    break;
                }
#else    
                // linux isn't supported by resolink yet, but should be eventually, via NDI
                case VideoSharingProtocol.NDI:
                {
                    CameraToShare.gameObject.RemoveComponent<SyphonServer>();
                    CameraToShare.AddComponentIfAbsent<NdiSender>();
                    break;
                }
#endif
            }
        }
    }
}