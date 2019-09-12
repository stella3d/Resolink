using System;
using System.Collections.Generic;
using UnityEngine;

namespace Resolink
{
    /// <summary>
    /// Stores data about an OSC event configuration from Resolume
    /// </summary>
    [Serializable]
    public class ResolumeOscMap : ScriptableObject
    {
        /// <summary>The version of Resolume that created the source file </summary>
        public ResolumeVersion Version;
        
        /// <summary>All active, ungrouped Resolume shortcuts in the map</summary>
        public List<ResolumeOscShortcut> Shortcuts = new List<ResolumeOscShortcut>();

        /// <summary>All active shortcuts that make up r/g/b/a components of a color, grouped together</summary>
        public List<ColorShortcutGroup> ColorGroups = new List<ColorShortcutGroup>();
        
        /// <summary>All active shortcuts that make up x/y components of a Vector2, grouped together</summary>
        public List<Vector2ShortcutGroup> Vector2Groups = new List<Vector2ShortcutGroup>();
        
        /// <summary>All active shortcuts that make up x/y/z components of a Vector3, grouped together</summary>
        public List<Vector3ShortcutGroup> Vector3Groups = new List<Vector3ShortcutGroup>();
    }
}