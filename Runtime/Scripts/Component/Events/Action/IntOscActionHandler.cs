using System;
using OscCore;

namespace Resolink
{
    [Serializable]
    public class IntOscActionHandler : OscActionHandler<int>
    {
        protected override int GetMessageValueCore(OscMessageValues values)
        {
            return values.ReadIntElement(0);
        }

        public IntOscActionHandler(Action<int> action) : base(action) { }
    }
}