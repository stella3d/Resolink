using System;

namespace Resolink
{
    [Serializable]
    public class ResolumeOscShortcut
    {
        public long UniqueId;
        public ShortcutPath Input;
        public ShortcutPath Output;
        public SubTarget[] SubTargets;                // not all Shortcuts have subtargets
        public string TypeName;
    }
}