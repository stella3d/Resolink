using OscCore;
using OscJack;

namespace Resolink
{
    public class StringOscEventHandler : OscEventHandler<StringUnityEvent, string>
    {
        protected override string GetMessageValue(OscDataHandle dataHandle)
        {
            return dataHandle.GetElementAsString(0);
        }
        
        protected override string GetMessageValueCore(OscMessageValues values)
        {
            return values.ReadStringElement(0);
        }
    }
}