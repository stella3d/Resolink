using System;
using System.Collections.Generic;
using UnityEngine;

namespace Resolunity
{
    [Serializable]
    public class ResolumeOscMap : ScriptableObject
    {
        public ResolumeVersion Version;
        
        public List<ResolumeOscShortcut> Shortcuts = new List<ResolumeOscShortcut>();
    }
}