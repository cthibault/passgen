using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using PasswordGenerator.CommandLine;

namespace PasswordGenerator
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                ExecutionContext executionContext = ExecutionContext.Parse(args, true);

                if (executionContext.ShowHelp)
                {
                    Console.WriteLine(ExecutionContext.GetHelpText());
                }
                else
                {
                    if (executionContext.Verbose)
                    {
                        ExecuteWithDistributionAnalysis(executionContext);
                    }
                    else
                    {
                        Execute(executionContext);
                    }
                }
            }
            catch (Exception ex)
            {
                var previousColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine(ex.Message);
                Console.ForegroundColor = previousColor;

                Environment.ExitCode = 1;
            }

            if (Debugger.IsAttached)
            {
                Console.Read();
            }
        }

        private static void Execute(ExecutionContext executionContext)
        {
            var generator = new PasswordGenerator(executionContext.RuleSet);
            for (int i = 0; i < executionContext.Count; i++)
            {
                string result = GeneratePassword(generator, executionContext.Length);

                Console.WriteLine(result);
            }
        }

        private static void ExecuteWithDistributionAnalysis(ExecutionContext executionContext)
        {
            var generator = new PasswordGenerator(executionContext.RuleSet);

            Dictionary<CharacterSet, int> distribution = executionContext.RuleSet.Requirements.ToDictionary(r => r.CharacterSet, r => 0);
            Dictionary<CharacterSet, Dictionary<char, int>> setDistribution =
                executionContext.RuleSet.Requirements.ToDictionary(r => r.CharacterSet, r => r.CharacterSet.Chars.ToDictionary(c => c, c => 0));

            for (int i = 0; i < executionContext.Count; i++)
            {
                string result = GeneratePassword(generator, executionContext.Length);

                Console.WriteLine(result);

                foreach (var c in result.ToCharArray())
                {
                    var type = distribution.Keys.Single(ct => ct.Chars.Contains(c));
                    distribution[type]++;
                    setDistribution[type][c]++;
                }
            }

            Console.WriteLine();
            Console.WriteLine("PASSWORD REQUIREMENTS");
            Console.WriteLine(string.Join("\r\n\r\n", executionContext.RuleSet.Requirements.Select(r => r.GetDefinition())));
            Console.WriteLine();

            double totalCharCount = distribution.Values.Sum();
            foreach (var kvp in setDistribution)
            {
                Console.WriteLine($"== {kvp.Key.Name} [{Math.Round(distribution[kvp.Key]/totalCharCount*100, 2)}] ==");

                double typeTotal = kvp.Value.Values.Sum();
                foreach (var cKvp in kvp.Value.OrderByDescending(x => x.Value))
                {
                    Console.WriteLine($"{cKvp.Key} - {cKvp.Value} [{Math.Round(cKvp.Value/typeTotal*100, 2)}]");
                }

                Console.WriteLine();
            }
        }

        private static string GeneratePassword(PasswordGenerator generator, int length)
        {
            string result;

            using (var secureGenerated = generator.Generate(length))
            {
                IntPtr ptr = Marshal.SecureStringToBSTR(secureGenerated);
                result = Marshal.PtrToStringUni(ptr);
            }
            
            return result;
        }
    }
}
