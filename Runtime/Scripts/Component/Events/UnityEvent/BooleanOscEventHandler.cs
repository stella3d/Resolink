using OscJack;

namespace Resolink
{
    public class BooleanOscEventHandler : OscEventHandler<BoolUnityEvent, bool>
    {
        protected override bool GetMessageValue(OscDataHandle dataHandle)
        {
            return dataHandle.GetElementAsInt(0) > 0;
        }
        
        public override void SendValue()
        {
            OscRouter.Client.Send(Shortcut.Input.Path, Value ? 1 : 0);
        }
    }
}