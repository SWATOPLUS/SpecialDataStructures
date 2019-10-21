using System;
using System.Linq;
using System.Text;

namespace Sds2SegmentSum
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var inputArgs = Console.ReadLine()
                .Split(' ')
                .Select(int.Parse)
                .ToArray();

            var treeSize = inputArgs[0];
            var queryCount = inputArgs[1];

            ISegmentTree tree;

            if (args.FirstOrDefault() == "trivial")
            {
                tree = new TrivialSegmentTree(treeSize);
            }
            else
            {
                tree = new TrivialSegmentTree(treeSize);
            }

            var initialItems = Console.ReadLine()
                .Split(' ')
                .Select(int.Parse)
                .ToArray();
                

            foreach (var index in Enumerable.Range(0, treeSize))
            {
                tree.SetByIndex(index, initialItems[index]);
            }

            var outputBuilder = new StringBuilder();

            foreach (var _ in Enumerable.Range(0, queryCount))
            {
                var query = Console.ReadLine()
                    .Split(' ')
                    .Select(int.Parse)
                    .ToArray();

                var result = null as int?;

                switch (query[0])
                {
                    case 1:
                        tree.SetByIndex(query[1] - 1, query[2]);
                        break;
                    case 2:
                        tree.IncrementByRange(query[1] - 1, query[2] - 1);
                        break;
                    case 3:
                        result = tree.GetEvenSumByRange(query[1] - 1, query[2] - 1);
                        break;
                    case 4:
                        result = tree.GetOddSumByRange(query[1] - 1, query[2] - 1);
                        break;
                    default:
                        throw new InvalidOperationException();
                }

                if (result != null)
                {
                    outputBuilder.AppendLine(result.Value.ToString());
                }
            }

            Console.Write(outputBuilder.ToString());
        }

        private interface ISegmentTree
        {
            void SetByIndex(int index, int value);

            void IncrementByRange(int start, int end);

            int GetEvenSumByRange(int start, int end);

            int GetOddSumByRange(int start, int end);
        }

        private class TrivialSegmentTree : ISegmentTree
        {
            private readonly int[] _items;

            public TrivialSegmentTree(int size)
            {
                _items = new int[size];
            }

            public void SetByIndex(int index, int value)
            {
                _items[index] = value;
            }

            public void IncrementByRange(int start, int end)
            {
                foreach (var index in Enumerable.Range(start, end - start + 1))
                {
                    _items[index]++;
                }
            }

            public int GetEvenSumByRange(int start, int end)
            {
                return Enumerable.Range(start, end - start + 1)
                    .Select(x => _items[x])
                    .Where(x => x % 2 == 0)
                    .Sum();
            }

            public int GetOddSumByRange(int start, int end)
            {
                return Enumerable.Range(start, end - start + 1)
                    .Select(x => _items[x])
                    .Where(x => x % 2 != 0)
                    .Sum();
            }
        }
    }
}
