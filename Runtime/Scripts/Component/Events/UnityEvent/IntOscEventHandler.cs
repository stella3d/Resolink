using OscJack;

namespace Resolink
{
    public class IntOscEventHandler : OscEventHandler<IntUnityEvent, int> 
    {         
        protected override int GetMessageValue(OscDataHandle dataHandle)
        {
            return dataHandle.GetElementAsInt(0);
        }
    }
}