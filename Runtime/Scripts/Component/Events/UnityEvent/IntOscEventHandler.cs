using OscCore;
using OscJack;

namespace Resolink
{
    public class IntOscEventHandler : OscEventHandler<IntUnityEvent, int> 
    {         
        protected override int GetMessageValue(OscMessageValues dataHandle)
        {
            return dataHandle.ReadIntElement(0);
        }
    }
}