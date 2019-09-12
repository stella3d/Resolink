using System.Collections.Generic;

namespace Resolink
{
    public partial class OscMapParser
    {
        static ResolumeOscShortcut s_CurrentRedShortcut;
        static ResolumeOscShortcut s_CurrentGreenShortcut;
        static ResolumeOscShortcut s_CurrentBlueShortcut;
        static ResolumeOscShortcut s_CurrentAlphaShortcut;
        
        static readonly List<ResolumeOscShortcut> k_RedColorShortcuts = new List<ResolumeOscShortcut>();
        static readonly List<ResolumeOscShortcut> k_GreenColorShortcuts = new List<ResolumeOscShortcut>();
        static readonly List<ResolumeOscShortcut> k_BlueColorShortcuts = new List<ResolumeOscShortcut>();
        static readonly List<ResolumeOscShortcut> k_AlphaColorShortcuts = new List<ResolumeOscShortcut>();

        static readonly HashSet<string> k_ColorShortcutPrefixes = new HashSet<string>();
        static readonly List<ColorShortcutGroup> k_ColorGroups = new List<ColorShortcutGroup>();
        
        void FindColorGroups()
        {
            if (!ResolinkEditorSettings.Instance.GroupColors)    
                return;
            
            k_ColorShortcutPrefixes.Clear();
            k_RedColorShortcuts.Clear();
            k_GreenColorShortcuts.Clear();
            k_BlueColorShortcuts.Clear();
            k_AlphaColorShortcuts.Clear();
            
            foreach (var shortcut in m_Shortcuts)
            {
                var inPath = shortcut.Input.Path;
                if(Regexes.RedColorComponent.IsMatch(inPath))
                    k_RedColorShortcuts.Add(shortcut);
                else if(Regexes.GreenColorComponent.IsMatch(inPath))
                    k_GreenColorShortcuts.Add(shortcut);
                else if(Regexes.BlueColorComponent.IsMatch(inPath))
                    k_BlueColorShortcuts.Add(shortcut);
                else if(Regexes.AlphaColorComponent.IsMatch(inPath))
                    k_AlphaColorShortcuts.Add(shortcut);
            }

            AddToColorPrefixSet(k_RedColorShortcuts);
            AddToColorPrefixSet(k_GreenColorShortcuts);
            AddToColorPrefixSet(k_BlueColorShortcuts);
            AddToColorPrefixSet(k_AlphaColorShortcuts);

            foreach (var prefix in k_ColorShortcutPrefixes)
            {
                if (AllColorComponentsFound(prefix))
                {
                    var group = new ColorShortcutGroup()
                    {
                        Red = s_CurrentRedShortcut,
                        Green = s_CurrentGreenShortcut,
                        Blue = s_CurrentBlueShortcut,
                        Alpha = s_CurrentAlphaShortcut
                    };
                    
                    k_ColorGroups.Add(group);

                    m_Shortcuts.Remove(s_CurrentRedShortcut);
                    m_Shortcuts.Remove(s_CurrentGreenShortcut);
                    m_Shortcuts.Remove(s_CurrentBlueShortcut);
                    m_Shortcuts.Remove(s_CurrentAlphaShortcut);
                }
            }
        }

        static bool AllColorComponentsFound(string prefix)
        {
            var rFound = PrefixFoundInList(prefix, k_RedColorShortcuts, ref s_CurrentRedShortcut);       
            var gFound = PrefixFoundInList(prefix, k_GreenColorShortcuts, ref s_CurrentGreenShortcut);       
            var bFound = PrefixFoundInList(prefix, k_BlueColorShortcuts, ref s_CurrentBlueShortcut);       
            var aFound = PrefixFoundInList(prefix, k_AlphaColorShortcuts, ref s_CurrentAlphaShortcut);       
            return rFound && gFound && bFound && aFound;
        }

        static bool PrefixFoundInList(string prefix, List<ResolumeOscShortcut> shortcuts, ref ResolumeOscShortcut colorRef)
        {
            foreach (var shortcut in shortcuts)
            {
                if (shortcut.Input.Path.StartsWith(prefix))
                {
                    colorRef = shortcut;
                    return true;
                }
            }

            return false;
        }

        static void AddToColorPrefixSet(List<ResolumeOscShortcut> shortcuts)
        {
            foreach (var shortcut in shortcuts)
                k_ColorShortcutPrefixes.Add(PrefixFromShortcut(shortcut));
        }
    }
}