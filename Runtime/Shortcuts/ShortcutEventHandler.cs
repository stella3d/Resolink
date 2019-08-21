using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace UnityResolume
{
    [Serializable] public class IntEvent        : UnityEvent<int> {}
    [Serializable] public class FloatEvent      : UnityEvent<float> {}
    
    [Serializable]
    public abstract class ShortcutEventHandler<TUnityEvent>
    {
        [SerializeField]
        protected TUnityEvent m_Event;
        
        protected ResolumeOscShortcut m_Shortcut;
        
        public TUnityEvent Event => m_Event;

        protected ShortcutEventHandler(ResolumeOscShortcut shortcut)
        {
            m_Shortcut = shortcut;
        }
    }
    
    [Serializable]
    public class FloatEventHandler : ShortcutEventHandler<FloatEvent>
    {
        public FloatEventHandler(ResolumeOscShortcut shortcut) : base(shortcut)
        {
            if (shortcut.DataType == typeof(float))
                Debug.LogWarningFormat("Cannot create float event with data type {0}, path {1}",
                    shortcut.DataType, shortcut.Output.Path);
            
            m_Event = new FloatEvent();
        }
    }
    
    [Serializable]
    public class IntEventHandler : ShortcutEventHandler<IntEvent>
    {
        public IntEventHandler(ResolumeOscShortcut shortcut) : base(shortcut)
        {
            if (shortcut.DataType == typeof(int))
                Debug.LogWarningFormat("Cannot create int event with data type {0}, path {1}",
                    shortcut.DataType, shortcut.Output.Path);
            
            m_Event = new IntEvent();
        }
    }
}