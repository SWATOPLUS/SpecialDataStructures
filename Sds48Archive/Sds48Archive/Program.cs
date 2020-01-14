using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sds48Archive
{
    internal static class Program
    {
        private const string InputFileName = "archive.in";
        private const string OutputFileName = "archive.out";

        private static void Main()
        {
            var inputLines = File.ReadAllLines(InputFileName);

            var inputArgs = inputLines[0]
                .Split(' ')
                .Select(int.Parse)
                .ToArray();

            var n = inputArgs[0];
            var q = inputArgs[1];

            var array = new DecomposedArray(Enumerable.Range(1, n));

            foreach (var row in Enumerable.Range(1, q))
            {
                var rangeArgs = inputLines[row]
                    .Split(' ')
                    .Select(int.Parse)
                    .ToArray();

                var from = rangeArgs[0] - 1;
                var to = rangeArgs[1] - 1;

                array.PullRange(from, to);
            }

            var output = string.Join(" ", array);

            File.WriteAllText(OutputFileName, output);
        }

        private class DecomposedArray : IEnumerable<int>
        {
            private readonly int _rebuildRate;
            private readonly Segment[] _segments;
            private int _segmentCount;
            private int[] _data;

            public DecomposedArray(IEnumerable<int> data)
            {
                _data = data.ToArray();
                _rebuildRate = (int) Math.Round(Math.Sqrt(_data.Length));
                _segments = new Segment[_rebuildRate + 2];
                _segments[0] = new Segment(0, _data.Length);
                _segmentCount = 1;
            }

            public void PullRange(int from, int to)
            {
                if (_rebuildRate < _segmentCount)
                {
                    RebuildData();
                }

                var fromSegment = CutRange(from);
                var toSegment = CutRange(to + 1);

                ReverseListPart(_segments, 0, fromSegment);
                ReverseListPart(_segments, fromSegment, toSegment);
                ReverseListPart(_segments, 0, toSegment);
            }

            private void ReverseListPart(Segment[] list, int from, int to)
            {
                var length = to - from;

                for (var i = from; i < from + length / 2; i++)
                {
                    var temp = list[i];
                    list[i] = list[to - (i - from) - 1];
                    list[to - (i - from) - 1] = temp;
                }
            }

            private void RebuildData()
            {
                var result = new int[_data.Length];
                var index = 0;

                for (var i = 0; i < _segments.Length; i++)
                {
                    if (i == _segmentCount)
                    {
                        break;
                    }

                    var segment = _segments[i];

                    Array.Copy(_data, segment.Start, result, index, segment.Length);
                    index += segment.Length;
                }

                _data = result;
                _segmentCount = 1;
                _segments[0] = new Segment(0, _data.Length);
            }

            private (int, int) FindCutIndex(int itemIndex)
            {
                var length = 0;

                for (var i = 0; i < _segmentCount; i++)
                {
                    length += _segments[i].Length;

                    if (itemIndex < length)
                    {
                        return (i, _segments[i].Length - (length - itemIndex));
                    }
                }

                return (-1, -1);
            }

            private int CutRange(int itemIndex)
            {
                if (itemIndex == _data.Length)
                {
                    return _segmentCount;
                }

                var (segmentIndex, cutLength) = FindCutIndex(itemIndex);

                if (cutLength == 0)
                {
                    return segmentIndex;
                }

                var segment = _segments[segmentIndex];
                
               
                for (var i = _segmentCount; i > segmentIndex + 1; i--)
                {
                    _segments[i] = _segments[i - 1];
                }

                var (first, last) = segment.Split(cutLength);
                _segments[segmentIndex] = first;
                _segments[segmentIndex + 1] = last;
                _segmentCount++;

                return segmentIndex + 1;
            }

            private struct Segment
            {
                public int Start { get; }

                public int End { get; }

                public int Length { get; }
                
                public Segment(int start, int end)
                {
                    Start = start;
                    End = end;
                    Length = End - Start;
                }
                
                public (Segment first, Segment last) Split(int cutLength)
                {
                    var first = new Segment(Start, Start + cutLength);
                    var last = new Segment(Start + cutLength, End);

                    return (first, last);
                }
            }

            public IEnumerator<int> GetEnumerator()
            {
                if (_segments.Length > 1)
                {
                    RebuildData();
                }

                return (_data as IEnumerable<int>).GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}
