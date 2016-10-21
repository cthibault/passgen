using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using PasswordGenerator.CommandLine.Configuration;

namespace PasswordGenerator.CommandLine
{
    sealed class ExecutionContext
    {
        private ExecutionContext()
        {
            RuleSet = new RuleSet();
        }

        public int Length { get; private set; } = GetDefaultPasswordLength();
        public int Count { get; private set; } = Constants.DefaultPasswordCount;

        public bool Verbose { get; private set; }
        public bool ShowHelp { get; private set; }
        public RuleSet RuleSet { get; private set; }

        public static ExecutionContext Parse(string[] args, bool defaultFallback = false)
        {
            var context = new ExecutionContext();

            char[] separator = new[] { Constants.KeyValueSeparator };

            foreach (string arg in args)
            {
                var keyValues = arg.Split(separator, 2);

                var evaluator = ArgumentEvaluators.SingleOrDefault(ap => ap.Keys.Any(k => k.Equals(keyValues[0], StringComparison.InvariantCultureIgnoreCase)));

                if (evaluator == null)
                {
                    throw new InvalidDataException($"Invalid parameter: '{keyValues[0]}'");
                }

                var values = keyValues.Length > 1
                    ? keyValues[1].Split(separator, evaluator.ExpectedValueCount)
                    : new string[0];

                evaluator.Evalute(values, context);
            }
            
            if (!context.RuleSet.IsValid && defaultFallback)
            {
                context.RuleSet = GetDefaultRuleSet();
            }

            return context;
        }

        public static RuleSet GetDefaultRuleSet()
        {
            RuleSet ruleSet = RuleSet.Default;

            RuleSetConfig config = GetDefaultRuleSetConfig();
            if (config != null)
            {
                IEnumerable<CharacterSetRequirement> requirements = RuleSetConfig.DefaultRuleSetConfig.Requirements
                    .All
                    .Select(
                        r => new CharacterSetRequirement(new CharacterSet(r.Name, r.CharacterArray), r.MinCount));

                ruleSet = new RuleSet(requirements);
            }

            return ruleSet;
        }

        public static int GetDefaultPasswordLength()
        {
            int length = Constants.DefaultPasswordLength;

            RuleSetConfig config = GetDefaultRuleSetConfig();
            if (config != null)
            {
                length = config.PasswordLength;
            }

            return length;
        }

        public static string GetHelpText()
        {
            var builder = new StringBuilder();
            builder.AppendLine("PARAMETERS");

            foreach (var evaluator in ArgumentEvaluators)
            {
                builder.AppendLine(evaluator.ToString());
                builder.AppendLine();
            }

            return builder.ToString();
        }

        private static RuleSetConfig GetDefaultRuleSetConfig()
        {
            RuleSetConfig config = null;

            try
            {
                config = RuleSetConfig.DefaultRuleSetConfig;
            }
            catch (Exception ex)
            {
                var previousColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine(ex.Message);
                Console.ForegroundColor = previousColor;

                config = null;
            }

            return config;
        }

        #region Argument Evaluator Definitions
        private static readonly List<ArgumentEvaluator> ArgumentEvaluators = new List<ArgumentEvaluator>
        {
            new ArgumentEvaluator(
                name: "Password Length",
                description: "Length of the generated passwords",
                keys: new[] { "/length", "/len", "-length", "-len"},
                expectedValueCount: 1,
                evaluateFunction: (values, argument) =>
                {
                    argument.Length = int.Parse(values.First());
                }),

            new ArgumentEvaluator(
                name: "Password Count",
                description: "Number of passwords to generate",
                keys: new[] {"/count", "-count"},
                expectedValueCount: 1,
                evaluateFunction: (values, argument) =>
                {
                    argument.Count = int.Parse(values.First());
                }),

            new ArgumentEvaluator(
                name: "Lower Case Requirement",
                description: "Min number of lower case letters in generated password",
                keys: new[] {"/lower", "/l", "-lower", "-l"},
                expectedValueCount: 1,
                evaluateFunction: (values, argument) =>
                {
                    argument.RuleSet.AddOrReplaceRequirement(
                        new CharacterSetRequirement(
                            characterSet: CharacterSet.Default_LowerCaseLetters,
                            minCount: int.Parse(values.First())));
                }),

            new ArgumentEvaluator(
                name: "Upper Case Requirement",
                description: "Min number of upper case letters in generated password",
                keys: new[] {"/upper", "/u", "-upper", "-u"},
                expectedValueCount: 1,
                evaluateFunction: (values, argument) =>
                {
                    argument.RuleSet.AddOrReplaceRequirement(
                        new CharacterSetRequirement(
                            characterSet: CharacterSet.Default_UpperCaseLetters,
                            minCount: int.Parse(values.First())));
                }),

            new ArgumentEvaluator(
                name: "Number Requirement",
                description: "Min number of digits in generated password",
                keys: new[] {"/number", "/n", "-number", "-n"},
                expectedValueCount: 1,
                evaluateFunction: (values, argument) =>
                {
                    argument.RuleSet.AddOrReplaceRequirement(
                        new CharacterSetRequirement(
                            characterSet: CharacterSet.Default_Numbers,
                            minCount: int.Parse(values.First())));
                }),

            new ArgumentEvaluator(
                name: "Special Character Requirement",
                description: "Min number of special characters in generated password",
                keys: new[] {"/special", "/s", "-special", "-s"},
                expectedValueCount: 1,
                evaluateFunction: (values, argument) =>
                {
                    argument.RuleSet.AddOrReplaceRequirement(
                        new CharacterSetRequirement(
                            characterSet: CharacterSet.Default_SpecialCharacters,
                            minCount: int.Parse(values.First())));
                }),

            new ArgumentEvaluator(
                name: "Custom Set Requirement",
                description: "Min number of characters in the generated password from a provided set",
                keys: new[] {"/custom", "/c", "-custom", "-c"},
                expectedValueCount: 3,
                evaluateFunction: (values, argument) =>
                {
                    argument.RuleSet.AddOrReplaceRequirement(
                        new CharacterSetRequirement(
                            characterSet: new CharacterSet(
                                name: values.ElementAt(0),
                                chars: values.ElementAt(2).ToCharArray()),
                            minCount: int.Parse(values.ElementAt(1))));
                }),

            new ArgumentEvaluator(
                name: "Verbose",
                description: "Include verbose details (i.e. set and character distributions, requirements, etc.)",
                keys: new[] { "/verbose", "/v", "-verbose", "-v"},
                expectedValueCount: 0,
                evaluateFunction: (values, argument) =>
                {
                    argument.Verbose = true;
                }),

            new ArgumentEvaluator(
                name: "Help",
                description: "Show help",
                keys: new[] { "/help", "/h", "/?", "-help", "-h" },
                expectedValueCount: 0,
                evaluateFunction: (values, argument) =>
                {
                    argument.ShowHelp = true;
                }),
        }; 
        #endregion
    }
}
