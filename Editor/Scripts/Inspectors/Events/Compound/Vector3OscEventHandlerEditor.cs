using UnityEngine;
using UnityEditor;

namespace Resolink
{
    [CustomEditor(typeof(Vector3OscEventHandler))]
    public class Vector3OscEventHandlerEditor : CompoundOscEventHandlerEditor
        <Vector3OscEventHandler, FloatOscActionHandler, Vector3UnityEvent, Vector3, float> { }
}