using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordGenerator
{
    public class RuleSet
    {
        private readonly List<CharacterSetRequirement> _requirements;

        public RuleSet()
        {
            _requirements = new List<CharacterSetRequirement>();
        }
        public RuleSet(IEnumerable<CharacterSetRequirement> requirements)
        {
            _requirements = requirements.ToList();
        }

        public IEnumerable<CharacterSetRequirement> Requirements => _requirements;

        public bool IsValid => _requirements.Any();

        public int MinLength => _requirements.Sum(r => r.MinCount);

        public bool TryAddNewRequirement(CharacterSetRequirement requirement)
        {
            var existingRequirement = _requirements.SingleOrDefault(r => r.CharacterSet.Name.Equals(requirement.CharacterSet.Name));

            if (existingRequirement != null)
            {
                return false;
            }

            _requirements.Add(requirement);
            return true;
        }

        public bool TryRemoveExistingRequirement(CharacterSetRequirement requirement)
        {
            var existingRequirement = _requirements.SingleOrDefault(r => r.CharacterSet.Name.Equals(requirement.CharacterSet.Name));

            if (existingRequirement == null)
            {
                return false;
            }

            return _requirements.Remove(existingRequirement);
        }

        public bool AddOrReplaceRequirement(CharacterSetRequirement requirement)
        {
            TryRemoveExistingRequirement(requirement);

            return TryAddNewRequirement(requirement);
        }


        public static RuleSet Default = new RuleSet(
            new List<CharacterSetRequirement>
            {
                new CharacterSetRequirement(CharacterSet.Default_LowerCaseLetters, 1),
                new CharacterSetRequirement(CharacterSet.Default_UpperCaseLetters, 1),
                new CharacterSetRequirement(CharacterSet.Default_Numbers, 1),
                new CharacterSetRequirement(CharacterSet.Default_SpecialCharacters, 1)
            });
    }
}
