using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace PasswordGenerator.CommandLine.Configuration
{
    public class CharacterSetRequirementCollection : ConfigurationElementCollection
    {
        public List<CharacterSetRequirementConfig> All => this.Cast<CharacterSetRequirementConfig>().ToList();

        public CharacterSetRequirementConfig this[int index]
        {
            get { return (CharacterSetRequirementConfig) BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new CharacterSetRequirementConfig();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((CharacterSetRequirementConfig) element).Name;
        }
    }
}