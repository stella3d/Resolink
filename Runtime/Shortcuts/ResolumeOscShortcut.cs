using System;

namespace UnityResolume
{
    [Serializable]
    public class ResolumeOscShortcut
    {
        public long UniqueId;
        public ShortcutPath Input;
        public ShortcutPath Output;
        public SubTarget[] SubTargets;                // not all Shortcuts have subtargets
        public string ParamNodeName;
        public Type DataType;
    }
}