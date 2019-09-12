using UnityEngine;

namespace Resolink
{
    public class ColorOscEventHandler : CompoundOscEventHandler<FloatOscActionEventHandler, 
        ColorUnityEvent, Color, float>
    {
        public override void Setup()
        {
            if (Handlers == null)
            {
                Handlers = new[]
                {
                    new FloatOscActionEventHandler(SetRed),
                    new FloatOscActionEventHandler(SetGreen),
                    new FloatOscActionEventHandler(SetBlue),
                    new FloatOscActionEventHandler(SetAlpha),
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