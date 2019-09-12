using UnityEngine;

namespace Resolink
{
    [UnityEditor.CustomEditor(typeof(ColorOscEventHandler))]
    public class ColorOscEventHandlerEditor : CompoundOscEventHandlerEditor
        <ColorOscEventHandler, OscFloatOscActionHandler, ColorUnityEvent, Color, float> { }
}