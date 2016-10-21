using System.Configuration;

namespace PasswordGenerator.CommandLine.Configuration
{
    class RuleSetConfig : ConfigurationSection
    {
        private const string SectionKey = "defaultRuleSet";
        private const string RequirementsKey = "requirements";
        private const string PasswordLengthKey = "passwordLength";

        public static RuleSetConfig DefaultRuleSetConfig => ConfigurationManager.GetSection(SectionKey) as RuleSetConfig;

        [ConfigurationProperty(RequirementsKey, IsRequired = true)]
        public CharacterSetRequirementCollection Requirements
        {
            get { return (CharacterSetRequirementCollection)base[RequirementsKey]; }
            set { CharacterSetRequirementCollection collection = value; }
        }

        [ConfigurationProperty(PasswordLengthKey, IsRequired = false, DefaultValue = CommandLine.Constants.DefaultPasswordLength)]
        public int PasswordLength
        {
            get { return (int)this[PasswordLengthKey]; }
            set { this[PasswordLengthKey] = value; }
        }
    }
}
