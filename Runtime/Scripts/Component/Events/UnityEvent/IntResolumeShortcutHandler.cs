using OscCore;


namespace Resolink
{
    public class IntResolumeShortcutHandler : ResolumeShortcutHandler<IntUnityEvent, int> 
    {         
        protected override int GetMessageValueCore(OscMessageValues values)
        {
            return values.ReadIntElement(0);
        }
    }
}