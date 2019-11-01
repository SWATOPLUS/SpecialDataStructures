using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sds1Bag
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var inputArgs = Console.ReadLine()
                .Split(' ')
                .Select(int.Parse)
                .ToArray();

            var itemCount = inputArgs[0];
            var bagSize = inputArgs[1];

            IBagFiller bagFiller;

            if (args.FirstOrDefault() == "trivial")
            {
                bagFiller = new TrivialBagFiller();
            }
            else
            {
                bagFiller = new TrivialBagFiller();
            }

            var items = new BagItem[itemCount];

            foreach (var index in Enumerable.Range(0, itemCount))
            {
                var item = Console.ReadLine()
                    .Split(' ')
                    .Select(int.Parse)
                    .ToArray();

                items[index] = new BagItem(item[0], item[1]);
            }

            var result = bagFiller.FillBag(items, bagSize);

            var outputBuilder = new StringBuilder();

            outputBuilder.AppendLine(result.Length.ToString());
            outputBuilder.AppendLine(string.Join(" ", result.Select(x => x + 1)));

            Console.Write(outputBuilder);
        }

        private interface IBagFiller
        {
            int[] FillBag(IReadOnlyList<BagItem> items, int bagSize);
        }

        private class TrivialBagFiller : IBagFiller
        {
            public int[] FillBag(IReadOnlyList<BagItem> items, int bagSize)
            {
                var flags = new bool[items.Count];

                var best = Array.Empty<int>();
                var bestCost = 0L;

                while (IncrementFlags(flags))
                {
                    var subset = flags
                        .Select((flag, index) => (flag, index))
                        .Where(x => x.flag)
                        .Select(x => (Index: x.index, Item: items[x.index]))
                        .ToArray();

                    var cost = subset.Sum(x => x.Item.Cost);
                    var size = subset.Sum(x => x.Item.Size);

                    if (bagSize < size)
                    {
                        continue;
                    }

                    if (cost <= bestCost)
                    {
                        continue;
                    }

                    bestCost = cost;
                    best = subset
                        .Select(x => x.Index)
                        .ToArray();
                }

                return best;
            }

            private static bool IncrementFlags(IList<bool> flags)
            {
                var index = 0;

                while (index < flags.Count)
                {
                    if (!flags[index])
                    {
                        flags[index] = true;

                        return true;
                    }

                    flags[index] = false;
                    index++;
                }

                return false;
            }
        }

        private class BagItem
        {
            public long Size { get; }

            public long Cost { get; }

            public BagItem(long size, long cost)
            {
                Size = size;
                Cost = cost;
            }
        }
    }
}
