using System;
using OscCore;

namespace Resolink
{
    [Serializable]
    public class StringOscActionHandler : OscActionHandler<string>
    {
        protected override string GetMessageValueCore(OscMessageValues values)
        {
            return values.ReadStringElement(0);
        }

        public StringOscActionHandler(Action<string> action) : base(action) { }
    }
}