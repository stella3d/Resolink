using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Resolink
{
    public static class PathUtils
    {
        public static bool TryParseWildcardInt(string inputPath, out int value)
        {
            var match = Regexes.WildcardNumber.Match(inputPath);

            value = default;
            return true;
        }

        public static bool IsWildcardTemplate(string path)
        {
            const string asterisk = "/*/";
            var wildcardIndex = path.IndexOf(asterisk, StringComparison.CurrentCulture);
            return wildcardIndex != -1;
        }
        
        public static bool HasLayerNumber(string path)
        {
            return Regexes.LayerNumber.IsMatch(path);
        }
        
        // this assumed that the regex has matched
        internal static int LayerNumber(string path)
        {
            const string layers = "layers/";
            var layerIndex = path.IndexOf(layers, StringComparison.CurrentCulture);
            var firstSlashIndex = layerIndex + 6;
            var nextSlashIndex = path.IndexOf('/', firstSlashIndex, 3);
            var subStr = path.Substring(firstSlashIndex, nextSlashIndex - firstSlashIndex);
            int.TryParse(subStr, out var value);
            return value;
        }

        public static Regex RegexForWildcardPath(string path)
        {
            if (!IsWildcardTemplate(path))
                return null;
            
            // as an input example,    /composition/layers/*/autopilot
            // we want the regex like  ^\/composition\/layers\/[0-9]+\/autopilot$
            path = "^" + path + "$";
            var regexStr = path.Replace("/", "\\/");
            
            // all of the wildcards in resolume are numbers as far as i've seen
            regexStr = regexStr.Replace("*", "[0-9]+");
            Debug.Log($"{path} -> {regexStr}");
            var regex = new Regex(regexStr);
            return regex;
        }
        
        public static Regex RegexForPath(string fullPath)
        {
            var path = "^" + fullPath + "$";
            var regexStr = path.Replace("/", "\\/");
            
            // all of the wildcards in resolume are numbers as far as i've seen
            regexStr = regexStr.Replace("*", "[0-9]+");
            
            Debug.Log($"{fullPath} -> {regexStr}");
            var regex = new Regex(regexStr);
            return regex;
        }
    }
}