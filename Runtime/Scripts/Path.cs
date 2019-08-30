using System;
using UnityEngine;

namespace Resolunity
{
    public static class Path
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
    }
}