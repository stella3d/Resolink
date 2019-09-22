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
        
        public override void SendValue()
        {
            OscRouter.Client.Send(Handlers[0].Shortcut.Input.Path, m_Value.x);
            OscRouter.Client.Send(Handlers[1].Shortcut.Input.Path, m_Value.y);
            OscRouter.Client.Send(Handlers[2].Shortcut.Input.Path, m_Value.z);
        }
        
        public void SetX(float x) { m_Value.x = x; }
        
        public void SetY(float y) { m_Value.y = y; }
        
        public void SetZ(float z) { m_Value.z = z; }
    }
}