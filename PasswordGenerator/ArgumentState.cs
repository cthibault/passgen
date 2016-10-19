using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PasswordGenerator
{
    class ArgumentState
    {
        private readonly List<CharTypeRequirement> _requirements = new List<CharTypeRequirement>();
        private ArgumentState() { }

        public static ArgumentState Parse(string[] args)
        {
            var argumentState = new ArgumentState();

            char[] separator = new[] { KeyValueSeparator };

            foreach (string arg in args)
            {
                var keyValues = arg.Split(separator, 2);
                var argParam = ArgumentParameterEvaluators.SingleOrDefault(ap => ap.Keys.Any(k => k.Equals(keyValues[0], StringComparison.InvariantCultureIgnoreCase)));
                if (argParam == null)
                {
                    throw new InvalidDataException($"Invalid parameter: '{keyValues[0]}'");
                }

                var values = keyValues.Length > 1
                    ? keyValues[1].Split(separator, argParam.ExpectedValueCount)
                    : new string[0];

                argParam.Evalute(values, argumentState);
            }

            return argumentState;
        }

        public static string GetHelpText()
        {
            var builder = new StringBuilder();
            builder.AppendLine("PARAMETERS");

            foreach (var evaluator in ArgumentParameterEvaluators)
            {
                builder.AppendLine(evaluator.ToString());
                builder.AppendLine();
            }

            return builder.ToString();
        }

        public int Length { get; private set; } = DefaultPasswordLength;
        public int Count { get; private set; } = DefaultPasswordCount;

        public bool Verbose { get; private set; }
        public bool ShowHelp { get; private set; }
        public IEnumerable<CharTypeRequirement> Requirements => _requirements;

        public const int DefaultPasswordLength = 12;
        public const int DefaultPasswordCount = 1;

        private const char KeyValueSeparator = ':';

        private static readonly List<ArgumentParameterEvaluator> ArgumentParameterEvaluators = new List<ArgumentParameterEvaluator>
        {
            new ArgumentParameterEvaluator(
                name: "Password Length",
                description: "Length of the generated passwords",
                keys: new[] { "/length", "/len", "-length", "-len"},
                expectedValueCount: 1,
                evaluateFunction: (values, argument) =>
                {
                    argument.Length = int.Parse(values.First());
                }),

            new ArgumentParameterEvaluator(
                name: "Password Count",
                description: "Number of passwords to generate",
                keys: new[] {"/count", "-count"},
                expectedValueCount: 1,
                evaluateFunction: (values, argument) =>
                {
                    argument.Count = int.Parse(values.First());
                }),

            new ArgumentParameterEvaluator(
                name: "Lower Case Requirement",
                description: "Min number of lower case letters in generated password",
                keys: new[] {"/lower", "/l", "-lower", "-l"},
                expectedValueCount: 1,
                evaluateFunction: (values, argument) =>
                {
                    argument._requirements.Add(
                        new CharTypeRequirement(
                            charType: CharType.Default_LowerCaseLetters,
                            minCount: int.Parse(values.First())));
                }),

            new ArgumentParameterEvaluator(
                name: "Upper Case Requirement",
                description: "Min number of upper case letters in generated password",
                keys: new[] {"/upper", "/u", "-upper", "-u"},
                expectedValueCount: 1,
                evaluateFunction: (values, argument) =>
                {
                    argument._requirements.Add(
                        new CharTypeRequirement(
                            charType: CharType.Default_UpperCaseLetters,
                            minCount: int.Parse(values.First())));
                }),

            new ArgumentParameterEvaluator(
                name: "Number Requirement",
                description: "Min number of digits in generated password",
                keys: new[] {"/number", "/n", "-number", "-n"},
                expectedValueCount: 1,
                evaluateFunction: (values, argument) =>
                {
                    argument._requirements.Add(
                        new CharTypeRequirement(
                            charType: CharType.Default_Numbers,
                            minCount: int.Parse(values.First())));
                }),

            new ArgumentParameterEvaluator(
                name: "Special Character Requirement",
                description: "Min number of special characters in generated password",
                keys: new[] {"/special", "/s", "-special", "-s"},
                expectedValueCount: 1,
                evaluateFunction: (values, argument) =>
                {
                    argument._requirements.Add(
                        new CharTypeRequirement(
                            charType: CharType.Default_SpecialCharacters,
                            minCount: int.Parse(values.First())));
                }),

            new ArgumentParameterEvaluator(
                name: "Custom Set Requirement",
                description: "Min number of characters in the generated password from a provided set",
                keys: new[] {"/custom", "/c", "-custom", "-c"},
                expectedValueCount: 3,
                evaluateFunction: (values, argument) =>
                {
                    argument._requirements.Add(
                        new CharTypeRequirement(
                            charType: new CharType(
                                name: values.ElementAt(0), 
                                chars: values.ElementAt(2).ToCharArray()), 
                            minCount: int.Parse(values.ElementAt(1))));
                }),

            new ArgumentParameterEvaluator(
                name: "Verbose",
                description: "Include verbose details (i.e. set and character distributions, requirements, etc.)",
                keys: new[] { "/verbose", "/v", "-verbose", "-v"},
                expectedValueCount: 0,
                evaluateFunction: (values, argument) =>
                {
                    argument.Verbose = true;
                }),

            new ArgumentParameterEvaluator(
                name: "Help",
                description: "Show help",
                keys: new[] { "/help", "/h", "/?", "-help", "-h" },
                expectedValueCount: 0,
                evaluateFunction: (values, argument) =>
                {
                    argument.ShowHelp = true;
                }),
        };

        class ArgumentParameterEvaluator
        {
            public ArgumentParameterEvaluator(string name, string description, string[] keys, int expectedValueCount, Action<IEnumerable<string>, ArgumentState> evaluateFunction)
            {
                Name = name;
                Description = description;
                ExpectedValueCount = expectedValueCount;
                Keys = keys;
                EvaluateFunction = evaluateFunction;
            }

            public string Name { get; }
            public string Description { get; }
            public int ExpectedValueCount { get; }
            public string[] Keys { get; }
            private Action<IEnumerable<string>, ArgumentState> EvaluateFunction { get; }

            public void Evalute(IEnumerable<string> values, ArgumentState argumentState)
            {
                if (values.Count() < ExpectedValueCount)
                {
                    throw new InvalidDataException($"Invalid parameter data for '{Name}': {string.Join(", ", values)}.");
                }

                EvaluateFunction(values, argumentState);
            }

            public override string ToString()
            {
                return $"{Name}: {Description}\r\n{string.Join(" | ", Keys)}";
            }
        }
    }
}
