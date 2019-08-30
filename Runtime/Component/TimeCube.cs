using System.Collections;
using System.Collections.Generic;
using OscJack;
using UnityEngine;

namespace Resolunity
{
    public class TimeCube : MonoBehaviour
    {
        public float BPM;
        
        bool m_PauseQueued;
        bool m_UnpauseQueued;
        bool m_Paused;

        bool m_IsFirstTempoEvent = true;
        float m_InitialTimeScale;
        float m_InitialEventValue;
        float m_PreviousEventValue;
        float m_PreviousTimeScale;
        
        void Awake()
        {
            m_InitialTimeScale = Time.timeScale;
            m_IsFirstTempoEvent = true;
            
            Debug.Log("initial time scale " + m_InitialTimeScale);
        }

        void Update()
        {
            if (m_PauseQueued)
            {
                m_PreviousTimeScale = Time.timeScale;
                Time.timeScale = 0;
                m_PauseQueued = false;
                m_Paused = true;
            }
            else if (m_UnpauseQueued)
            {
                m_Paused = false;
                m_UnpauseQueued = false;
                Time.timeScale = m_PreviousTimeScale;
            }
        }

        public void HandlePauseEvent(bool eventValue)
        {
            // Debug.Log("HandlePauseEvent");
            if (!m_Paused && eventValue)
            {
                m_PauseQueued = true;
                return;
            }

            if (m_Paused && !eventValue)
                m_UnpauseQueued = true;
        }
        
        public void HandleTempoEvent(float eventValue)
        {
            // Debug.Log("HandleTempoEvent");
            if (m_IsFirstTempoEvent)
            {
                BPM = Utils.ResolumeBpmEventToRealBpm(eventValue);
                m_InitialTimeScale = Time.timeScale;
                m_PreviousTimeScale = Time.timeScale;
                m_InitialEventValue = eventValue;
                m_PreviousEventValue = eventValue;
                m_IsFirstTempoEvent = false;
            }
            else
            {
                // what if we got 0.18 and previous was 0.2 ?
                // we would expect to have a portion of 0.9
                var portionOfPrevious = eventValue / m_PreviousEventValue;
                var previousBpm = Utils.ResolumeBpmEventToRealBpm(m_PreviousEventValue);
                BPM = Utils.ResolumeBpmEventToRealBpm(eventValue);
                Debug.Log($"bpm change from {previousBpm} to {BPM}");
                
                Time.timeScale = m_PreviousTimeScale * portionOfPrevious;
                m_PreviousTimeScale = Time.timeScale;
                m_PreviousEventValue = eventValue;
            }

            BPM = Utils.ResolumeBpmEventToRealBpm(eventValue);
        }
    }
}