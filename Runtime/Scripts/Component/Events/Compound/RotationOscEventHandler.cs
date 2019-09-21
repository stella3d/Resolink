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

        public override void SendValue()
        {
            m_EulerAngles = Value.eulerAngles;
            OscRouter.Client.Send(Handlers[0].Shortcut.Input.Path, m_EulerAngles.x);
            OscRouter.Client.Send(Handlers[1].Shortcut.Input.Path, m_EulerAngles.y);
            OscRouter.Client.Send(Handlers[2].Shortcut.Input.Path, m_EulerAngles.z);
        }

        // only convert euler -> quaternion once per frame before invoking the user callback
        protected override void ProcessBeforeInvoke()
        {
            Value = Quaternion.Euler(m_EulerAngles);
        }
        
        const float minAngle = -180f;
        const float maxAngle = 180f;

        // Resolume outputs rotations in Euler angles as a 0-1 float,
        // where 0 is -180 and 1 is 180, so we lerp between those to get actual angles.
        // TODO - not sure if a straight euler translation is accurate -
        // depending on where you position the camera, the rotation can appear differently than it does in resolume
        public void SetEulerX(float x)
        {
            m_EulerAngles.x = minAngle + 360f * x;
        }

        public void SetEulerY(float y)
        {
            m_EulerAngles.y = minAngle + 360f * y;
        }

        public void SetEulerZ(float z)
        {
            m_EulerAngles.z = minAngle + 360f * z;
        }
    }
}