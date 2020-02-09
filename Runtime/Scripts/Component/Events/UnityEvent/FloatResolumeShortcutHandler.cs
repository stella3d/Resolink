using OscCore;
using OscJack;

namespace Resolink
{
    public class FloatResolumeShortcutHandler : ResolumeShortcutHandler<FloatUnityEvent, float>
    {
        protected override float GetMessageValue(OscDataHandle dataHandle)
        {
            return dataHandle.GetElementAsFloat(0);
        }
        
        protected override float GetMessageValueCore(OscMessageValues values)
        {
            return values.ReadFloatElement(0);
        }
    }
}