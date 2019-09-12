using System;
using OscJack;

namespace Resolink
{
    public class OscFloatOscActionHandler : OscActionHandler<float>
    {
        protected override float GetMessageValue(OscDataHandle dataHandle)
        {
            return dataHandle.GetElementAsFloat(0);
        }

        public OscFloatOscActionHandler(Action<float> action) : base(action) { }
    }
}