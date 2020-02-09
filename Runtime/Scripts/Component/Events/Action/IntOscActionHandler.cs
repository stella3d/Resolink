using System;
using OscCore;
using OscJack;

namespace Resolink
{
    [Serializable]
    public class IntOscActionHandler : OscActionHandler<int>
    {
        protected override int GetMessageValue(OscDataHandle dataHandle)
        {
            return dataHandle.GetElementAsInt(0);
        }
        
        protected override int GetMessageValueCore(OscMessageValues values)
        {
            return values.ReadIntElement(0);
        }

        public IntOscActionHandler(Action<int> action) : base(action) { }
    }
}