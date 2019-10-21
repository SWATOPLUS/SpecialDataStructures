using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sds2SegmentSum.TestGenerator
{
    internal class Program
    {
        private const string OutputFileName = "input.txt";
        private const int QueryCount = 100_000;
        private const int TreeSize = 100_000;
        private const int ElementSize = 1_000_000_000;

        private static readonly Random Random = new Random(42);

        private static void Main()
        {
            var elements = BuildRandomElements();
            var commands = Enumerable.Range(0, QueryCount)
                .Select(x => BuildRandomCommand());
            
            var lines = new[]
            {
                $"{TreeSize} {QueryCount}",
                string.Join(" ", elements)
            }.Concat(commands);

            File.WriteAllLines(OutputFileName, lines);
        }

        private static IEnumerable<int> BuildRandomElements()
        {
            return Enumerable.Range(0, TreeSize)
                .Select(x => Random.Next(1, ElementSize + 1));
        }

        private static string BuildRandomCommand()
        {
            var opCode = Random.Next(1, 5);

            switch (opCode)
            {
                case 1:
                    return $"{opCode} {BuildIndex()} {BuildElement()}";
                case 2:
                    return $"{opCode} {BuildIndexPair()}";
                case 3:
                    return $"{opCode} {BuildIndexPair()}";
                case 4:
                    return $"{opCode} {BuildIndexPair()}";
                default:
                    throw new InvalidOperationException();
            }

        }

        private static int BuildElement()
        {
            return Random.Next(1, ElementSize + 1);
        }

        private static int BuildIndex(int start = 1)
        {
            return Random.Next(start, TreeSize + 1);
        }

        private static string BuildIndexPair()
        {
            var first = BuildIndex();
            var second = BuildIndex(first);

            return $"{first} {second}";
        }
    }
}
