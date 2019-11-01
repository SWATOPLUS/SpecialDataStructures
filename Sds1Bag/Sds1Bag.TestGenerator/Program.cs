using System;
using System.IO;
using System.Linq;

namespace Sds1Bag.TestGenerator
{
    internal class Program
    {
        private const string OutputFileName = "input.txt";
        private static int MaxItemCount = 40;
        private static int MaxItemSize = 1_000_000_000;
        private static int MaxBagSize = 1_000_000_000;
        private static int MaxItemCost = 1_000_000_000;

        private static readonly Random Random = new Random(42);

        private static void Main(string[] args)
        {
            if (args.Length == 1)
            {
                MaxItemCount = int.Parse(args[0]);
            }

            if (args.Length == 2)
            {
                MaxItemCount = int.Parse(args[0]);
                MaxItemSize = int.Parse(args[1]);
            }

            if (args.Length == 3)
            {
                MaxItemCount = int.Parse(args[0]);
                MaxItemSize = int.Parse(args[1]);
                MaxBagSize = int.Parse(args[2]);
            }

            var itemCount = Random.NextNInt(MaxItemCount);
            var bagSize = Random.NextNInt(MaxBagSize);
            var items = Enumerable.Range(0, itemCount)
                .Select(x => GenerateItem());

            var output = new[] {$"{itemCount} {bagSize}"}
                .Concat(items);

            File.WriteAllLines(OutputFileName, output);
        }

        private static string GenerateItem()
        {
            return Random.NextNInt(MaxItemSize) + " " + Random.NextNInt(MaxItemCost);
        }
    }

    internal static class RandomExtensions
    {
        public static int NextNInt(this Random random, int max) => random.Next(1, max + 1);
    }
}
