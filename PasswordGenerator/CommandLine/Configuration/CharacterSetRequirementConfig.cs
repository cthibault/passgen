using System.Configuration;

namespace PasswordGenerator.CommandLine.Configuration
{
    public class CharacterSetRequirementConfig : ConfigurationElement
    {
        private const string NameKey = "name";
        private const string CharactersKey = "characters";
        private const string MinCountKey = "minCount";


        public CharacterSetRequirementConfig()
        {
        }

        public CharacterSetRequirementConfig(string name, string characters, int minCount)
        {
            Name = name;
            Characters = characters;
            MinCount = minCount;
        }


        [ConfigurationProperty(NameKey, IsRequired = true)]
        public string Name
        {
            get { return (string) this[NameKey]; }
            set { this[NameKey] = value; }
        }

        [ConfigurationProperty(CharactersKey, IsRequired = true)]
        private string Characters
        {
            get { return this[CharactersKey].ToString(); }
            set { this[CharactersKey] = value; }
        }

        public char[] CharacterArray => Characters?.ToCharArray();

        [ConfigurationProperty(MinCountKey, IsRequired = false, DefaultValue = 1)]
        [IntegerValidator(ExcludeRange = false, MaxValue = 100, MinValue = 1)]
        public int MinCount
        {
            get { return (int)this[MinCountKey]; }
            set { this[MinCountKey] = value; }
        }
    }
}