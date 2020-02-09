using System;
using OscCore;
using OscJack;

namespace Resolink
{
    [Serializable]
    public class BooleanOscActionHandler : OscActionHandler<bool>
    {
        protected override bool GetMessageValue(OscDataHandle dataHandle)
        {
            return dataHandle.GetElementAsInt(0) > 0;
        }
        
        protected override bool GetMessageValueCore(OscMessageValues values)
        {
            return values.ReadIntElement(0) > 0;
        }

        public BooleanOscActionHandler(Action<bool> action) : base(action) { }
    }
}