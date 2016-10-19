namespace PasswordGenerator
{
    public class CharType
    {
        public CharType(string name, char[] chars)
        {
            Name = name;
            Chars = chars;
        }

        public string Name { get; }
        public char[] Chars { get; }


        public override string ToString()
        {
            return $"Name:\t{Name}\r\nSet:\t{string.Join("", Chars)}";
        }


        public static CharType Default_UpperCaseLetters = new CharType("Default_UpperCaseLetters",
            new char[]
            {
                'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U',
                'V', 'W', 'X', 'Y', 'Z'
            });

        public static CharType Default_LowerCaseLetters = new CharType("Default_LowerCaseLetters",
            new char[]
            {
                'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u',
                'v', 'w', 'x', 'y', 'z'
            });

        public static CharType Default_Numbers = new CharType("Default_Numbers",
            new char[] {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9'});

        public static CharType Default_SpecialCharacters = new CharType("Default_SpecialCharacters",
            new char[]
            {
                '+', '-', '=', '_', '@', '#', '$', '%', '^', '&', ';', ':', ',', '.', '<', '>', '/', '~', '\\', '[', ']',
                '(', ')', '{', '}', '?', '!', '|'
            });
    }
}