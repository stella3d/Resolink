namespace Resolink
{
    [System.Serializable]
    public class ShortcutPath
    {
        public string Name;

        /// <summary>
        /// The OSC address this message is associated with
        /// </summary>
        public string Path;
        
        public int TranslationType;
        public int AllowedTranslationTypes;
    }
}