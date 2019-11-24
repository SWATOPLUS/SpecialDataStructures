using System;
using System.Collections;
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
                bagFiller = new BinaryBagFiller();
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

        private class BinaryBagFiller : IBagFiller
        {
            public class BagItemSet : IComparable<BagItemSet>, IEnumerable<int>
            {
                public BitArray FirstPart;
                public BitArray SecondPart;
                public long Size;
                public long Cost;
                public int HigherIndex;

                public BagItemSet(BitArray mask, long size, long cost, int higherIndex)
                {
                    FirstPart = new BitArray(mask);
                    SecondPart = new BitArray(mask.Length);
                    Size = size;
                    Cost = cost;
                    HigherIndex = higherIndex;
                }

                public BagItemSet(int count, long size, long cost, int higherIndex)
                {
                    FirstPart = new BitArray(count);
                    SecondPart = new BitArray(count);
                    Size = size;
                    Cost = cost;
                    HigherIndex = higherIndex;
                }

                public int CompareTo(BagItemSet other)
                {
                    return Size.CompareTo(other.Size);
                }

                public IEnumerator<int> GetEnumerator()
                {
                    var count = FirstPart.Count;
                    var firstSetCount = count / 2;
                    var secondSetCount = count - firstSetCount;

                    for (var i = 0; i < firstSetCount; i++)
                    {
                        if (FirstPart[i])
                        {
                            yield return i;
                        }
                    }

                    for (var j = 0; j < secondSetCount; j++)
                    {
                        if (SecondPart[j])
                        {
                            yield return j + firstSetCount;
                        }
                    }
                }

                IEnumerator IEnumerable.GetEnumerator()
                {
                    return GetEnumerator();
                }
            }

            public int[] FillBag(IReadOnlyList<BagItem> items, int bagSize)
            {
                var firstPartCount = items.Count / 2;
                var secondPartCount = items.Count - firstPartCount;

                var currentCost = 0L;
                var currentSize = 0L;

                var firstMask = new BitArray(items.Count);
                var secondMask = new BitArray(items.Count);

                var possibleSets = new List<BagItemSet>();
                var bestSet = new BagItemSet(items.Count, 0L, 0L, -1);

                for (var setMask = 0; setMask < Math.Pow(2, firstPartCount); setMask++)
                {
                    var index = GetLowestBit(setMask);

                    if (setMask == 0)
                    {
                        possibleSets.Add(new BagItemSet(firstMask, currentSize, currentCost, -1));
                        continue;
                    }

                    firstMask[index] = !firstMask[index];

                    if (firstMask[index])
                    {
                        currentCost += items[index].Cost;
                        currentSize += items[index].Size;
                    }
                    else
                    {
                        currentCost -= items[index].Cost;
                        currentSize -= items[index].Size;
                    }

                    possibleSets.Add(new BagItemSet(firstMask, currentSize, currentCost, -1));
                }

                possibleSets.Sort();

                var maxCurrentIndex = 0;
                var maxCurrentPrice = 0L;

                for (var j = 0; j < possibleSets.Count; j++)
                {
                    if (possibleSets[j].Cost > maxCurrentPrice)
                    {
                        maxCurrentPrice = possibleSets[j].Cost;
                        maxCurrentIndex = j;
                    }
                    else if (possibleSets[j].Cost < maxCurrentPrice)
                    {
                        possibleSets[j].HigherIndex = maxCurrentIndex;
                    }

                }

                currentCost = 0L;
                currentSize = 0L;

                for (var setMask = 0; setMask < Math.Pow(2, secondPartCount); setMask++)
                {
                    var lowestIndex = GetLowestBit(setMask);

                    if (setMask == 0)
                    {
                        var maxIndex = GetMaxWeightIndex(possibleSets, currentSize, bagSize);

                        if (possibleSets[maxIndex].HigherIndex >= 0)
                        {
                            maxIndex = possibleSets[maxIndex].HigherIndex;
                        }

                        if (possibleSets[maxIndex].Size <= bagSize - currentSize &&
                            possibleSets[maxIndex].Cost + currentCost > bestSet.Cost)
                        {
                            bestSet.Cost = possibleSets[maxIndex].Cost + currentCost;
                            bestSet.Size = possibleSets[maxIndex].Size + currentSize;
                            bestSet.FirstPart = new BitArray(possibleSets[maxIndex].FirstPart);
                            bestSet.SecondPart = new BitArray(secondMask);
                        }

                        continue;
                    }

                    secondMask[lowestIndex] = !secondMask[lowestIndex];

                    if (secondMask[lowestIndex])
                    {
                        currentCost += items[lowestIndex + firstPartCount].Cost;
                        currentSize += items[lowestIndex + firstPartCount].Size;
                    }
                    else
                    {
                        currentCost -= items[lowestIndex + firstPartCount].Cost;
                        currentSize -= items[lowestIndex + firstPartCount].Size;
                    }
                    
                    var maxWeightIndex = GetMaxWeightIndex(possibleSets, currentSize, bagSize);

                    if (maxWeightIndex < 0)
                    {
                        continue;
                    }

                    if (possibleSets[maxWeightIndex].HigherIndex >= 0)
                    {
                        maxWeightIndex = possibleSets[maxWeightIndex].HigherIndex;
                    }

                    if (possibleSets[maxWeightIndex].Size <= bagSize - currentSize &&
                        possibleSets[maxWeightIndex].Cost + currentCost > bestSet.Cost)
                    {
                        bestSet.Cost = possibleSets[maxWeightIndex].Cost + currentCost;
                        bestSet.Size = possibleSets[maxWeightIndex].Size + currentSize;
                        bestSet.FirstPart = new BitArray(possibleSets[maxWeightIndex].FirstPart);
                        bestSet.SecondPart = new BitArray((secondMask));
                    }
                }

                return bestSet.ToArray();
            }

            private static int GetMaxWeightIndex(IReadOnlyList<BagItemSet> itemSets, long currentSize, long bagSize)
            {
                if (currentSize > bagSize)
                {
                    return -1;
                }

                var left = 0;
                var right = itemSets.Count - 1;

                while (left <= right)
                {
                    var middle = (left + right) / 2;

                    if (itemSets[middle].Size > bagSize - currentSize)
                    {
                        right = middle - 1;
                    }
                    else
                    {
                        left = middle + 1;
                    }
                }

                return right;
            }

            private static int GetLowestBit(int cluster)
            {
                if (cluster == 0)
                {
                    return 0;
                }

                var mask = 1;
                var index = 0;

                while ((mask & cluster) == 0)
                {
                    mask <<= 1;
                    index++;
                }

                return index;
            }
        }

        private struct BagItem
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
