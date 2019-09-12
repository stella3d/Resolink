using UnityEngine;

namespace Resolink
{
    public class ColorOscEventHandler : CompoundOscEventHandler<OscFloatOscActionHandler, 
        ColorUnityEvent, Color, float>
    {
        public override void Setup()
        {
            if (Handlers == null)
            {
                Handlers = new[]
                {
                    new OscFloatOscActionHandler(SetRed),
                    new OscFloatOscActionHandler(SetGreen),
                    new OscFloatOscActionHandler(SetBlue),
                    new OscFloatOscActionHandler(SetAlpha),
                };
            }
        }
        
        public void SetRed(float r)
        {
            Value.r = r;
        }
        
        public void SetGreen(float g)
        {
            Value.g = g;
        }
        
        public void SetBlue(float b)
        {
            Value.b = b;
        }
        
        public void SetAlpha(float a)
        {
            Value.a = a;
        }
    }
}