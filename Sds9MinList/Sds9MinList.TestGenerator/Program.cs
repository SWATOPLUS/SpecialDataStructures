using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sds9MinList.TestGenerator
{
    internal class Program
    {
        private const string OutputFileName = "input.txt";
        private const int TestSize = 1_000_000;
        private const int ItemSize = 1_000_000;
        private static readonly Random Random = new Random(42);

        private static void Main()
        {
            var commands = BuildRandomCommands(TestSize);

            var lines = new[] {TestSize.ToString()}.Concat(commands);

            File.WriteAllLines(OutputFileName, lines);
        }

        private static IEnumerable<string> BuildRandomCommands(int size)
        {
            var items = 0;

            foreach (var _ in Enumerable.Range(0, size))
            {
                var randomRange = items == 0 ? 2 : 4;
                var command = BuildCommand(Random.Next(randomRange));

                if (command.First() == '+')
                {
                    items++;
                }
                else
                {
                    items--;
                }

                yield return command;
            }
        }

        private static string BuildCommand(int opCode)
        {
            switch (opCode)
            {
                case 0:
                    return $"{CommandOperator.PushFront} {Random.Next(ItemSize)}";
                case 1:
                    return $"{CommandOperator.PushBack} {Random.Next(ItemSize)}";
                case 2:
                    return CommandOperator.PopFront;
                case 3:
                    return CommandOperator.PopBack;
                default:
                    throw new InvalidOperationException();
            }
        }
    }

    internal static class CommandOperator
    {
        public const string PushFront = "+L";
        public const string PushBack = "+R";
        public const string PopFront = "-L";
        public const string PopBack = "-R";
    }

}
