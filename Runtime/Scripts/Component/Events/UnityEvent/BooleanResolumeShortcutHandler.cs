using OscCore;
using OscJack;

namespace Resolink
{
    public class BooleanResolumeShortcutHandler : ResolumeShortcutHandler<BoolUnityEvent, bool>
    {
        protected override bool GetMessageValue(OscDataHandle dataHandle)
        {
            return dataHandle.GetElementAsInt(0) > 0;
        }
        
        protected override bool GetMessageValueCore(OscMessageValues values)
        {
            return values.ReadIntElement(0) > 0;
        }
    }
}