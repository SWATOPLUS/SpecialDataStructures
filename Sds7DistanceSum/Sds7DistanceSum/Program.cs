using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Sds7DistanceSum
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var inputArgs = Console.ReadLine()
                .Split(' ')
                .Select(int.Parse)
                .ToArray();

            var vertexCount = inputArgs[0];

            IDistanceCalculator distanceCalculator;

            if (args.FirstOrDefault() == "trivial")
            {
                distanceCalculator = new TrivialDistanceCalculator();
            }
            else
            {
                distanceCalculator = new BinaryDistanceCalculator();
            }

            var graph = new bool[vertexCount][];

            foreach (var rowIndex in Enumerable.Range(0, vertexCount))
            {
                var row = Console.ReadLine()
                    .Select(x => x == '1')
                    .ToArray();

                graph[rowIndex] = row;
            }

            var sw = new Stopwatch();
            sw.Start();

            var result = distanceCalculator.Calculate(graph);

            sw.Stop();

            var outputBuilder = new StringBuilder();

            outputBuilder.AppendLine(result.ToString());

            Console.Write(outputBuilder);
        }

        private interface IDistanceCalculator
        {
            long Calculate(bool[][] graph);
        }

        private class TrivialDistanceCalculator : IDistanceCalculator
        {
            public long Calculate(bool[][] graph)
            {
                var size = graph.Length;

                return Enumerable.Range(0, size)
                    .Select(x => GetDistanceSquareSum(graph, x))
                    .Sum();
            }

            private static long GetDistanceSquareSum(bool[][] graph, int startIndex)
            {
                var size = graph.Length;
                var queue = new Queue<int>();
                var distance = new long?[size];

                distance[startIndex] = 0;
                queue.Enqueue(startIndex);

                while (queue.Any())
                {
                    var currentIndex = queue.Dequeue();
                    var currentDistance = distance[currentIndex];

                    var items = Enumerable.Range(0, size)
                        .Where(x => graph[currentIndex][x])
                        .Where(x => distance[x] == null);

                    foreach (var index in items)
                    {
                        distance[index] = currentDistance + 1;
                        queue.Enqueue(index);
                    }
                }

                return distance
                    .Select(x => x ?? size)
                    .Select(x => x * x)
                    .Sum();
            }
        }

        private class BinaryDistanceCalculator : IDistanceCalculator
        {
            public long Calculate(bool[][] graph)
            {
                var size = graph.Length;
                var bitGraph = new BitSet2048[size];
                var init = Enumerable.Range(0, size)
                    .Select(x => true)
                    .ToArray();

                for (var index = 0; index < size; index++)
                {
                    bitGraph[index] = new BitSet2048(graph[index]);
                }

                return Enumerable.Range(0, size)
                    .Select(x => GetDistanceSquareSum(bitGraph, init, x))
                    .Sum();
            }

            private static long GetDistanceSquareSum(BitSet2048[] graph, bool[] init, int startIndex)
            {
                var size = graph.Length;
                var queue = new Queue<int>();
                var distance = new long?[size];

                var notUsed = new BitSet2048(init);
                var work = BitSet2048.CreateClear();

                distance[startIndex] = 0;
                queue.Enqueue(startIndex);
                notUsed[startIndex] = false;

                while (queue.Any())
                {
                    var currentIndex = queue.Dequeue();
                    var currentDistance = distance[currentIndex];
                    var currentRow = graph[currentIndex];

                    for (var index = 0; index < work.Clusters.Length; index++)
                    {
                        work.Clusters[index] = notUsed.Clusters[index] & currentRow.Clusters[index];
                    }

                    var items = work.GetItems();

                    foreach (var index in items)
                    {
                        distance[index] = currentDistance + 1;
                        notUsed[index] = false;
                        queue.Enqueue(index);
                    }
                }

                return distance
                    .Select(x => x ?? size)
                    .Select(x => x * x)
                    .Sum();
            }
        }
    }

    internal struct BitSet2048
    {
        public static BitSet2048 CreateClear()
        {
            return new BitSet2048
            {
                Clusters = new ulong[_clusterCount]
            };
        }

        private const int _size = 2048;
        private const int _bitsInByte = 8;
        private const int _bitsInCluster = sizeof(ulong) * _bitsInByte;
        private const int _clusterCount = _size / _bitsInCluster;

        public ulong[] Clusters;

        public BitSet2048(bool[] flags)
        {
            if (flags.Length > _size)
            {
                throw new ArgumentException($"flags.Length > {_size}");
            }

            Clusters = new ulong[_clusterCount];

            for (var index = 0; index < flags.Length; index++)
            {
                if (flags[index])
                {
                    var clusterIndex = index / _bitsInCluster;
                    var offsetIndex = index % _bitsInCluster;
                    var mask = 1UL << offsetIndex;

                    Clusters[clusterIndex] |= mask;
                }
            }
        }

        public bool this[int index]
        {
            get
            {
                var clusterIndex = index / _bitsInCluster;
                var offsetIndex = index % _bitsInCluster;
                var mask = 1UL << offsetIndex;

                return (Clusters[clusterIndex] & mask) != 0UL;
            }

            set
            {
                var clusterIndex = index / _bitsInCluster;
                var offsetIndex = index % _bitsInCluster;
                var mask = 1UL << offsetIndex;

                if (value)
                {
                    Clusters[clusterIndex] |= mask;
                }
                else
                {
                    Clusters[clusterIndex] &= ~mask;
                }
            }
        }

        public IEnumerable<int> GetItems()
        {
            for (var clusterIndex = 0; clusterIndex < Clusters.Length; clusterIndex++)
            {
                var cluster = Clusters[clusterIndex];

                if (cluster == 0)
                {
                    continue;
                }

                var offset = 0;
                var mask = 1UL;

                while (cluster != 0)
                {
                    if ((mask & cluster) != 0)
                    {
                        cluster &= ~mask;

                        yield return clusterIndex * _bitsInCluster + offset;
                    }

                    offset++;
                    mask <<= 1;
                }
            }
        }
    }
}
