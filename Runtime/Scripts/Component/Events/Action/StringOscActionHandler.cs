using System;
using OscCore;
using OscJack;

namespace Resolink
{
    [Serializable]
    public class StringOscActionHandler : OscActionHandler<string>
    {
        protected override string GetMessageValue(OscDataHandle dataHandle)
        {
            return dataHandle.GetElementAsString(0);
        }

        protected override string GetMessageValueCore(OscMessageValues values)
        {
            return values.ReadStringElement(0);
        }

        public StringOscActionHandler(Action<string> action) : base(action) { }
    }
}