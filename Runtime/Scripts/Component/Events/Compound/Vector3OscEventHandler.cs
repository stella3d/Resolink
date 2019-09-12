using UnityEngine;

namespace Resolink
{
    public class Vector3OscEventHandler : CompoundOscEventHandler<FloatOscActionHandler, 
        Vector3UnityEvent, Vector3, float>
    {
        public override void Setup()
        {
            if (Handlers == null)
            {
                Handlers = new[]
                {
                    new FloatOscActionHandler(SetX),
                    new FloatOscActionHandler(SetY),
                    new FloatOscActionHandler(SetZ),
                };
            }
        }
        
        public void SetX(float x) { Value.x = x; }
        
        public void SetY(float y) { Value.y = y;  }
        
        public void SetZ(float z) { Value.z = z; }
    }
}