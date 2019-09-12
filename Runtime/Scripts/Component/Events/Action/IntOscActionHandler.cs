using System;
using OscJack;

namespace Resolink
{
    public class IntOscActionHandler : OscActionHandler<int>
    {
        protected override int GetMessageValue(OscDataHandle dataHandle)
        {
            return dataHandle.GetElementAsInt(0);
        }

        public IntOscActionHandler(Action<int> action) : base(action) { }
    }
}