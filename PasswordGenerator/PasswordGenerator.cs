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
        public PasswordGenerator(IEnumerable<CharTypeRequirement> requirements)
        {
            Requirements = new ReadOnlyCollection<CharTypeRequirement>(requirements.ToArray());
        }
        
        public IReadOnlyCollection<CharTypeRequirement> Requirements { get; }

        public SecureString Generate(int length)
        {
            return PasswordGeneratorInteral.Generate(Requirements, length);
        }


        private class PasswordGeneratorInteral
        {
            private PasswordGeneratorInteral(IEnumerable<CharTypeRequirement> requirements, int length) : this(requirements, length, new RandomProvider())
            { }
            private PasswordGeneratorInteral(IEnumerable<CharTypeRequirement> requirements, int length, IRandomProvider randomProvider)
            {
                if (requirements == null)
                {
                    throw new ArgumentNullException(nameof(requirements));
                }

                if (!requirements.Any())
                {
                    throw new ArgumentOutOfRangeException(nameof(requirements));
                }

                if (randomProvider == null)
                {
                    throw new ArgumentNullException(nameof(randomProvider));
                }

                int requirementsMinCount = requirements.Sum(r => r.MinCount);
                if (length < requirementsMinCount)
                {
                    throw new ArgumentOutOfRangeException(nameof(length), length, $"{nameof(length)} is shorter than the requirements specify ({requirementsMinCount}).");
                }

                Length = length;
                RequirementResults = requirements.Select(r => new CharTypeResult(r)).ToList();
                RandomProvider = randomProvider;
                GeneratedPassword = new SecureString();
            }

            private int Length { get; }
            private List<CharTypeResult> RequirementResults { get; }
            private IRandomProvider RandomProvider { get; }
            private SecureString GeneratedPassword { get; }

            public static SecureString Generate(IEnumerable<CharTypeRequirement> requirements, int length)
            {
                var generator = new PasswordGeneratorInteral(requirements, length);

                return generator.Generate();
            }

            private SecureString Generate()
            {
                for (int i = 0; i < Length; i++)
                {
                    GenerateNextCharacter(i, RequirementResults);
                }

                var incompleteTypes = RequirementResults.Where(r => r.RequirementDelta < 0);
                while (incompleteTypes.Any())
                {
                    var overCompleteType = GetNextCharTypeResult(RequirementResults.Where(r => r.RequirementDelta > 0));
                    var charIndexToReplace = overCompleteType.Indicies.ElementAt(RandomProvider.Next(overCompleteType.Indicies.Count()));
                    overCompleteType.RemoveIndex(charIndexToReplace);

                    GenerateNextCharacter(charIndexToReplace, incompleteTypes, true);
                }

                GeneratedPassword.MakeReadOnly();

                return GeneratedPassword;
            }

            private void GenerateNextCharacter(int index, IEnumerable<CharTypeResult> typeResults, bool replace = false)
            {
                var typeResult = GetNextCharTypeResult(typeResults);
                char character = GetNextCharacter(typeResult.CharType.Chars);

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

            private CharTypeResult GetNextCharTypeResult(IEnumerable<CharTypeResult> typeResults)
            {
                if (typeResults == null || !typeResults.Any())
                {
                    throw new ArgumentOutOfRangeException(nameof(typeResults));
                }

                int index = RandomProvider.Next(typeResults.Count());

                return typeResults.ElementAt(index);
            }
        }

        private class CharTypeResult
        {
            private readonly List<int> _indicies = new List<int>();

            public CharTypeResult(CharType charType, int minCount)
            {
                CharType = charType;
                MinCount = minCount;
            }

            public CharTypeResult(CharTypeRequirement requirement)
            {
                if (requirement == null)
                {
                    throw new ArgumentNullException(nameof(requirement));
                }

                CharType = requirement.CharType;
                MinCount = requirement.MinCount;
            }

            public CharType CharType { get; }
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