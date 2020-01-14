using System;
using System.Collections.Generic;
using System.Linq;

namespace Sds3_2_EulerChecker
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var items = Console.ReadLine().Split()
                .Select(int.Parse);

            var result = ValidateEulerBypass(items);

            var output = result ? "YES" : "NO";

            Console.WriteLine(output);
        }

        private static bool ValidateEulerBypass(IEnumerable<int> items)
        {
            var usedDict = new Dictionary<int, bool>();
            var stack = new Stack<int>();
            var previous = 0;
            var treeCount = 0;

            foreach (var item in items)
            {
                if (!stack.Any())
                {
                    stack.Push(item);
                    previous = item;
                    usedDict[item] =true;

                    treeCount++;

                    if (treeCount > 1)
                    {
                        return false;
                    }

                    continue;
                }

                if (item == stack.Peek())
                {
                    stack.Pop();
                    previous = item;

                    continue;
                }

                usedDict.TryGetValue(item, out var used);

                if (!used)
                {
                    stack.Push(previous);
                    previous = item;
                    usedDict[item] = true;

                    continue;
                }

                return false;
            }

            return true;
        }
    }
}
