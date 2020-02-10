using System;
using OscCore;

namespace Resolink
{
    [Serializable]
    public class FloatOscActionHandler : OscActionHandler<float>
    {
        protected override float GetMessageValueCore(OscMessageValues values)
        {
            return values.ReadFloatElement(0);
        }

        public FloatOscActionHandler(Action<float> action) : base(action) { }
    }
}