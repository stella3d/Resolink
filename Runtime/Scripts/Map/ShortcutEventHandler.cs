using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Resolink
{

    
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
    public class FloatEventHandler : ShortcutEventHandler<FloatUnityEvent>
    {
        public FloatEventHandler(ResolumeOscShortcut shortcut) : base(shortcut)
        {
            if (shortcut.TypeName == typeof(float).Name)
                Debug.LogWarningFormat("Cannot create float event with data type {0}, path {1}",
                    shortcut.TypeName, shortcut.Output.Path);
            
            m_Event = new FloatUnityEvent();
        }
    }
    
    [Serializable]
    public class IntEventHandler : ShortcutEventHandler<IntUnityEvent>
    {
        public IntEventHandler(ResolumeOscShortcut shortcut) : base(shortcut)
        {
            if (shortcut.TypeName == typeof(int).Name)
                Debug.LogWarningFormat("Cannot create int event with data type {0}, path {1}",
                    shortcut.TypeName, shortcut.Output.Path);
            
            m_Event = new IntUnityEvent();
        }
    }
}