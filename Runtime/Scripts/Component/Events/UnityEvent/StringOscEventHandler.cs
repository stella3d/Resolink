using OscJack;

namespace Resolink
{
    public class StringOscEventHandler : OscEventHandler<StringUnityEvent, string>
    {
        public override bool ReadData(OscDataHandle handle)
        {
            var newValue = handle.GetElementAsString(0);
            if (m_Value != newValue)
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