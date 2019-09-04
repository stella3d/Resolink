using System.Collections.Generic;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace Resolink.Tests
{
    public class RegexTests
    {
        [TestCase("/composition/layers/4/connectprevclip")]
        [TestCase("/composition/layers/3/connectnextclip")]
        [TestCase("/composition/layers/2/connectspecificclip")]
        public void ConnectClip(string input, bool match = true)
        {
            AssertRegexMatch(RegexStrings.ConnectClip, input, match);
        }
        
        [TestCase("/composition/layers/1/video/effects/transform/scalew/behaviour/gain")]
        [TestCase("/composition/layers/2/video/effects/transform/positiony/behaviour/gain")]
        [TestCase("/composition/layers/3/video/effects/delayrgb/opacity/behaviour/gain")]
        [TestCase("/composition/layers/4/video/opacity/behaviour/gain")]
        [TestCase("/composition/layers/4/video/opacity/behaviour/gain/something/after", false)]
        public void BehaviorGain(string input, bool match = true)
        {
            AssertRegexMatch(RegexStrings.BehaviorGain, input, match);
        }
        
        [TestCase("/composition/layers/4/video/effects/transform/anchorx/behaviour/fallback")]
        [TestCase("/composition/layers/3/video/effects/transform/positiony/behaviour/fallback")]
        [TestCase("/composition/layers/2/video/effects/transform/scaleh/behaviour/fallback")]
        [TestCase("/composition/layers/1/video/opacity/behaviour/fallback")]
        [TestCase("/composition/layers/1/video/opacity/behaviour/fallback/something/after", false)]
        public void BehaviorFallback(string input, bool match = true)
        {
            AssertRegexMatch(RegexStrings.BehaviorFallback, input, match);
        }
        
        
        [TestCase("/composition/dashboard/link1")]
        [TestCase("/composition/dashboard/link8")]
        [TestCase("/layers/1/dashboard/link4")]
        [TestCase("/layers/1/dashboard/link", false)]
        [TestCase("/composition/dashboard/link9", false)]
        public void DashboardLink(string input, bool match = true)
        {
            AssertRegexMatch(RegexStrings.DashboardLink, input, match);
        }



        void AssertRegexMatch(Regex regex, string input, bool match = true)
        {
            if(match)
                Assert.True(regex.IsMatch(input), $"Expected {regex} to match {input}, but it did not");
            else
                Assert.False(regex.IsMatch(input), $"Expected {regex} to not match {input}, but it did");
        }
        
        void AssertRegexMatch(string pattern, string input, bool match = true)
        {
            var regex = new Regex(pattern);
            if(match)
                Assert.True(regex.IsMatch(input), $"Expected {regex} to match {input}, but it did not");
            else
                Assert.False(regex.IsMatch(input), $"Expected {regex} to not match {input}, but it did");
        }
    }
}