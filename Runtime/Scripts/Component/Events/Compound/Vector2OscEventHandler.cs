using UnityEngine;

namespace Resolink
{
    public class Vector2OscEventHandler : CompoundOscEventHandler<FloatOscActionHandler, 
        Vector2UnityEvent, Vector2, float>
    {
        public override void Setup()
        {
            if (Handlers == null)
            {
                Handlers = new[]
                {
                    new FloatOscActionHandler(SetX),
                    new FloatOscActionHandler(SetY),
                };
            }
        }
        
        public void SetX(float x) { Value.x = x; }
        
        public void SetY(float y) { Value.y = y;  }
    }
}