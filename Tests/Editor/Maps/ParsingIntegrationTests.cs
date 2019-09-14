using System.Collections;
using NUnit.Framework;
using UnityEngine;

namespace Resolink.Tests
{
    public class ParsingIntegrationTests : ScriptableObject
    {
        public static ExpectedParseResult[] AllTests;

        [TestCaseSource(typeof(Data), nameof(Data.All))]
        public void All(ExpectedParseResult testCase)
        { 
            testCase.AssertExpectedResult();
        }


        public static class Data
        {
            public static IEnumerable All
            {
                get
                {
                    AllTests = EditorUtils.LoadAllAssets<ExpectedParseResult>();
                    foreach (var t in AllTests)
                        yield return new TestCaseData(t);
                }
            }
        }
    }
}