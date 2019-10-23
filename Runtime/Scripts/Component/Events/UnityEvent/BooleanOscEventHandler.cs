using OscCore;

namespace Resolink
{
    public class BooleanOscEventHandler : OscEventHandler<BoolUnityEvent, bool>
    {
        protected override bool GetMessageValue(OscMessageValues dataHandle)
        {
            return dataHandle.ReadIntElement(0) > 0;
        }
    }
}