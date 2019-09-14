using NUnit.Framework;

namespace Resolink.Tests
{
    public static class ResolumeOscShortcutExtensions
    {
        public static void AssertDeepEqual(this ResolumeOscMap self, ResolumeOscMap other)
        {
            self.AssertSameShortcuts(other);
            self.AssertSameColorGroups(other);
            self.AssertSameVector2Groups(other);
            self.AssertSameVector3Groups(other);
        }

        public static void AssertSameShortcuts(this ResolumeOscMap self, ResolumeOscMap other)
        {
            Assert.AreEqual(other.Shortcuts.Count, self.Shortcuts.Count);
            for (int i = 0; i < self.Shortcuts.Count; i++)
                AssertDeepEqual(other.Shortcuts[i], self.Shortcuts[i]);
        }
        
        public static void AssertSameColorGroups(this ResolumeOscMap self, ResolumeOscMap other)
        {
            Assert.AreEqual(other.ColorGroups.Count, self.ColorGroups.Count);
            for (int i = 0; i < self.ColorGroups.Count; i++)
                AssertEqual(other.ColorGroups[i], self.ColorGroups[i]);
        }
        
        public static void AssertSameVector2Groups(this ResolumeOscMap self, ResolumeOscMap other)
        {
            Assert.AreEqual(other.Vector2Groups.Count, self.Vector2Groups.Count);
            for (int i = 0; i < self.Vector2Groups.Count; i++)
                AssertEqual(other.Vector2Groups[i], self.Vector2Groups[i]);
        }
        
        public static void AssertSameVector3Groups(this ResolumeOscMap self, ResolumeOscMap other)
        {
            Assert.AreEqual(other.Vector3Groups.Count, self.Vector3Groups.Count);
            for (int i = 0; i < self.Vector3Groups.Count; i++)
                AssertEqual(other.Vector3Groups[i], self.Vector3Groups[i]);
        }
        
        public static void AssertDeepEqual(this ResolumeOscShortcut self, ResolumeOscShortcut other)
        {
            AssertShortcutPathsEqual(self.Input, other.Input);
            AssertShortcutPathsEqual(self.Output, other.Output);
            Assert.AreEqual(other.TypeName, self.TypeName);
            Assert.AreEqual(other.UniqueId, self.UniqueId);
            AssertSubTargetsEqual(self.SubTargets, other.SubTargets);
        }
        
        public static void AssertEqual(this ColorShortcutGroup self, ColorShortcutGroup other)
        {
            AssertDeepEqual(other.Red, self.Red);
            AssertDeepEqual(other.Green, self.Green);
            AssertDeepEqual(other.Blue, self.Blue);
            AssertDeepEqual(other.Alpha, self.Alpha);
        }
        
        public static void AssertEqual(this Vector2ShortcutGroup self, Vector2ShortcutGroup other)
        {
            Assert.AreEqual(other.X, self.X);
            Assert.AreEqual(other.Y, self.Y);
        }
        
        public static void AssertEqual(this Vector3ShortcutGroup self, Vector3ShortcutGroup other)
        {
            Assert.AreEqual(other.X, self.X);
            Assert.AreEqual(other.Y, self.Y);
            Assert.AreEqual(other.Z, self.Z);
        }
        
        public static void AssertShortcutPathsEqual(this ShortcutPath self, ShortcutPath other)
        {
            if (self == null)
            {
                Assert.Null(other);
                return;
            }

            Assert.AreEqual(other.Name, self.Name, 
                $"Expected shortcut path name to equal {other.Name}, but it was {self.Name}");
            Assert.AreEqual(other.Path, self.Path,
                $"Expected shortcut path to equal {other.Path}, but it was {self.Path}");
            // ignore the translation type fields for now
        }

        public static void AssertSubTargetsEqual(this SubTarget[] self, SubTarget[] other)
        {
            Assert.AreEqual(other.Length, self.Length);
            for (var i = 0; i < self.Length; i++)
            {
                var s = self[i];
                var o = other[i];
                Assert.AreEqual(o.Type, s.Type);
                Assert.AreEqual(o.OptionIndex, s.OptionIndex);
            }
        }
    }
}