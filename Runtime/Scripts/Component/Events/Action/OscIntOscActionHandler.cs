using System;
using OscJack;

namespace Resolink
{
    public class OscIntOscActionHandler : OscActionHandler<int>
    {
        protected override int GetMessageValue(OscDataHandle dataHandle)
        {
            return dataHandle.GetElementAsInt(0);
        }

        public OscIntOscActionHandler(Action<int> action) : base(action) { }
    }
}