namespace Resolink
{
    public enum VideoSharingProtocol
    {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
        Spout,
#endif
#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
        Syphon,
#endif
        NDI
    }
}
