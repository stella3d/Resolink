using OscCore;
using OscJack;

namespace Resolink
{
    public class IntResolumeShortcutHandler : ResolumeShortcutHandler<IntUnityEvent, int> 
    {         
        protected override int GetMessageValue(OscDataHandle dataHandle)
        {
            return dataHandle.GetElementAsInt(0);
        }
        
        protected override int GetMessageValueCore(OscMessageValues values)
        {
            return values.ReadIntElement(0);
        }
    }
}