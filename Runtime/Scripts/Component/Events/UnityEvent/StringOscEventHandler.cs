using OscJack;

namespace Resolink
{
    public class StringOscEventHandler : OscEventHandler<StringUnityEvent, string>
    {
        protected override string GetMessageValue(OscDataHandle dataHandle)
        {
            return dataHandle.GetElementAsString(0);
        }
    }
}