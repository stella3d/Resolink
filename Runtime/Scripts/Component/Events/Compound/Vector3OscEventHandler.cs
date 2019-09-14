using UnityEngine;

namespace Resolink
{
    public class Vector3OscEventHandler : CompoundOscEventHandler<FloatOscActionHandler, 
        Vector3UnityEvent, Vector3, float>
    {
        public override void Setup()
        {
            if (Handlers == null || Handlers.Length == 0)
            {
                Handlers = new[]
                {
                    new FloatOscActionHandler(SetX),
                    new FloatOscActionHandler(SetY),
                    new FloatOscActionHandler(SetZ),
                };
            }
            else
            {
                Handlers[0].Event = SetX;
                Handlers[1].Event = SetY;
                Handlers[2].Event = SetZ;
            }
        }
        
        public void SetHandlers(Vector3ShortcutGroup group)
        {
            Handlers[0].Shortcut = group.X; 
            Handlers[1].Shortcut = group.Y;
            Handlers[2].Shortcut = group.Z;
        }
        
        public void SetX(float x) { Value.x = x; }
        
        public void SetY(float y) { Value.y = y; }
        
        public void SetZ(float z) { Value.z = z; }
    }
}