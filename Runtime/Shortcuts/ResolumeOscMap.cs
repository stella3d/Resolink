using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityResolume
{
    [Serializable]
    public class ResolumeOscMap : ScriptableObject
    {
        public List<ResolumeOscShortcut> Shortcuts = new List<ResolumeOscShortcut>();
    }
}