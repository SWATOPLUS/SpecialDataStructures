using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sds4PersistentSegmentTree.TestGenerator
{
    internal static class Program
    {
        private const string OutputFileName = "input.txt";

        private static int ArraySize = 200_000;
        private static int QueryCount = 200_000;
        private static int ItemSize = 1_000_000;

        private static readonly Random Random = new Random();

        private static void Main(string[] args)
        {
            if (args.Length == 1)
            {
                ArraySize = int.Parse(args[0]);
            }

            if (args.Length == 2)
            {
                ArraySize = int.Parse(args[0]);
                QueryCount = int.Parse(args[1]);
            }

            if (args.Length == 3)
            {
                ArraySize = int.Parse(args[0]);
                QueryCount = int.Parse(args[1]);
                ItemSize = int.Parse(args[2]);
            }

            var array = GenerateArray(ArraySize, ItemSize);
            var queries = Enumerable.Range(0, QueryCount)
                .Select(x => GenerateQuery(ArraySize, ItemSize));

            var output = new[]
                {
                    $"{ArraySize} {QueryCount}",
                    string.Join(" ", array),
                }
                .Concat(queries);

            File.WriteAllLines(OutputFileName, output);
        }

        private static IEnumerable<int> GenerateArray(int arraySize, int itemSize)
        {
            return Enumerable.Range(0, arraySize)
                .Select(x => Random.NextNInt(itemSize));
        }

        private static string GenerateQuery(int arraySize, int itemSize)
        {
            var l = Random.NextNInt(arraySize);
            var r = Random.NextNInt(l, arraySize);

            var x = Random.NextNInt(itemSize);
            var y = Random.NextNInt(x, itemSize);

            return $"{l} {r} {x} {y}";
        }
    }

    internal static class RandomExtensions
    {
        public static int NextNInt(this Random random, int max) => random.Next(1, max + 1);

        public static int NextNInt(this Random random, int min, int max) => random.Next(min, max + 1);
    }
}
