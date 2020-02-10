using OscCore;

namespace Resolink
{
    public class FloatResolumeShortcutHandler : ResolumeShortcutHandler<FloatUnityEvent, float>
    {
        protected override float GetMessageValueCore(OscMessageValues values)
        {
            return values.ReadFloatElement(0);
        }
    }
}