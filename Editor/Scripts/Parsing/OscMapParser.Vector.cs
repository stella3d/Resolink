using System.Collections.Generic;

namespace Resolink
{
    public partial class OscMapParser
    {
        static ResolumeOscShortcut s_XShortcut;
        static ResolumeOscShortcut s_YShortcut;
        static ResolumeOscShortcut s_ZShortcut;
        
        static readonly List<ResolumeOscShortcut> k_XShortcuts = new List<ResolumeOscShortcut>();
        static readonly List<ResolumeOscShortcut> k_YShortcuts = new List<ResolumeOscShortcut>();
        static readonly List<ResolumeOscShortcut> k_ZShortcuts = new List<ResolumeOscShortcut>();

        static readonly HashSet<string> k_Vector2ShortcutPrefixes = new HashSet<string>();
        static readonly HashSet<string> k_Vector3ShortcutPrefixes = new HashSet<string>();
        
        static readonly List<Vector2ShortcutGroup> k_Vector2Groups = new List<Vector2ShortcutGroup>();
        static readonly List<Vector3ShortcutGroup> k_Vector3Groups = new List<Vector3ShortcutGroup>();

        void FindVector2Groups()
        {
            if (!ResolinkEditorSettings.Instance.GroupVector2s)    
                return;

            ClearXyzShortcuts();
            k_Vector2ShortcutPrefixes.Clear();
            
            foreach (var shortcut in m_Shortcuts)
            {
                foreach (var regex in Regexes.Vector2.All)
                {
                    if (regex.IsMatch(shortcut.Input.Path))
                    {
                        AssignToVectorFieldList(shortcut);
                        break;
                    }
                }
            }

            AddToVector2PrefixSet(k_XShortcuts);
            AddToVector2PrefixSet(k_YShortcuts);

            foreach (var prefix in k_Vector3ShortcutPrefixes)
            {
                if (!AllVector2ComponentsFound(prefix))
                    continue;
                
                var group = new Vector2ShortcutGroup { X = s_XShortcut, Y = s_YShortcut };
                k_Vector2Groups.Add(group);

                m_Shortcuts.Remove(s_XShortcut);
                m_Shortcuts.Remove(s_YShortcut);
            }
        }

        static void AssignToVectorFieldList(ResolumeOscShortcut shortcut)
        {
            var inPath = shortcut.Input.Path;
            if(inPath.EndsWith("x"))
                k_XShortcuts.Add(shortcut);
            else if(inPath.EndsWith("y"))
                k_YShortcuts.Add(shortcut);
            else if(inPath.EndsWith("z"))
                k_ZShortcuts.Add(shortcut);
        }

        void FindVector3Groups()
        {
            if (!ResolinkEditorSettings.Instance.GroupVector3s)    
                return;

            ClearXyzShortcuts();
            k_Vector3ShortcutPrefixes.Clear();

            foreach (var shortcut in m_Shortcuts)
            {
                foreach (var regex in Regexes.Vector3.All)
                {
                    if (regex.IsMatch(shortcut.Input.Path))
                    {
                        AssignToVectorFieldList(shortcut);
                        break;
                    }
                }
            }

            AddToVector3PrefixSet(k_XShortcuts);
            AddToVector3PrefixSet(k_YShortcuts);
            AddToVector3PrefixSet(k_ZShortcuts);

            foreach (var prefix in k_Vector3ShortcutPrefixes)
            {
                if (!AllVector3ComponentsFound(prefix))
                    continue;
                
                var group = new Vector3ShortcutGroup { X = s_XShortcut, Y = s_YShortcut, Z = s_ZShortcut };
                k_Vector3Groups.Add(group);

                m_Shortcuts.Remove(s_XShortcut);
                m_Shortcuts.Remove(s_YShortcut);
                m_Shortcuts.Remove(s_ZShortcut);
            }
        }
        
        static void ClearXyzShortcuts()
        {
            k_XShortcuts.Clear();
            k_YShortcuts.Clear();
            k_ZShortcuts.Clear();
        }
        
        static bool AllVector2ComponentsFound(string prefix)
        {
            var xFound = PrefixFoundInList(prefix, k_XShortcuts, ref s_XShortcut);       
            var yFound = PrefixFoundInList(prefix, k_YShortcuts, ref s_YShortcut);       
            return xFound && yFound;
        }

        static bool AllVector3ComponentsFound(string prefix)
        {
            var xFound = PrefixFoundInList(prefix, k_XShortcuts, ref s_XShortcut);       
            var yFound = PrefixFoundInList(prefix, k_YShortcuts, ref s_YShortcut);       
            var zFound = PrefixFoundInList(prefix, k_YShortcuts, ref s_ZShortcut);       
            return xFound && yFound && zFound;
        }

        static void AddToVector2PrefixSet(List<ResolumeOscShortcut> shortcuts)
        {
            foreach (var shortcut in shortcuts)
                k_Vector2ShortcutPrefixes.Add(PrefixFromShortcut(shortcut));
        }
        
        static void AddToVector3PrefixSet(List<ResolumeOscShortcut> shortcuts)
        {
            foreach (var shortcut in shortcuts)
                k_Vector3ShortcutPrefixes.Add(PrefixFromShortcut(shortcut));
        }

        public static string PrefixFromShortcut(ResolumeOscShortcut shortcut)
        {
            var inPath = shortcut.Input.Path;
            var lastSplit = inPath.LastIndexOf('/');
            return inPath.Substring(0, lastSplit);
        }
    }
}