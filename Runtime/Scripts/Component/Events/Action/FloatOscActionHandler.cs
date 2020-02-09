using System;
using OscCore;
using OscJack;

namespace Resolink
{
    [Serializable]
    public class FloatOscActionHandler : OscActionHandler<float>
    {
        protected override float GetMessageValue(OscDataHandle dataHandle)
        {
            return dataHandle.GetElementAsFloat(0);
        }
        
        protected override float GetMessageValueCore(OscMessageValues values)
        {
            return values.ReadFloatElement(0);
        }

        public FloatOscActionHandler(Action<float> action) : base(action) { }
    }
}