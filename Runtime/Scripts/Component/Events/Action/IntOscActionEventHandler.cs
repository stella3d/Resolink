using System;
using OscJack;

namespace Resolink
{
    public class IntOscActionEventHandler : OscActionEventHandler<int>
    {
        protected override int GetMessageValue(OscDataHandle dataHandle)
        {
            return dataHandle.GetElementAsInt(0);
        }

        public IntOscActionEventHandler(Action<int> action) : base(action) { }
    }
}