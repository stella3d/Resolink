using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace Resolunity.Tests
{
    public class PathParsingTests
    {
        [TestCase("/composition/layers/*/autopilot", "\\/composition\\/layers\\/[0-9]+\\/autopilot")]
        [TestCase("/composition/layers/*/clips/*/select", "\\/composition\\/layers\\/[0-9]+\\/clips\\/[0-9]+\\/select")]
        [TestCase("/composition/layers/4/autopilot", null)]
        public void RegexForWildcardPath(string path, string expectedPattern)
        {
            var regex = PathUtils.RegexForWildcardPath(path);
            if(regex == null)
                Assert.Null(expectedPattern);
            else
                Assert.AreEqual(expectedPattern, regex.ToString());
        }
    }
}