using UnityEngine;

namespace Resolink
{
    public class Vector2OscEventHandler : CompoundOscEventHandler<FloatOscActionHandler, 
        Vector2UnityEvent, Vector2, float>
    {
        public override void Setup()
        {
            if (Handlers == null || Handlers.Length == 0)
            {
                Handlers = new[]
                {
                    new FloatOscActionHandler(SetX),
                    new FloatOscActionHandler(SetY),
                };
            }
            else
            {
                Handlers[0].Event = SetX;
                Handlers[1].Event = SetY;
            }
        }
        
        public void SetHandlers(Vector2ShortcutGroup group)
        {
            Handlers[0].Shortcut = group.X; 
            Handlers[1].Shortcut = group.Y;
        }
        
        public void SetX(float x) { Value.x = x; }
        
        public void SetY(float y) { Value.y = y;  }
    }
}