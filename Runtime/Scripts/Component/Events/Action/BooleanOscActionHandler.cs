using System;
using OscCore;
using OscJack;

namespace Resolink
{
    [Serializable]
    // TODO - differentiate between int-as-bool and bool tag handler ?
    public class BooleanOscActionHandler : OscActionHandler<bool>
    {
        protected override bool GetMessageValue(OscMessageValues dataHandle)
        {
            return dataHandle.ReadIntElement(0) > 0;
        }

        public BooleanOscActionHandler(Action<bool> action) : base(action) { }
    }
}