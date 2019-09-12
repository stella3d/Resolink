using System;
using UnityEngine;
using UnityEngine.Events;

namespace Resolink
{
    // Resolume OSC messages are all one of these 4 types
    [Serializable] public class IntUnityEvent : UnityEvent<int> {}

    [Serializable] public class FloatUnityEvent : UnityEvent<float> { }
    
    [Serializable] public class BoolUnityEvent : UnityEvent<bool> { }
    
    [Serializable] public class StringUnityEvent : UnityEvent<string> { }
    
    
    // these event types are handled by combining several primitive events
    [Serializable] public class Vector2UnityEvent : UnityEvent<Vector2> { }
    [Serializable] public class Vector3UnityEvent : UnityEvent<Vector3> { }
    [Serializable] public class ColorUnityEvent : UnityEvent<Color> { }
}