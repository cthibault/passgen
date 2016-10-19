using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading.Tasks;

namespace PasswordGenerator
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            ArgumentState argumentState = null;

            try
            {
                argumentState = ArgumentState.Parse(args);
            }
            catch (Exception ex)
            {
                var previousColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine(ex.Message);
                Console.ForegroundColor = previousColor;

                Environment.Exit(1);

            }

            if (argumentState.ShowHelp)
            {
                Console.WriteLine(ArgumentState.GetHelpText());
            }
            else
            {
                if (argumentState.Verbose)
                {
                    ExecuteWithDistributionAnalysis(argumentState);
                }
                else
                {
                    Execute(argumentState);
                }
            }

            if (Debugger.IsAttached)
            {
                Console.Read();
            }
        }

        private static void Execute(ArgumentState argumentState)
        {
            var generator = new PasswordGenerator(argumentState.Requirements);
            for (int i = 0; i < argumentState.Count; i++)
            {
                string result = GeneratePassword(generator, argumentState.Length);

                Console.WriteLine(result);
            }
        }

        private static void ExecuteWithDistributionAnalysis(ArgumentState argumentState)
        {
            var generator = new PasswordGenerator(argumentState.Requirements);

            Dictionary<CharType, int> distribution = argumentState.Requirements.ToDictionary(r => r.CharType, r => 0);
            Dictionary<CharType, Dictionary<char, int>> typeDistribution =
                argumentState.Requirements.ToDictionary(r => r.CharType, r => r.CharType.Chars.ToDictionary(c => c, c => 0));

            for (int i = 0; i < argumentState.Count; i++)
            {
                string result = GeneratePassword(generator, argumentState.Length);

                Console.WriteLine(result);

                foreach (var c in result.ToCharArray())
                {
                    var type = distribution.Keys.Single(ct => ct.Chars.Contains(c));
                    distribution[type]++;
                    typeDistribution[type][c]++;
                }
            }

            Console.WriteLine();
            Console.WriteLine("PASSWORD REQUIREMENTS");
            Console.WriteLine(string.Join("\r\n\r\n", argumentState.Requirements.Select(r => r.ToString())));
            Console.WriteLine();

            double totalCharCount = distribution.Values.Sum();
            foreach (var kvp in typeDistribution)
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
