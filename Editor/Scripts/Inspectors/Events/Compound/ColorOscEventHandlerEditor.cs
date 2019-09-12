using UnityEngine;

namespace Resolink
{
    [UnityEditor.CustomEditor(typeof(ColorOscEventHandler))]
    public class ColorOscEventHandlerEditor : CompoundOscEventHandlerEditor
        <ColorOscEventHandler, FloatOscActionHandler, ColorUnityEvent, Color, float> { }
}