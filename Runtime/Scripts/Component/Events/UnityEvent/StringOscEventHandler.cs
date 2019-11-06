using OscCore;

namespace Resolink
{
    public class StringOscEventHandler : OscEventHandler<StringUnityEvent, string>
    {
        protected override string GetMessageValue(OscMessageValues dataHandle)
        {
            return dataHandle.ReadStringElement(0);
        }
    }
}