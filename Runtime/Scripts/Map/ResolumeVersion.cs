namespace Resolink
{
    [System.Serializable]
    public struct ResolumeVersion
    {
        public int Major;
        public int Minor;
        public int Micro;

        public override string ToString()
        {
            return $"{Major}.{Minor}.{Micro}";
        }
    }
}