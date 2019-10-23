using System.IO;
using System.Linq;

namespace Sds45MegaInversions
{
    internal static class Program
    {
        private const string InputFileName = "input.txt";
        private const string OutputFileName = "output.txt";

        private static void Main()
        {
            var input = File.ReadAllLines(InputFileName);

            var size = int.Parse(input[0]);

            var orderTree = new FenwickTree(size);
            var inversionsTree = new FenwickTree(size);

            var result = 0L;

            foreach (var order in input.Skip(1).Select(int.Parse))
            {
                orderTree.Add(order, 1);

                var inversions = orderTree.Query(size) - orderTree.Query(order);

                inversionsTree.Add(order, inversions);

                result += inversionsTree.Query(size) - inversionsTree.Query(order);
            }

            File.WriteAllText(OutputFileName, result.ToString());
        }

        private class FenwickTree
        {
            private readonly long[] _tree;

            public FenwickTree(int n)
            {
                _tree = new long[RoundUpPow2(2 * n) + 1];
            }

            public void Add(int k, long x)
            {
                if (k == 0)
                {
                    return;
                }

                while (k < _tree.Length)
                {
                    _tree[k] += x;

                    k += LowBit(k);
                }
            }

            public long Query(int k)
            {
                var sum = 0L;

                while (k > 0)
                {
                    sum += _tree[k];

                    k -= LowBit(k);
                }

                return sum;
            }

            private static int RoundUpPow2(int n)
            {
                var up = 1;

                while (up < n)
                {
                    up *= 2;
                }

                return up;
            }

            private static int LowBit(int n)
            {
                return n & -n;
            }
        }
    }
}
