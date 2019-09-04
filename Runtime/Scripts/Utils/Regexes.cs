using System.Text.RegularExpressions;

namespace Resolink
{
    public class RegexStrings
    {
        public const string ConnectClip = "layers/[0-9]+/connect[a-z]+clip$";
        
        public const string BehaviorGain = "/[a-z]+/behaviour/gain$";
        public const string BehaviorFallback = "/[a-z]+/behaviour/fallback$";
        
        
        public const string BehaviorDuration = "/[a-z]+/behaviour/duration$";

        public const string DashboardLink = "/dashboard/link[1-8]$";

    }

    public static class Regexes
    {
        const string k_NumberRegexPattern = "/([0-9]+)/";
        
        const string k_LayerNumberRegexPattern = "layers/([0-9]+)/";



        public static Regex WildcardNumber { get; } = new Regex(k_NumberRegexPattern);
        public static Regex LayerNumber { get; } = new Regex(k_LayerNumberRegexPattern);
    }
}