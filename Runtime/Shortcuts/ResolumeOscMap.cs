using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityResolume
{
    [Serializable]
    public class ResolumeOscMap : ScriptableObject
    {
        public ResolumeVersion Version;
        
        public List<ResolumeOscShortcut> Shortcuts = new List<ResolumeOscShortcut>();
        
        public OscMapEvents Events;
    }
}