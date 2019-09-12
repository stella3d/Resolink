using System;
using OscJack;

namespace Resolink
{
    public class BoolOscActionEventHandler : OscActionEventHandler<bool>
    {
        protected override bool GetMessageValue(OscDataHandle dataHandle)
        {
            return dataHandle.GetElementAsInt(0) > 0;
        }

        public BoolOscActionEventHandler(Action<bool> action) : base(action) { }
    }
}