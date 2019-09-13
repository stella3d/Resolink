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
        const string transEffect = "/(transform|effect)/";
        
        public static class Vector3
        {
            
            public static Regex XYZPos { get; } = new Regex(transEffect + "[xyz]pos");
            public static Regex PositionXYZ { get; } = new Regex(transEffect + "position[xyz]");
            public static Regex RotationXYZ { get; } = new Regex(transEffect + "rotation[xyz]");
            public static Regex AnchorXYZ { get; } = new Regex(transEffect + "anchor[xyz]");
            public static Regex TranslateXYZ { get; } = new Regex(transEffect + "(d)?translate[xyz]");
            public static Regex RotateXYZ { get; } = new Regex(transEffect + "(g)?rotate[xyz]");
            public static Regex FragRotationXYZ { get; } = new Regex(transEffect + "/effect/fragrotation[xyz]");

            public static Regex[] All { get; } = 
            {
                XYZPos, PositionXYZ, RotationXYZ, AnchorXYZ, TranslateXYZ, RotateXYZ, FragRotationXYZ
            };
        }
        
        public static class Vector2
        {
            // TODO - add regexes for vec2 components
            public static Regex[] All { get; } = { };
        }

        const string k_NumberRegexPattern = "/([0-9]+)/";
        
        const string k_LayerNumberRegexPattern = "layers/([0-9]+)/";
        
        public static Regex RedColorComponent { get; } = new Regex("/color([0-9]+)?/(r|red)$");
        public static Regex GreenColorComponent { get; } = new Regex("/color([0-9]+)?/(g|green)$");
        public static Regex BlueColorComponent { get; } = new Regex("/color([0-9]+)?/(b|blue)$");
        public static Regex AlphaColorComponent { get; } = new Regex("/color([0-9]+)?/(a|alpha)$");
        
        public static Regex WildcardNumber { get; } = new Regex(k_NumberRegexPattern);
        public static Regex LayerNumber { get; } = new Regex(k_LayerNumberRegexPattern);
    }
}