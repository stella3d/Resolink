using OscCore;

namespace Resolink
{
    public class StringResolumeShortcutHandler : ResolumeShortcutHandler<StringUnityEvent, string>
    {
        protected override string GetMessageValueCore(OscMessageValues values)
        {
            return values.ReadStringElement(0);
        }
    }
}