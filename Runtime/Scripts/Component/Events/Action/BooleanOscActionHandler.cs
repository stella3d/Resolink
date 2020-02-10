using System;
using OscCore;

namespace Resolink
{
    [Serializable]
    public class BooleanOscActionHandler : OscActionHandler<bool>
    {
        protected override bool GetMessageValueCore(OscMessageValues values)
        {
            return values.ReadIntElement(0) > 0;
        }

        public BooleanOscActionHandler(Action<bool> action) : base(action) { }
    }
}