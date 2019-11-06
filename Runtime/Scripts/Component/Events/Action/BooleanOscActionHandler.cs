using System;
using OscCore;

namespace Resolink
{
    [Serializable]
    public class BooleanOscActionHandler : OscActionHandler<bool>
    {
        protected override bool GetMessageValue(OscMessageValues dataHandle)
        {
            return dataHandle.ReadIntElement(0) > 0;
        }

        public BooleanOscActionHandler(Action<bool> action) : base(action) { }
    }
}