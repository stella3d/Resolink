using System;
using OscCore;

namespace Resolink
{
    [Serializable]
    public class StringOscActionHandler : OscActionHandler<string>
    {
        protected override string GetMessageValue(OscMessageValues dataHandle)
        {
            return dataHandle.ReadStringElement(0);
        }

        public StringOscActionHandler(Action<string> action) : base(action) { }
    }
}