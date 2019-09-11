using UnityEngine;

namespace Resolink
{
    public static class Utils
    {
        /// <summary>
        /// Convert from the value that resolume sends for a tempo event into BPM
        /// </summary>
        /// <param name="value">The osc event value</param>
        /// <returns>The current BPM</returns>
        public static float ResolumeBpmEventToRealBpm(float value)
        {
            const float min = 20f;
            const float max = 500f;
            return min + (max - min) * Mathf.Clamp01(value);
        }
    }
}