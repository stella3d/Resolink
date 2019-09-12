using UnityEngine;
using UnityEditor;

namespace Resolink
{
    [CustomEditor(typeof(Vector2OscEventHandler))]
    public class Vector2OscEventHandlerEditor : CompoundOscEventHandlerEditor
        <Vector2OscEventHandler, FloatOscActionHandler, Vector2UnityEvent, Vector2, float> { }
}