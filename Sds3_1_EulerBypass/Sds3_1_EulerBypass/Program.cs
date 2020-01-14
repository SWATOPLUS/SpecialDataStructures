using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Sds3_1_EulerBypass
{
    internal class Program
    {
        private static void Main()
        {
            var inputArgs = Console.ReadLine().Split()
                .Select(int.Parse)
                .ToArray();

            var treeSize = inputArgs[0];

            var tree = new Graph(treeSize);

            foreach (var _ in Enumerable.Range(0, treeSize - 1))
            {
                var edgeArgs = Console.ReadLine().Split()
                    .Select(int.Parse)
                    .ToArray();

                var from = edgeArgs[0] - 1;
                var to = edgeArgs[1] - 1;

                tree.AddEdge(from, to);
            }

            var result = tree.Select(x => x + 1);
            var output = string.Join(" ", result);

            Console.WriteLine(output);
        }
    }

    internal class Graph : IEnumerable<int>
    {
        private readonly IReadOnlyList<IList<int>> _neighbors;

        public Graph(int size)
        {
            _neighbors = Enumerable.Range(0, size)
                .Select(x => new List<int>())
                .ToArray();
        }

        public void AddEdge(int from, int to)
        {
            _neighbors[to].Add(from);
            _neighbors[from].Add(to);
        }

        public IEnumerator<int> GetEnumerator()
        {
            var used = new bool[_neighbors.Count];
            var stack = new Stack<(int Vertex, int NeighborIndex)>();
            
            stack.Push((0, 0));
            used[0] = true;

            while (stack.Any())
            {
                var (vertex, neighborIndex) = stack.Pop();

                yield return vertex;

                for (var i = neighborIndex; i < _neighbors[vertex].Count; i++)
                {
                    var neighbor = _neighbors[vertex][i];

                    if (!used[neighbor])
                    {
                        stack.Push((vertex, i + 1));
                        stack.Push((neighbor, 0));
                        used[neighbor] = true;

                        break;
                    }
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
