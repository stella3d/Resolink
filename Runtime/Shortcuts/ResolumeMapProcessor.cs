using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityResolume
{
    public static class ResolumeMapMethods
    {
        static readonly Dictionary<string, List<SubTarget>> k_TargetGroup = new Dictionary<string, List<SubTarget>>();
        static readonly List<ResolumeOscShortcut> k_NewShortcuts = new List<ResolumeOscShortcut>();
        static readonly HashSet<string> k_ProcessedOutputs = new HashSet<string>();
        
        public static void GroupSubTargets(this ResolumeOscMap map)
        {
            k_ProcessedOutputs.Clear();
            k_TargetGroup.Clear();
            foreach (var shortcut in map.Shortcuts)
            {
                var outPath = shortcut.Output.Path;
                if (shortcut.SubTargets == null)
                {
                    k_TargetGroup.Add(outPath, null);
                    continue;
                }

                if (k_TargetGroup.TryGetValue(outPath, out var targetList))
                    targetList.Add(shortcut.SubTargets[0]);
                else
                    k_TargetGroup.Add(outPath, new List<SubTarget> {shortcut.SubTargets[0]});
            }

            foreach (var kvp in k_TargetGroup)
            {
                var targets = kvp.Value;
                if(targets == null || targets.Count == 1)
                    continue;
                
                targets.Sort();
                
                Debug.LogFormat("path {0} - {1} sub-targets", kvp.Key, kvp.Value?.Count);
            }

            foreach (var shortcut in map.Shortcuts)
            {
                var outPath = shortcut.Output.Path;
                if (k_ProcessedOutputs.Contains(outPath))
                    continue;

                if (k_TargetGroup.TryGetValue(outPath, out var targetList))
                {
                    shortcut.SubTargets = targetList?.ToArray();
                    k_NewShortcuts.Add(shortcut);
                }

                k_ProcessedOutputs.Add(outPath);
            }
            
            Debug.LogFormat("{0} shortcuts after grouping sub-targets", k_NewShortcuts.Count);
            map.Shortcuts = k_NewShortcuts;
        }
    }
}