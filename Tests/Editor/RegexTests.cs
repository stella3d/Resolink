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
        
        [TestCase("/composition/connectprevcolumn")]
        [TestCase("/composition/connectnextcolumn")]
        [TestCase("/composition/connectspecificcolumn")]
        public void ConnectSemanticColumn(string input, bool match = true)
        {
            AssertRegexMatch(RegexStrings.ConnectSemanticColumn, input, match);
        }
        
        
        
        [TestCase("/composition/layers/6/clips/1/transport/cuepoints/setparams/set1")]
        [TestCase("/composition/layers/5/clips/2/transport/cuepoints/setparams/set2")]
        [TestCase("/composition/layers/4/clips/3/transport/cuepoints/setparams/set3")]
        [TestCase("/composition/layers/3/clips/4/transport/cuepoints/setparams/set4")]
        [TestCase("/composition/layers/2/clips/5/transport/cuepoints/setparams/set5")]
        [TestCase("/composition/layers/1/clips/6/transport/cuepoints/setparams/set6")]
        [TestCase("/composition/layers/1/clips/6/transport/cuepoints/setparams/set", false)]
        [TestCase("/composition/layers/1/clips/6/transport/cuepoints/setparams/set7", false)]
        public void CuepointsSet(string input, bool match = true)
        {
            AssertRegexMatch(RegexStrings.CuepointsSet, input, match);
        }
        
        [TestCase("/composition/layers/6/clips/1/transport/cuepoints/jumpparams/jump1")]
        [TestCase("/composition/layers/5/clips/2/transport/cuepoints/jumpparams/jump2")]
        [TestCase("/composition/layers/4/clips/3/transport/cuepoints/jumpparams/jump3")]
        [TestCase("/composition/layers/3/clips/4/transport/cuepoints/jumpparams/jump4")]
        [TestCase("/composition/layers/2/clips/5/transport/cuepoints/jumpparams/jump5")]
        [TestCase("/composition/layers/1/clips/6/transport/cuepoints/jumpparams/jump6")]
        [TestCase("/composition/layers/1/clips/6/transport/cuepoints/jumpparams/jump", false)]
        [TestCase("/composition/layers/1/clips/6/transport/cuepoints/jumpparams/jump7", false)]
        public void CuepointJump(string input, bool match = true)
        {
            AssertRegexMatch(RegexStrings.CuepointsJump, input, match);
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
        
        // we group these together because they fall in the same category and have the same type + very similar paths
        [TestCase("/composition/layers/1/bypassed")]
        [TestCase("/composition/layers/2/solo")]
        [TestCase("/composition/layers/3/clear")]
        [TestCase("/composition/bypassed")]
        [TestCase("/composition/someeffectprobably/bypassed")]
        [TestCase("/compositionbypassed", false)]
        public void BypassedSoloClear(string input, bool match = true)
        {
            AssertRegexMatch(RegexStrings.BypassedSoloClear, input, match);
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
        
        
        [TestCase("/application/ui/clipsscrollhorizontal")]
        [TestCase("/application/ui/clipsscrollvertical")]
        public void ClipScroll(string input, bool match = true)
        {
            AssertRegexMatch(RegexStrings.ApplicationClipScroll, input, match);
        }

        [TestCase("/composition/tempocontroller/tempotap")]
        [TestCase("/composition/tempocontroller/tempopull")]
        [TestCase("/composition/tempocontroller/tempopush")]
        public void TempoTapPullPush(string input, bool match = true)
        {
            AssertRegexMatch(RegexStrings.TempoTapPullPush, input, match);
        }
        
        [TestCase("/composition/layers/*/clips/*/transport/position")]
        [TestCase("/composition/layers/*/clips/*/transport/position/in")]
        [TestCase("/composition/layers/*/clips/*/transport/position/out")]
        [TestCase("/composition/layers/*/clips/*/transport/position/out/something", false)]
        [TestCase("/composition/layers/*/clips/*/transport/position/no", false)]
        public void TransportPosition(string input, bool match = true)
        {
            AssertRegexMatch(RegexStrings.TransportPosition, input, match);
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