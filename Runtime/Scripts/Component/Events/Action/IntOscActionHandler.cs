using System;
using OscCore;
using OscJack;

namespace Resolink
{
    [Serializable]
    public class IntOscActionHandler : OscActionHandler<int>
    {
        protected override int GetMessageValue(OscMessageValues dataHandle)
        {
            return dataHandle.ReadIntElement(0);
        }

        public IntOscActionHandler(Action<int> action) : base(action) { }
    }
}