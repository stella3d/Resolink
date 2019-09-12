using System;
using OscJack;

namespace Resolink
{
    public class FloatOscActionHandler : OscActionHandler<float>
    {
        protected override float GetMessageValue(OscDataHandle dataHandle)
        {
            return dataHandle.GetElementAsFloat(0);
        }

        public FloatOscActionHandler(Action<float> action) : base(action) { }
    }
}