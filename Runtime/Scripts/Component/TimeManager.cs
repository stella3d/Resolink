using UnityEngine;

namespace Resolink
{
    public class TimeManager : MonoBehaviour
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
            BPM = Utils.ResolumeBpmEventToRealBpm(eventValue);

            if (m_IsFirstTempoEvent)
            {
                m_InitialTimeScale = Time.timeScale;
                m_PreviousTimeScale = Time.timeScale;
                m_InitialEventValue = eventValue;
                m_PreviousEventValue = eventValue;
                m_IsFirstTempoEvent = false;
            }
            else
            {
                var portionOfPrevious = eventValue / m_PreviousEventValue;
                Time.timeScale = m_PreviousTimeScale * portionOfPrevious;
                m_PreviousTimeScale = Time.timeScale;
                m_PreviousEventValue = eventValue;
            }

        }
    }
}