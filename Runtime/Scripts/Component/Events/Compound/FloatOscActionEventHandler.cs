using System;
using OscJack;

namespace Resolink
{
    public class FloatOscActionEventHandler : OscActionEventHandler<float>
    {
        protected override float GetMessageValue(OscDataHandle dataHandle)
        {
            return dataHandle.GetElementAsFloat(0);
        }

        public FloatOscActionEventHandler(Action<float> action) : base(action) { }
    }
}