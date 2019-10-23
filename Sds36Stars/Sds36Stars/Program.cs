using System.IO;
using System.Linq;

namespace Sds36Stars
{
    internal class Program
    {
        private const string InputFileName = "input.txt";
        private const string OutputFileName = "output.txt";


        private static void Main(string[] args)
        {
            var input = File.ReadAllLines(InputFileName);
            var size = int.Parse(input[0]);
            var tree = new SegmentTree(500000 + 1);
            var ranks = new int[size];

            foreach (var line in input.Skip(1))
            {
                var coords = line.Split().Select(int.Parse).ToArray();
                var (x, y) = (coords[0], coords[1]);

                var rank = tree.SumRange(0, x + 1);

                ranks[rank]++;

                var count = tree.Get(x);

                tree.Update(x, count + 1);
            }

            var output = string.Join("\n", ranks);

            File.WriteAllText(OutputFileName, output);
        }

        private class SegmentTree
        {
            private readonly int[] _tree;
            private readonly int _size;

            public SegmentTree(int n)
            {
                _tree = new int[2 * n];
                _size = n;
            }

            public void Update(int index, int value)
            {
                _tree[index + _size] = value;

                index += _size;

                for (var i = index; i > 1; i /= 2)
                {
                    _tree[i / 2] = _tree[i] + _tree[i ^ 1];
                }
            }

            public int Get(int index)
            {
                return _tree[index + _size];
            }

            // [l+, r-]
            public int SumRange(int l, int r)
            {
                var sum = 0;

                l += _size;
                r += _size;

                while (l < r)
                {
                    if ((l & 1) > 0)
                    {
                        sum += _tree[l];

                        l++;
                    }

                    if ((r & 1) > 0)
                    {
                        r--;

                        sum += _tree[r];
                    }

                    l /= 2;
                    r /= 2;
                }

                return sum;
            }
        }
    }
}
