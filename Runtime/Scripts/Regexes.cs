using System.Text.RegularExpressions;

namespace Resolink
{
    public static class Regexes
    {
        const string k_NumberRegexPattern = "\\/([0-9]+)\\/";
        
        const string k_LayerNumberRegexPattern = "layers\\/([0-9]+)\\/";

        public static Regex WildcardNumber { get; } = new Regex(k_NumberRegexPattern);
        public static Regex LayerNumber { get; } = new Regex(k_LayerNumberRegexPattern);
    }
}