using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sds10KingKonReception
{
    internal class Program
    {
        private const int TimeSize = 1_000_005;

        private static void Main(string[] args)
        {
            var inputArgs = Console.ReadLine()
                .Split(' ')
                .Select(int.Parse)
                .ToArray();

            IKingKonReception reception;

            var outputBuilder = new StringBuilder();

            var queryCount = inputArgs[0];

            if (args.FirstOrDefault() == "trivial")
            {
                reception = new TrivialKingKonReception();
            }
            else
            {
                reception = new KingKonReception(TimeSize);
            }

            var joinTime = new int[queryCount];

            foreach (var index in Enumerable.Range(0, queryCount))
            {
                var commandParts = Console.ReadLine()
                    .Split(' ');

                var command = commandParts.First();

                var commandArgs = commandParts.Skip(1)
                    .Select(int.Parse)
                    .ToArray();

                switch (command)
                {
                    case "+":
                        joinTime[index] = commandArgs[0];
                        reception.Join(commandArgs[0], commandArgs[1]);
                        break;
                    case "-":
                        reception.Cancel(joinTime[commandArgs[0] - 1]);
                        break;
                    case "?":
                        var result = reception.Query(commandArgs[0]);
                        outputBuilder.AppendLine(result.ToString());
                        break;
                    default:
                        throw new InvalidOperationException();
                }
            }

            Console.Write(outputBuilder.ToString());
        }

        private interface IKingKonReception
        {
            void Join(int time, int duration);

            void Cancel(int time);

            long Query(int time);
        }

        private class TrivialKingKonReception : IKingKonReception
        {
            private readonly Dictionary<int, int> _timeDurationDictionary = new Dictionary<int, int>();
 
            public void Join(int time, int duration)
            {
                _timeDurationDictionary.Add(time, duration);
            }

            public void Cancel(int time)
            {
                _timeDurationDictionary.Remove(time);
            }

            public long Query(int time)
            {
                var result = QueryAbsolute(time);

                return Math.Max(0, result - time);
            }

            private long QueryAbsolute(int queryTime)
            {
                var timeStamp = 0L;

                foreach (var join in _timeDurationDictionary.OrderBy(x => x.Key))
                {
                    var time = join.Key;
                    var duration = join.Value;

                    if (queryTime < time)
                    {
                        return timeStamp;
                    }

                    if (timeStamp < time)
                    {
                        timeStamp = time;
                    }

                    timeStamp += duration;
                }

                return timeStamp;
            }
        }

        private class KingKonReception : IKingKonReception
        {
            private readonly Node[] _tree;
            private readonly int[] _durations;

            public KingKonReception(int size)
            {
                _tree = new Node[(size + 1) * 4];
                _durations = new int[size + 1];

                GrowTree(1, 1, size);
            }

            public void Join(int time, int duration)
            {
                _durations[time] = duration;

                ChangeValue(1, time, duration);
            }

            public void Cancel(int time)
            {
                var duration = _durations[time];

                ChangeValue(1, time, -duration);
            }

            public long Query(int time)
            {
                _sum = 0;
                GetSum(1, time);

                return Math.Max(_sum - time, 0);
            }
            
            private long _sum;

            private void GetSum(int index, int queryTime)
            {
                while (true)
                {
                    if (_tree[index].Right <= queryTime)
                    {
                        _sum = Math.Max(_tree[index].Max, _sum + _tree[index].Sum);
                        return;
                    }

                    var mid = _tree[index].Mid;

                    GetSum(LeftChildIndex(index), queryTime);

                    if (mid >= queryTime)
                    {
                        break;
                    }

                    index = RightChildIndex(index);
                }
            }

            private void ChangeValue(int index, int position, int diff)
            {
                if (_tree[index].IsLeaf())
                {
                    _tree[index].Sum += diff;
                    _tree[index].Max += diff;

                    return;
                }

                var mid = _tree[index].Mid;

                if (position <= mid)
                {
                    ChangeValue(LeftChildIndex(index), position, diff);
                }

                if (position > mid)
                {
                    ChangeValue(RightChildIndex(index), position, diff);
                }

                UpdateChildren(index);
            }

            private void GrowTree(int index, int left, int right)
            {
                _tree[index].Left = left;
                _tree[index].Right = right;

                if (left == right)
                {
                    _tree[index].Sum = 0;
                    _tree[index].Max = left;
                    return;
                }

                var mid = _tree[index].Mid;

                GrowTree(LeftChildIndex(index), left, mid);
                GrowTree(RightChildIndex(index), mid + 1, right);

                UpdateChildren(index);
            }

            private void UpdateChildren(int index)
            {
                var left = _tree[LeftChildIndex(index)];
                var right = _tree[RightChildIndex(index)];

                _tree[index].Sum = left.Sum + right.Sum;
                _tree[index].Max = Math.Max(right.Max, left.Max + right.Sum);
            }

            private static int LeftChildIndex(int index) => index * 2;

            private static int RightChildIndex(int index) => index * 2 + 1;
            
            private struct Node
            {
                public int Left { get; set; }

                public int Right { get; set; }

                public long Sum { get; set; }

                public long Max { get; set; }

                public bool IsLeaf() => Left == Right;

                public int Mid => (Left + Right) / 2;
            }
        }
    }
}
