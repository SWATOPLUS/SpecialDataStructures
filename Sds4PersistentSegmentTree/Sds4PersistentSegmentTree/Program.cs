using System;
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

            // var arraySize = inputArgs[0];
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
                segmentTree = new RangeSegmentTree(array);
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
                    commandArgs[1] - 1,
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
            return _items.Skip(startIndex).Take(endIndex - startIndex + 1)
                .Count(x => from <= x && x <= to);
        }
    }

    public class RangeSegmentTree : IRangeSegmentTree
    {
        private readonly List<SourceItem> _items;
        private readonly ImmutableCountTree[] _trees;

        public RangeSegmentTree(IEnumerable<int> items)
        {
            _items = new[] {0}.Concat(items)
                .Select((x, i) => new SourceItem(x, i))
                .ToList();

            _items.Sort();

            var size = _items.Count;

            _trees = new ImmutableCountTree[size];

            _trees[0] = new ImmutableCountTree(size - 1);

            foreach (var index in Enumerable.Range(1, size - 1))
            {
                _trees[index] = _trees[index - 1].IncrementIndex(_items[index].Index - 1);
            }
        }

        public int QueryCount(int startIndex, int endIndex, int from, int to)
        {
            var fromItemIndex = FixIndex(_items.BinarySearch(new SourceItem(from - 0.5m)));
            var toItemIndex = FixIndex(_items.BinarySearch(new SourceItem(to + 0.5m)));

            // var fromItemIndex = GetDownIndex(_items, from);
            // var toItemIndex = GetUpperIndex(_items, to);

            var fromCount = _trees[fromItemIndex].GetCount(startIndex, endIndex);
            var toCount = _trees[toItemIndex].GetCount(startIndex, endIndex);

            return toCount - fromCount;
        }

        private static int FixIndex(int index)
        {
            return ~index - 1;
        }

        private static int GetDownIndex(List<SourceItem> items, int value)
        {
            var size = items.Count;

            if (value <= items[0].Value)
            {
                return -1;
            }

            var l = 0;
            var r = size - 1;
            var mid = 0;

            while (l <= r)
            {
                mid = (l + r) / 2;
                if (items[mid].Value >= value)
                    r = mid - 1;
                else
                    l = mid + 1;
            }

            return r;
        }

        private static int GetUpperIndex(List<SourceItem> items, int value)
        {
            var n = items.Count;

            if (value >= items[n - 1].Value)
            {
                return n - 1;
            }

            var l = 0;
            var r = n - 1;
            var mid = 0;

            while (l <= r)
            {
                mid = (l + r) / 2;
                if (items[mid].Value > value)
                    r = mid - 1;
                else
                    l = mid + 1;
            }
            return r;
        }

        private class SourceItem : IComparable<SourceItem>
        {
            public decimal Value { get; }

            public int Index { get; }

            public SourceItem(int value, int index)
            {
                Value = value;
                Index = index;
            }

            public SourceItem(int value)
            {
                Value = value;
                Index = -1;
            }

            public SourceItem(decimal value)
            {
                Value = value;
                Index = -1;
            }

            public int CompareTo(SourceItem other)
            {
                return Value.CompareTo(other.Value);
            }
        }
    }

    public class ImmutableCountTree
    {
        private readonly Node _root;
        private readonly int _size;

        private ImmutableCountTree(int size, Node root)
        {
            _size = size;
            _root = root;
        }

        public ImmutableCountTree(int size) : this(size, BuildEmptyTree(0, size - 1))
        {
        }

        public ImmutableCountTree IncrementIndex(int index)
        {
            var tree = IncrementTree(_root, index, 0, _size - 1);

            return new ImmutableCountTree(_size, tree);
        }

        public int GetCount(int left, int right)
        {
            return GetCount(_root, left, right, 0, _size - 1);
        }
        
        private static int GetCount(Node root, int l, int r, int left, int right)
        {
            if (l <= left && right <= r)
            {
                return root.Count;
            }

            if (right < l || r < left)
            {
                return 0;
            }

            var mid = (left + right) / 2;


		return GetCount(root.Left, l, r, left, mid) +
               GetCount(root.Right, l, r, mid + 1, right);
            
        }

        private static Node BuildEmptyTree(int left, int right)
        {
            if (left == right)
            {
                return new Node(0);
            }

            var mid = (left + right) / 2;

            var leftNode = BuildEmptyTree(left, mid);
            var rightNode = BuildEmptyTree(mid + 1, right);

            return new Node(leftNode, rightNode);
        }

        private static Node IncrementTree(Node root, int index, int left, int right)
        {
            if (left == right)
            {
                return new Node(root.Count + 1);
            }

            var mid = (left + right) / 2;

            if (index <= mid)
            {
                return root.ReplaceLeft(IncrementTree(root.Left, index, left, mid));
            }

            return root.ReplaceRight(IncrementTree(root.Right, index, mid + 1, right));
        }

        private class Node
        {
            public int Count { get; }

            public Node Left { get; }

            public Node Right { get; }

            public Node(int count)
            {
                Count = count;
            }

            public Node(Node left, Node right)
            {
                Left = left;
                Right = right;
                Count = left.Count + right.Count;
            }

            public Node ReplaceLeft(Node left)
            {
                return new Node(left, Right);
            }

            public Node ReplaceRight(Node right)
            {
                return new Node(Left, right);
            }
        }
    }
}
