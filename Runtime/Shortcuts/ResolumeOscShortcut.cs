namespace UnityResolume
{
    [System.Serializable]
    public class ResolumeOscShortcut
    {
        public long UniqueId;
        public ShortcutPath Input;
        public ShortcutPath Output;
        // not all Shortcuts have subtargets
        public SubTarget SubTarget;
    }
}