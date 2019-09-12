using System;
using OscJack;

namespace Resolink
{
    public class OscBoolOscActionHandler : OscActionHandler<bool>
    {
        protected override bool GetMessageValue(OscDataHandle dataHandle)
        {
            return dataHandle.GetElementAsInt(0) > 0;
        }

        public OscBoolOscActionHandler(Action<bool> action) : base(action) { }
    }
}