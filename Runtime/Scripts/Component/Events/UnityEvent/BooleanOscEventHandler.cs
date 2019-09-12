using OscJack;

namespace Resolink
{
    public class BooleanOscEventHandler : OscEventHandler<BoolUnityEvent, bool>
    {
        protected override bool GetMessageValue(OscDataHandle dataHandle)
        {
            return dataHandle.GetElementAsInt(0) > 0;
        }
    }
}