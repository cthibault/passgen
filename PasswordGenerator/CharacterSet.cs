namespace PasswordGenerator
{
    public class CharacterSet
    {
        public CharacterSet(string name, char[] chars)
        {
            Name = name;
            Chars = chars;
        }

        public string Name { get; }
        public char[] Chars { get; }

        public string GetDefinition()
        {
            return $"Name:\t{Name}\r\nSet:\t{string.Join("", Chars)}";
        }

        public override string ToString()
        {
            return Name;
        }


        public static CharacterSet Default_UpperCaseLetters = new CharacterSet("Default_UpperCaseLetters",
            new char[]
            {
                'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U',
                'V', 'W', 'X', 'Y', 'Z'
            });

        public static CharacterSet Default_LowerCaseLetters = new CharacterSet("Default_LowerCaseLetters",
            new char[]
            {
                'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u',
                'v', 'w', 'x', 'y', 'z'
            });

        public static CharacterSet Default_Numbers = new CharacterSet("Default_Numbers",
            new char[] {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9'});

        public static CharacterSet Default_SpecialCharacters = new CharacterSet("Default_SpecialCharacters",
            new char[]
            {
                '+', '-', '=', '_', '@', '#', '$', '%', '^', '&', ';', ':', ',', '.', '<', '>', '/', '~', '\\', '[', ']',
                '(', ')', '{', '}', '?', '!', '|'
            });
    }
}