using System.Text.RegularExpressions;

namespace Resolink
{
    public class RegexStrings
    {
        public const string ApplicationClipScroll = "/application/ui/clipsscroll(horizontal|vertical)$";

        public const string ConnectClip = "layers/[0-9]+/connect[a-z]+clip$";
        public const string ConnectSemanticColumn = "^/composition/connect(prev|next|specific)column$";
        
        public const string CuepointsSet = "/cuepoints/setparams/set[1-6]$";
        public const string CuepointsJump = "/cuepoints/jumpparams/jump[1-6]$";
        
        public const string BehaviorGain = "/[a-z]+/behaviour/gain$";
        public const string BehaviorFallback = "/[a-z]+/behaviour/fallback$";

        public const string BypassedSoloClear = "/*/(bypassed|solo|clear)$";
        
        
        public const string BehaviorDuration = "/[a-z]+/behaviour/duration(/(divide|multiply))?$";

        public const string DashboardLink = "/dashboard/link[1-8]$";

        public const string TempoTapPullPush = "/tempocontroller/tempo(tap|pull|push)$";

        public const string TransportPosition = "/transport/position(/(in|out))?$";
    }

    public static class Regexes
    {
        const string k_NumberRegexPattern = "/([0-9]+)/";
        
        const string k_LayerNumberRegexPattern = "layers/([0-9]+)/";



        public static Regex WildcardNumber { get; } = new Regex(k_NumberRegexPattern);
        public static Regex LayerNumber { get; } = new Regex(k_LayerNumberRegexPattern);
    }
}