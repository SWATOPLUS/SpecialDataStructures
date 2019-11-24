using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sds4PersistentSegmentTree
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var inputArgs = Console.ReadLine()
                .Split(' ')
                .Select(int.Parse)
                .ToArray();

            var arraySize = inputArgs[0];
            var queryCount = inputArgs[1];

            var array = Console.ReadLine()
                .Split(' ')
                .Select(int.Parse)
                .ToArray();

            IRangeSegmentTree segmentTree;

            if (args.FirstOrDefault() == "trivial")
            {
                segmentTree = new TrivialRangeSegmentTree(array);
            }
            else
            {
                segmentTree = new TrivialRangeSegmentTree(array);
            }

            var outputBuilder = new StringBuilder();

            foreach (var _ in Enumerable.Range(0, queryCount))
            {
                var commandArgs = Console.ReadLine()
                    .Split(' ')
                    .Select(int.Parse)
                    .ToArray();

                var result = segmentTree.QueryCount(
                    commandArgs[0] - 1, 
                    commandArgs[1], 
                    commandArgs[2],
                    commandArgs[3]);

                outputBuilder.AppendLine(result.ToString());
            }

            Console.WriteLine(outputBuilder.ToString());
        }
    }

    public interface IRangeSegmentTree
    {
        int QueryCount(int startIndex, int endIndex, int from, int to);
    }

    public class TrivialRangeSegmentTree : IRangeSegmentTree
    {
        private readonly int[] _items;

        public TrivialRangeSegmentTree(IEnumerable<int> items)
        {
            _items = items.ToArray();
        }

        public int QueryCount(int startIndex, int endIndex, int from, int to)
        {
            return _items[startIndex..endIndex]
                .Count(x => from <= x && x <= to);
        }
    }
}
