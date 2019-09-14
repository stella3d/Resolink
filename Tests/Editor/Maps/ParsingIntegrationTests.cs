using NUnit.Framework;
using UnityEngine;

namespace Resolink.Tests
{
    public class ParsingIntegrationTests : ScriptableObject
    {
        public ExpectedParseResult[] AllTests;

        public void OnEnable()
        {
            AllTests = EditorUtils.LoadAllAssets<ExpectedParseResult>();
        }

        [Test]
        public void SingleColorControl()
        {
            foreach (var test in AllTests)
                test.AssertExpectedResult();
        }
    }
}