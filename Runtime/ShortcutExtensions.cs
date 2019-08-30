using System;

namespace Resolunity
{
    public static class ShortcutExtensions
    {
        const string compLayers = "/composition/layers";

        public static bool IsTimeEvent(this ResolumeOscShortcut shortcut)
        {
            const string tempoController = "tempocontroller";
            var inputPath = shortcut.Input.Path;
            return inputPath.Contains(tempoController);
        }
        
        public static bool IsCompositionEvent(this ResolumeOscShortcut shortcut, bool excludeDashboard = true)
        {
            var inputPath = shortcut.Input.Path;
            var dashboardOk = !excludeDashboard || !inputPath.Contains("dashboard");
            return inputPath.IndexOf("/composition", StringComparison.CurrentCulture) == 0 && dashboardOk;
        }
        
        public static bool IsLayerEvent(this ResolumeOscShortcut shortcut, bool excludeDashboard = true)
        {
            var inputPath = shortcut.Input.Path;
            var dashboardOk = !excludeDashboard || !inputPath.Contains("dashboard");
            return inputPath.IndexOf(compLayers, StringComparison.CurrentCulture) == 0 && dashboardOk;
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