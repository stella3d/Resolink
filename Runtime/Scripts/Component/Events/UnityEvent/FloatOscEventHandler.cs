using OscJack;
using UnityEngine;

namespace Resolink
{
    [ExecuteAlways]
    public class FloatOscEventHandler : OscEventHandler<FloatUnityEvent, float>
    {
        const float k_Epsilon = 0.00001f;
        
        public override bool ReadData(OscDataHandle handle)
        {
            var newValue = handle.GetElementAsFloat(0);
            var delta = m_Value - newValue;
            delta = delta > 0 ? delta : -delta; 
            if (delta > k_Epsilon)
            {
                m_Value = newValue;
                return true;
            }

            return false;
        }

        public override void SendValue()
        {
            OscRouter.Client.Send(Shortcut.Input.Path, Value);
        }
    }
}