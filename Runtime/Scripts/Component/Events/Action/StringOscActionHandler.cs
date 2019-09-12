using System;
using OscJack;

namespace Resolink
{
    public class StringOscActionHandler : OscActionHandler<string>
    {
        protected override string GetMessageValue(OscDataHandle dataHandle)
        {
            return dataHandle.GetElementAsString(0);
        }

        public StringOscActionHandler(Action<string> action) : base(action) { }
    }
}