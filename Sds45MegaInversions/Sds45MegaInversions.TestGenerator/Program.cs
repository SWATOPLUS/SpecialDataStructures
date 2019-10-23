using System;
using System.IO;
using System.Linq;
using MoreLinq;

namespace Sds45MegaInversions.TestGenerator
{
    internal static class Program
    {
        private const string OutputFileName = "input.txt";
        private const int TestSize = 100_000;
        private static readonly Random Random = new Random(42);

        private static void Main(string[] args)
        {
            var items = Enumerable.Range(0, TestSize)
                .Select(x => x + 1)
                .Shuffle(Random)
                .Select(x => x.ToString());

            var lines = new[]
            {
                TestSize.ToString(),
            }.Concat(items);

            File.WriteAllLines(OutputFileName, lines);
        }
    }
}
