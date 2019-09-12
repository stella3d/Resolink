using System;
using OscJack;

namespace Resolink
{
    public class StringOscActionEventHandler : OscActionEventHandler<string>
    {
        protected override string GetMessageValue(OscDataHandle dataHandle)
        {
            return dataHandle.GetElementAsString(0);
        }

        public StringOscActionEventHandler(Action<string> action) : base(action) { }
    }
}