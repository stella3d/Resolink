using OscJack;

namespace Resolink
{
    public class BooleanOscEventHandler : OscEventHandler<BoolUnityEvent, bool>
    {
        public override bool ReadData(OscDataHandle handle)
        {
            var newValue = handle.GetElementAsInt(0) > 0;
            if (m_Value != newValue)
            {
                m_Value = newValue;
                return true;
            }

            return false;
        }
        
        public override void SendValue()
        {
            OscRouter.Client.Send(Shortcut.Input.Path, Value ? 1 : 0);
        }
    }
}