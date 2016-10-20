using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordGenerator.CommandLine
{
    class ArgumentEvaluator
    {
        public ArgumentEvaluator(string name, string description, string[] keys, int expectedValueCount, Action<IEnumerable<string>, ExecutionContext> evaluateFunction)
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
        private Action<IEnumerable<string>, ExecutionContext> EvaluateFunction { get; }

        public void Evalute(IEnumerable<string> values, ExecutionContext context)
        {
            if (values.Count() < ExpectedValueCount)
            {
                throw new InvalidDataException($"Invalid parameter data for '{Name}': {string.Join(", ", values)}.");
            }

            EvaluateFunction(values, context);
        }

        public override string ToString()
        {
            return $"{Name}: {Description}\r\n{string.Join(" | ", Keys)}";
        }
    }
}
