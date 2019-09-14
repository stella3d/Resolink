using UnityEngine;
using Quaternion = UnityEngine.Quaternion;

namespace Resolink
{
    public class RotationOscEventHandler : CompoundOscEventHandler<FloatOscActionHandler, 
        QuaternionUnityEvent, Quaternion, float>
    {
        protected Vector3 m_EulerAngles = Vector3.zero;
        
        public override void Setup()
        {
            m_DefaultValue = Quaternion.identity;
            Value = m_DefaultValue;
            
            if (Handlers == null || Handlers.Length == 0)
            {
                Handlers = new[]
                {
                    new FloatOscActionHandler(SetEulerX),
                    new FloatOscActionHandler(SetEulerY),
                    new FloatOscActionHandler(SetEulerZ),
                };
            }
            else
            {
                Handlers[0].Event = SetEulerX;
                Handlers[1].Event = SetEulerY;
                Handlers[2].Event = SetEulerZ;
            }
        }
        
        public void SetHandlers(Vector3ShortcutGroup group)
        {
            Handlers[0].Shortcut = group.X; 
            Handlers[1].Shortcut = group.Y;
            Handlers[2].Shortcut = group.Z;
        }

        const float min = -180f;
        const float max = 180f;
        
        public void SetEulerX(float x)
        {
            m_EulerAngles.x = -180f + 360f * Mathf.Clamp01(x);    // lerp
            Value = Quaternion.Euler(m_EulerAngles);
        }

        public void SetEulerY(float y)
        {
            //m_EulerAngles.y = -180f + 360f * Mathf.Clamp01(y);
            m_EulerAngles.y = 180f - 360f * Mathf.Clamp01(y);
            Value = Quaternion.Euler(m_EulerAngles);
        }

        public void SetEulerZ(float z)
        {
            m_EulerAngles.z = 180f - 360f * Mathf.Clamp01(z);
            //m_EulerAngles.z = -180f + 360f * Mathf.Clamp01(z);
            Value = Quaternion.Euler(m_EulerAngles);
        }
    }
}