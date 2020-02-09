using OscCore;
using OscJack;

namespace Resolink
{
    public class FloatOscEventHandler : OscEventHandler<FloatUnityEvent, float>
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