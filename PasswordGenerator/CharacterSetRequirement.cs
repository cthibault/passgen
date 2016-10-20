namespace PasswordGenerator
{
    public class CharacterSetRequirement
    {
        public CharacterSetRequirement(CharacterSet characterSet, int minCount)
        {
            CharacterSet = characterSet;
            MinCount = minCount;
        }

        public CharacterSet CharacterSet { get; }
        public int MinCount { get; }

        public string GetDefinition()
        {
            return $"{CharacterSet.GetDefinition()}\r\nMin:\t{MinCount}";
        }

        public override string ToString()
        {
            return $"{CharacterSet.Name} - {MinCount}";
        }
    }
}