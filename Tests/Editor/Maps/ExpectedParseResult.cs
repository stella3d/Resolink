using System.IO;
using NUnit.Framework;
using UnityEngine;

namespace Resolink.Tests
{
    [CreateAssetMenu]
    public class ExpectedParseResult : ScriptableObject
    {
        public string XmlPath;

        public ResolumeOscMap ExpectedResult;

        public ResolumeOscMap Parse()
        {
            var fullPath = Application.dataPath + XmlPath;
            if (!File.Exists(fullPath))
            {
                Debug.LogWarning($"{fullPath} does not seem to exist!");
                return null;
            }

            return OscMapParser.Parse(fullPath, false);
        }

        public void AssertExpectedResult()
        {
            var parsed = Parse();
            Assert.NotNull(parsed);
            parsed.AssertDeepEqual(ExpectedResult);
        }
    }
}
