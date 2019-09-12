using System;
using OscJack;

namespace Resolink
{
    public class BooleanOscActionHandler : OscActionHandler<bool>
    {
        protected override bool GetMessageValue(OscDataHandle dataHandle)
        {
            return dataHandle.GetElementAsInt(0) > 0;
        }

        public BooleanOscActionHandler(Action<bool> action) : base(action) { }
    }
}