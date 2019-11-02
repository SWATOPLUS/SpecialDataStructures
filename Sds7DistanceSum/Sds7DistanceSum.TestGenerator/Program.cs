using System;
using System.IO;
using System.Linq;

namespace Sds7DistanceSum.TestGenerator
{
    internal class Program
    {
        private const string OutputFileName = "input.txt";
        private static int MaxGraphSize = 2000;

        private static readonly Random Random = new Random();

        private static void Main(string[] args)
        {
            if (args.Length == 1)
            {
                MaxGraphSize = int.Parse(args[0]);
            }

            var graphSize = Random.NextNInt(MaxGraphSize);
            var rows = Enumerable.Range(0, graphSize)
                .Select(x => GenerateRow(graphSize));

            var output = new[] { graphSize.ToString() }
                .Concat(rows);

            File.WriteAllLines(OutputFileName, output);
        }

        private static string GenerateRow(int graphSize)
        {
            var items = Enumerable.Range(0, graphSize)
                .Select(x => Random.Next(2));

            return string.Join(string.Empty, items);
        }
    }

    internal static class RandomExtensions
    {
        public static int NextNInt(this Random random, int max) => random.Next(1, max + 1);
    }
}
