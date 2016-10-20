using PasswordGenerator.Random;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace PasswordGenerator
{
    public class PasswordGenerator
    {
        public PasswordGenerator(RuleSet ruleSet)
        {
            RuleSet = ruleSet;
        }
        
        public RuleSet RuleSet { get; }

        public SecureString Generate(int length)
        {
            return PasswordGeneratorInteral.Generate(RuleSet, length);
        }


        private class PasswordGeneratorInteral
        {
            private PasswordGeneratorInteral(RuleSet ruleSet, int length) : this(ruleSet, length, new RandomProvider())
            { }
            private PasswordGeneratorInteral(RuleSet ruleSet, int length, IRandomProvider randomProvider)
            {
                if (ruleSet == null)
                {
                    throw new ArgumentNullException(nameof(ruleSet));
                }

                if (!ruleSet.IsValid)
                {
                    throw new ArgumentException("Invalid Rule Set", nameof(ruleSet));
                }

                if (randomProvider == null)
                {
                    throw new ArgumentNullException(nameof(randomProvider));
                }

                if (length < ruleSet.MinLength)
                {
                    throw new ArgumentOutOfRangeException(nameof(length), length, $"{nameof(length)} is shorter than what is needed to meet the requirements ({ruleSet.MinLength}).");
                }

                Length = length;
                SetResults = ruleSet.Requirements.Select(r => new CharacterSetResult(r)).ToList();
                RandomProvider = randomProvider;
                GeneratedPassword = new SecureString();
            }

            private int Length { get; }
            private List<CharacterSetResult> SetResults { get; }
            private IRandomProvider RandomProvider { get; }
            private SecureString GeneratedPassword { get; }

            public static SecureString Generate(RuleSet ruleSet, int length)
            {
                var generator = new PasswordGeneratorInteral(ruleSet, length);

                return generator.Generate();
            }

            private SecureString Generate()
            {
                for (int i = 0; i < Length; i++)
                {
                    GenerateNextCharacter(i, SetResults);
                }

                var incompleteTypes = SetResults.Where(r => r.RequirementDelta < 0);
                while (incompleteTypes.Any())
                {
                    var overCompleteType = GetNextCharTypeResult(SetResults.Where(r => r.RequirementDelta > 0));
                    var charIndexToReplace = overCompleteType.Indicies.ElementAt(RandomProvider.Next(overCompleteType.Indicies.Count()));
                    overCompleteType.RemoveIndex(charIndexToReplace);

                    GenerateNextCharacter(charIndexToReplace, incompleteTypes, true);
                }

                GeneratedPassword.MakeReadOnly();

                return GeneratedPassword;
            }

            private void GenerateNextCharacter(int index, IEnumerable<CharacterSetResult> setResults, bool replace = false)
            {
                var typeResult = GetNextCharTypeResult(setResults);
                char character = GetNextCharacter(typeResult.CharacterSet.Chars);

                typeResult.AddIndex(index);

                if (replace)
                {
                    GeneratedPassword.SetAt(index, character);
                }
                else
                {
                    GeneratedPassword.AppendChar(character);
                }
            }
            private char GetNextCharacter(char[] characters)
            {
                int index = RandomProvider.Next(characters.Length);

                return characters[index];
            }

            private CharacterSetResult GetNextCharTypeResult(IEnumerable<CharacterSetResult> setResults)
            {
                if (setResults == null || !setResults.Any())
                {
                    throw new ArgumentOutOfRangeException(nameof(setResults));
                }

                int index = RandomProvider.Next(setResults.Count());

                return setResults.ElementAt(index);
            }
        }

        private class CharacterSetResult
        {
            private readonly List<int> _indicies = new List<int>();

            public CharacterSetResult(CharacterSet characterSet, int minCount)
            {
                CharacterSet = characterSet;
                MinCount = minCount;
            }

            public CharacterSetResult(CharacterSetRequirement requirement)
            {
                if (requirement == null)
                {
                    throw new ArgumentNullException(nameof(requirement));
                }

                CharacterSet = requirement.CharacterSet;
                MinCount = requirement.MinCount;
            }

            public CharacterSet CharacterSet { get; }
            public int MinCount { get; }

            public int RequirementDelta => _indicies.Count - MinCount;

            public IEnumerable<int> Indicies => _indicies;

            public void AddIndex(int index)
            {
                _indicies.Add(index);
            }

            public void RemoveIndex(int index)
            {
                _indicies.Remove(index);
            }
        }
    }
}