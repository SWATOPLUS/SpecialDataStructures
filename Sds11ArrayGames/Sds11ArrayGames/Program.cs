using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Sds11ArrayGames
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

            var items = inputLines[1].Split()
                .Select(int.Parse)
                .ToArray();
                
            var pa = new PlayWithArray(items);

            var outputBuilder = new StringBuilder();

            foreach (var commandIndex in Enumerable.Range(0, q))
            {
                var command = inputLines[commandIndex + 2];
                var operation = command[0];
                var commandsArgs = command.Split()
                    .Skip(1)
                    .Select(int.Parse)
                    .ToArray();

                switch (operation)
                {
                    case 'Q':
                        var count = pa.GetEventCount(commandsArgs[0] - 1, commandsArgs[1] - 2);
                        outputBuilder.AppendLine(count.ToString());
                        break;
                    case 'S':
                        pa.Update(1, 0, n - 1, op.Item2 - 1, op.Item3);
                        break;
                    default:
                        throw new InvalidOperationException();
                }
            }

            File.WriteAllText(OutputFileName, outputBuilder.ToString());
        }
    }

    public class PlayWithArray
    {
        private const int ItemSize = 1024;
        private BitArray[] arrays;
        private int[] leaves;
        private readonly int _size;

        public PlayWithArray(IReadOnlyList<int> items)
        {
            _size = items.Count;

            arrays = new BitArray[_size * 4];
            leaves = new int[_size * 4];

            Build(items, 1, 0, _size - 1);
        }

        private void Build(IReadOnlyList<int> arr, int v, int l, int r)
        {
            arrays[v] = new BitArray(ItemSize);
            
            if (l == r)
            {
                arrays[v][arr[l]] = true;
                leaves[v] = arr[l];
            }
            else
            {
                int mid = (r - l) / 2 + l;
                Build(arr, v * 2, l, mid);
                Build(arr, v * 2 + 1, mid + 1, r);
                Update(v);
            }
        }

        private void Update(int v)
        {
            arrays[v] = MakeXOR(arrays[v * 2], arrays[v * 2 + 1]);
        }

        private static BitArray MakeXOR(BitArray b1, BitArray b2)
        {
            return (new BitArray(b1)).Xor(b2);
        }

        public void Update(int v, int tl, int tr, int pos, int new_val)
        {
            if (tl == tr)
            {
                arrays[v][leaves[v]] = false;
                arrays[v][new_val] = true;
                leaves[v] = new_val;
            }
            else
            {
                int mid = (tr - tl) / 2 + tl;
                if (pos <= mid)
                    Update(v * 2, tl, mid, pos, new_val);
                else
                    Update(v * 2 + 1, mid + 1, tr, pos, new_val);
                Update(v);
            }
        }

        public int GetEventCount(int from, int to)
        {
            return GetEvenCount(1, 0, _size - 1, from, to);
        }

        private int GetEvenCount(int v, int l, int r, int tl, int tr)
        {
            var ba = GetEvenCountInternal(v, l, r, tl, tr);

            return GetCardinality(ba);
        }

        private BitArray GetEvenCountInternal(int v, int l, int r, int tl, int tr)
        {
            if (l > tr || r < tl)
            {
                return new BitArray(ItemSize, false);
            }
            if (tl <= l && r <= tr)
            {
                return arrays[v];
            }
            int mid = (r - l) / 2 + l;
            return MakeXOR(GetEvenCountInternal(v * 2, l, mid, tl, tr), GetEvenCountInternal(v * 2 + 1, mid + 1, r, tl, tr));
        }

        private int GetCardinality(BitArray bitArray)
        {
            int[] ints = new int[(bitArray.Count >> 5) + 1];
            bitArray.CopyTo(ints, 0);
            int count = 0;

            ints[ints.Length - 1] &= ~(-1 << (bitArray.Count % 32));

            for (int i = 0; i < ints.Length; i++)
            {
                int c = ints[i];
                unchecked
                {
                    c = c - ((c >> 1) & 0x55555555);
                    c = (c & 0x33333333) + ((c >> 2) & 0x33333333);
                    c = ((c + (c >> 4) & 0xF0F0F0F) * 0x1010101) >> 24;
                }

                count += c;
            }
            return count;
        }

    }
}
