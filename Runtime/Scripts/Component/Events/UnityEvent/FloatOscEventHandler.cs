using OscCore;

namespace Resolink
{
    public class FloatOscEventHandler : OscEventHandler<FloatUnityEvent, float>
    {
        protected override float GetMessageValue(OscMessageValues dataHandle)
        {
            return dataHandle.ReadFloatElement(0);
        }
    }
}