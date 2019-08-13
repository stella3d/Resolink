using System.Collections;
using System.Collections.Generic;
using OscJack;
using UnityEngine;

namespace UnityResolume
{
    public class TimeCube : MonoBehaviour
    {
        [SerializeField] OscEventReceiver m_Receiver;

        bool m_PauseQueued;
        bool m_UnpauseQueued;
        bool m_Paused;
        
        void Awake()
        {

        }

        float m_PreviousTimeScale;

        void Update()
        {
            if (m_PauseQueued)
            {
                m_PreviousTimeScale = Time.timeScale;
                Time.timeScale = 0;
                Debug.Log("PAUSE");
                m_PauseQueued = false;
                m_Paused = true;
            }
            else if (m_UnpauseQueued && m_Paused)
            {
                Debug.Log("UNPAUSE");
                m_Paused = false;
                m_UnpauseQueued = false;
                Time.timeScale = m_PreviousTimeScale;
            }
        }

        void MonitorCallback(string address, OscDataHandle handle)
        {
            var value = handle.GetElementAsInt(0);
            if (address.Contains("/pause"))
            {
                if (!m_Paused && value > 0)
                {
                    m_PauseQueued = true;
                    return;
                }

                if (m_Paused && value == 0)
                    m_UnpauseQueued = true;
            }
        }
    }
}