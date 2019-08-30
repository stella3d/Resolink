using OscJack;

namespace Resolunity
{
    public class IntOscEventHandler : OscEventHandler<IntUnityEvent, int> 
    {         
        protected override int GetMessageValue(OscDataHandle dataHandle)
        {
            return dataHandle.GetElementAsInt(0);
        }
    }
}