using System;
using System.Collections.Generic;
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

            // var treeSize = inputArgs[0];
            var queryCount = inputArgs[1];

            var initialItems = Console.ReadLine()
                .Split(' ')
                .Select(int.Parse)
                .ToArray();


            ISegmentTree tree;

            if (args.FirstOrDefault() == "trivial")
            {
                tree = new TrivialSegmentTree(initialItems);
            }
            else
            {
                tree = new SegmentTree(initialItems);
            }

            var outputBuilder = new StringBuilder();

            foreach (var _ in Enumerable.Range(0, queryCount))
            {
                var query = Console.ReadLine()
                    .Split(' ')
                    .Select(int.Parse)
                    .ToArray();

                var result = null as long?;

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

            long GetEvenSumByRange(int start, int end);

            long GetOddSumByRange(int start, int end);
        }

        private class SegmentTree : ISegmentTree
        {
            private readonly int _size;
            private readonly Node[] _tree;
            private readonly long[] _increments;

            public SegmentTree(IReadOnlyList<int> items)
            {
                _size = items.Count;
                _tree = new Node[4 * _size];
                _increments = new long[4 * _size];
                BuildTree(items, 1, 0, _size - 1);
            }

            public void SetByIndex(int index, int value)
            {
                Set(index, value, 1, 0, _size - 1);
            }

            public void IncrementByRange(int start, int end)
            {
                Increment(start, end, 1, 0, _size - 1);
            }

            public long GetEvenSumByRange(int start, int end)
            {
                return GetSum(start, end, 1, 0, _size - 1).EvenSum;
            }

            public long GetOddSumByRange(int start, int end)
            {
                return GetSum(start, end, 1, 0, _size - 1).OddSum;
            }

            private void BuildTree(IReadOnlyList<int> items, int root, int left, int right)
            {
                if (left == right)
                {
                    _tree[root] = new Node(items[left]);
                }
                else
                {
                    var middle = (left + right) / 2;
                    BuildTree(items, 2 * root, left, middle);
                    BuildTree(items, 2 * root + 1, middle + 1, right);
                    _tree[root] = _tree[2 * root].SumWith(_tree[2 * root + 1]);
                }
            }

            private void ApplyIncrements(int root, int left, int right)
            {
                var increments = _increments[root];

                if (increments == 0)
                {
                    return;
                }

                if (left != right)
                {
                    _increments[2 * root] += increments;
                    _increments[2 * root + 1] += increments;
                }

                var oldNode = _tree[root];

                var oddSum = oldNode.OddSum + oldNode.OddAmount * increments;
                var evenSum = oldNode.EvenSum + oldNode.EvenAmount * increments;

                if (increments % 2 == 0)
                {
                    _tree[root] = new Node.Builder
                    {
                        OddSum = oddSum,
                        EvenSum = evenSum,
                        OddAmount = oldNode.OddAmount,
                        EvenAmount = oldNode.EvenAmount,
                    }.Build();
                }
                else
                {
                    _tree[root] = new Node.Builder
                    {
                        EvenSum = oddSum,
                        OddSum = evenSum,
                        EvenAmount = oldNode.OddAmount,
                        OddAmount = oldNode.EvenAmount,
                    }.Build();
                }

                _increments[root] = 0;
            }
            
            private Node GetSum(int start, int end, int root, int left, int right)
            {
                ApplyIncrements(root, left, right);

                if (start <= left && right <= end)
                {
                    return _tree[root];
                }

                if (right < start || end < left)
                {
                    return new Node();
                }

                var middle = (left + right) / 2;

                return GetSum(start, end, 2 * root, left, middle)
                    .SumWith(GetSum(start, end, 2 * root + 1, middle + 1, right));
            }

            private void Increment(int start, int end, int root, int left, int right)
            {
                ApplyIncrements(root, left, right);

                if (right < start || end < left)
                {
                    return;
                }

                if (start <= left && right <= end)
                {
                    _increments[root] = 1;
                    ApplyIncrements(root, left, right);

                    return;
                }

                var middle = (left + right) / 2;

                Increment(start, end, 2 * root, left, middle);
                Increment(start, end, 2 * root + 1, middle + 1, right);

                _tree[root] = _tree[2 * root].SumWith(_tree[2 * root + 1]);
            }

            private void Set(int index, int value, int root, int left, int right)
            {
                ApplyIncrements(root, left, right);

                if (index <= left && right <= index)
                {
                    _tree[root] = new Node(value);

                    return;
                }

                if (right < index || index < left)
                {
                    return;
                }

                var middle = (left + right) / 2;

                Set(index, value, 2 * root, left, middle);
                Set(index, value, 2 * root + 1, middle + 1, right);

                _tree[root] = _tree[2 * root].SumWith(_tree[2 * root + 1]);
            }

            private struct Node
            {
                public long EvenAmount { get; }

                public long OddAmount { get; }

                public long EvenSum { get; }

                public long OddSum { get; }
                
                public Node(int value) : this()
                {
                    if (value % 2 == 0)
                    {
                        EvenSum = value;
                        EvenAmount = 1;
                    }
                    else
                    {
                        OddSum = value;
                        OddAmount = 1;
                    }
                }

                public Node(long evenAmount, long oddAmount, long evenSum, long oddSum)
                {
                    EvenAmount = evenAmount;
                    OddAmount = oddAmount;
                    EvenSum = evenSum;
                    OddSum = oddSum;
                }

                public Node SumWith(Node other)
                {
                    return new Builder
                    {
                        EvenAmount = EvenAmount + other.EvenAmount,
                        EvenSum = EvenSum + other.EvenSum,
                        OddAmount = OddAmount + other.OddAmount,
                        OddSum = OddSum + other.OddSum
                    }.Build();
                }

                public struct Builder
                {
                    public long EvenAmount { get; set; }

                    public long OddAmount { get; set; }

                    public long EvenSum { get; set; }

                    public long OddSum { get; set; }

                    public Node Build()
                    {
                        return new Node(EvenAmount, OddAmount, EvenSum, OddSum);
                    }
                }
            }
        }

        private class TrivialSegmentTree : ISegmentTree
        {
            private readonly long[] _items;

            public TrivialSegmentTree(IEnumerable<int> items)
            {
                _items = items
                    .Select(x => (long) x)
                    .ToArray();
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

            public long GetEvenSumByRange(int start, int end)
            {
                return Enumerable.Range(start, end - start + 1)
                    .Select(x => _items[x])
                    .Where(x => x % 2 == 0)
                    .Sum();
            }

            public long GetOddSumByRange(int start, int end)
            {
                return Enumerable.Range(start, end - start + 1)
                    .Select(x => _items[x])
                    .Where(x => x % 2 != 0)
                    .Sum();
            }
        }
    }
}
