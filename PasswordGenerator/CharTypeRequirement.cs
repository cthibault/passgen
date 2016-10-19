namespace PasswordGenerator
{
    public class CharTypeRequirement
    {
        public CharTypeRequirement(CharType charType, int minCount)
        {
            CharType = charType;
            MinCount = minCount;
        }

        public CharType CharType { get; }
        public int MinCount { get; }

        public override string ToString()
        {
            return $"{CharType.ToString()}\r\nMin:\t{MinCount}";
        }
    }
}