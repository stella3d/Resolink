using UnityEngine;

namespace Resolunity
{
    public static class Utils
    {
        public static float ResolumeBpmEventToRealBpm(float eventValue)
        {
            const float min = 20f;
            const float max = 500f;
            return Mathf.Lerp(min, max, eventValue);
        }
    }
}