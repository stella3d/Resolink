using System;

namespace Resolink
{
    public static class ShortcutExtensions
    {
        const string compLayers = "/composition/layers";
        const string k_Cuepoints = "/cuepoints/";


        public static bool IsTimeEvent(this ResolumeOscShortcut shortcut)
        {
            const string tempoController = "tempocontroller";
            var inputPath = shortcut.Input.Path;
            return inputPath.Contains(tempoController);
        }
        
        public static bool IsCueEvent(this ResolumeOscShortcut shortcut)
        {
            return shortcut.Input.Path.Contains(k_Cuepoints);
        }
        
        public static bool IsClipTransportEvent(this ResolumeOscShortcut shortcut)
        {
            var notCuepoint = !shortcut.Input.Path.Contains(k_Cuepoints);
            var containsTrans = shortcut.Input.Path.Contains("/transport");
            var isOnLayers = shortcut.Input.Path.Contains("/layers");
            return notCuepoint && containsTrans && isOnLayers;
        }
        
        public static bool IsCompositionEvent(this ResolumeOscShortcut shortcut, bool excludeDashboard = true)
        {
            var inputPath = shortcut.Input.Path;
            if (inputPath.Contains("/composition/layers"))
                return false;
            
            var dashboardOk = !excludeDashboard || !inputPath.Contains("dashboard");
            return inputPath.IndexOf("/composition", StringComparison.CurrentCulture) == 0 && dashboardOk;
        }
        
        public static bool IsLayerEvent(this ResolumeOscShortcut shortcut, bool excludeDashboard = true)
        {
            var inputPath = shortcut.Input.Path;
            var dashboardOk = !excludeDashboard || !inputPath.Contains("dashboard");
            return inputPath.IndexOf(compLayers, StringComparison.CurrentCulture) == 0 && dashboardOk;
        }
        
        public static bool IsLayerEffectEvent(this ResolumeOscShortcut shortcut)
        {
            var inputPath = shortcut.Input.Path;
            var dashboardOk = !inputPath.Contains("dashboard");
            var hasEffect = inputPath.Contains("/effect/");
            var hasLayer = inputPath.Contains("/layers/");
            return dashboardOk && hasEffect && hasLayer;
        }
        
        public static bool IsCompositionDashboardEvent(this ResolumeOscShortcut shortcut)
        {
            const string compDashboard = "composition/dashboard";
            var inputPath = shortcut.Input.Path;
            return inputPath.Contains(compDashboard);
        }
        
        public static bool IsCompositionLayerDashboardEvent(this ResolumeOscShortcut shortcut)
        {
            var inputPath = shortcut.Input.Path;
            return inputPath.IndexOf(compLayers, StringComparison.CurrentCulture) == 0 
                   && inputPath.Contains("dashboard");
        }
               
        public static bool IsApplicationUiEvent(this ResolumeOscShortcut shortcut)
        {
            const string applicationUI = "application/ui";
            var inputPath = shortcut.Input.Path;
            return inputPath.Contains(applicationUI);
        }
    }
}