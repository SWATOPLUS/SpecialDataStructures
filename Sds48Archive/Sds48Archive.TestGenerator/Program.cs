using System;
using System.IO;
using System.Linq;

namespace Sds48Archive.TestGenerator
{
    internal static class Program
    {
        private const string OutputFileName = "archive.in";
        private static int ArraySize = 30_000;
        private static int QueryCount = 30_000;
        private static readonly Random Random = new Random(42);

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

            var items = Enumerable.Range(0, QueryCount)
                .Select(x =>
                {
                    var from = Random.NextNInt(ArraySize);
                    var to = Random.NextNInt(from, ArraySize);

                    return $"{from} {to}";
                });

            var lines = new[]
            {
                $"{ArraySize} {QueryCount}",
            }.Concat(items);

            File.WriteAllLines(OutputFileName, lines);
        }
    }

    internal static class RandomExtensions
    {
        public static int NextNInt(this Random random, int max) => random.Next(1, max + 1);

        public static int NextNInt(this Random random, int min, int max) => random.Next(min, max + 1);
    }
}
