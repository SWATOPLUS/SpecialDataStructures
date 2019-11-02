using System;
using System.Collections.Generic;
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
                distanceCalculator = new TrivialDistanceCalculator();
            }

            var graph = new bool[vertexCount][];

            foreach (var rowIndex in Enumerable.Range(0, vertexCount))
            {
                var row = Console.ReadLine()
                    .Select(x => x == '1')
                    .ToArray();

                graph[rowIndex] = row;
            }

            var result = distanceCalculator.Calculate(graph);

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
                var stack = new Stack<int>();

                var distance = Enumerable.Range(0, size)
                    .Select(x => null as long?)
                    .ToArray();


                distance[startIndex] = 0;
                stack.Push(startIndex);

                while (stack.Any())
                {
                    var currentIndex = stack.Pop();
                    var currentDistance = distance[currentIndex];

                    foreach (var index in Enumerable.Range(0, size))
                    {
                        if (graph[currentIndex][index] && distance[index] == null)
                        {
                            distance[index] = currentDistance + 1;
                            stack.Push(index);
                        }
                    }
                }

                return distance
                    .Select(x => x ?? size)
                    .Select(x => x * x)
                    .Sum();
            }
        }
    }
}
