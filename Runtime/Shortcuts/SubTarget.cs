namespace UnityResolume
{
    [System.Serializable]
    public class SubTarget
    {
        public int Type;
        public int OptionIndex;

        public bool IsValid()
        {
            return Type != 0 || OptionIndex != 0;
        }
    }
}