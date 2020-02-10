using OscCore;


namespace Resolink
{
    public class BooleanResolumeShortcutHandler : ResolumeShortcutHandler<BoolUnityEvent, bool>
    {
        protected override bool GetMessageValueCore(OscMessageValues values)
        {
            return values.ReadIntElement(0) > 0;
        }
    }
}