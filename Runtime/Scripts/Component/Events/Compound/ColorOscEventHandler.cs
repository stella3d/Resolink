using UnityEngine;

namespace Resolink
{
    public class ColorOscEventHandler : CompoundOscEventHandler<FloatOscActionHandler, 
        ColorUnityEvent, Color, float>
    {
        public override void Setup()
        {
            if (Handlers == null || Handlers.Length == 0)
            {
                Handlers = new[]
                {
                    new FloatOscActionHandler(SetRed),
                    new FloatOscActionHandler(SetGreen),
                    new FloatOscActionHandler(SetBlue),
                    new FloatOscActionHandler(SetAlpha),
                };
            }
            else
            {
                Handlers[0].Event = SetRed;
                Handlers[1].Event = SetGreen;
                Handlers[2].Event = SetBlue;
                Handlers[3].Event = SetAlpha;
            }

            // default to full alpha if unset, as resolume doesn't always send alpha while picking colors
            if (Mathf.Approximately(DefaultValue.a, 0f))
            {
                m_DefaultValue.a = 1f;
                Value.a = 1f;
            }
        }

        public void SetHandlers(ColorShortcutGroup group)
        {
            Handlers[0].Shortcut = group.Red; 
            Handlers[1].Shortcut = group.Green;
            Handlers[2].Shortcut = group.Blue;
            Handlers[3].Shortcut = group.Alpha;
        }
        
        public override void SendValue()
        {
            OscRouter.Client.Send(Handlers[0].Shortcut.Input.Path, Value.r);
            OscRouter.Client.Send(Handlers[1].Shortcut.Input.Path, Value.g);
            OscRouter.Client.Send(Handlers[2].Shortcut.Input.Path, Value.b);
            OscRouter.Client.Send(Handlers[3].Shortcut.Input.Path, Value.a);
        }

        public void SetRed(float r) { Value.r = r; }
        
        public void SetGreen(float g) { Value.g = g; }
        
        public void SetBlue(float b) { Value.b = b; }
        
        public void SetAlpha(float a) { Value.a = a; }
    }
}