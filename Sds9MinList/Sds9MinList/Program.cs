using System;
using System.Collections.Generic;
using System.Linq;

namespace Sds9MinList
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            IMinList<int> list;

            if (args.FirstOrDefault() == "linked")
            {
                list = new LinkedMinList<int>();
            }
            else
            {
                list = new StackMinList<int>();
            }

            var commandCountLine = Console.ReadLine() ?? throw new InvalidOperationException();
            var commandCount = int.Parse(commandCountLine.Trim());

            foreach (var _ in Enumerable.Range(0, commandCount))
            {
                var command = Console.ReadLine() ?? throw new InvalidOperationException();
                var commandParts = command.Split(' ');
                var op = commandParts[0];

                switch (op)
                {
                    case "+L":
                        list.PushFront(int.Parse(commandParts[1]));
                        break;
                    case "+R":
                        list.PushBack(int.Parse(commandParts[1]));
                        break;
                    case "-L":
                        list.PopFront();
                        break;
                    case "-R":
                        list.PopBack();
                        break;
                    default:
                        throw new InvalidOperationException();
                }

                Console.WriteLine(list.GetMin() ?? -1);
            }
        }

        private interface IMinList<T> where T : struct, IEquatable<T>
        {
            void PushFront(T x);
            void PushBack(T x);
            void PopFront();
            void PopBack();
            T? GetMin();
        }

        private class StackMinList<T> : IMinList<T> where T : struct, IEquatable<T>
        {
            private readonly Stack<(T Item, T Min)> _frontStack = new Stack<(T Item, T Min)>();
            private readonly Stack<(T Item, T Min)> _backStack = new Stack<(T Item, T Min)>();

            public void PushFront(T x)
            {
                var items = new List<T>();

                if (_frontStack.Any())
                {
                    items.Add(_frontStack.Peek().Min);
                }

                items.Add(x);

                _frontStack.Push((x, items.Min()));
            }

            public void PushBack(T x)
            {
                var items = new List<T>();

                if (_backStack.Any())
                {
                    items.Add(_backStack.Peek().Min);
                }

                items.Add(x);

                _backStack.Push((x, items.Min()));
            }

            public void PopFront()
            {
                if (_frontStack.Any())
                {
                    _frontStack.Pop();
                }
                else
                {
                    CutAndRebase(true);
                }
            }

            public void PopBack()
            {
                if (_backStack.Any())
                {
                    _backStack.Pop();
                }
                else
                {
                    CutAndRebase(false);
                }
            }

            private void CutAndRebase(bool isFront)
            {
                var items = _frontStack.Concat(_backStack.Reverse()).ToArray();

                var cutItems = isFront
                    ? items.Skip(1).Select(x => x.Item).ToArray()
                    : items.Take(items.Length - 1).Select(x => x.Item).ToArray();

                var frontItemsCount = cutItems.Length / 2;

                _frontStack.Clear();
                _backStack.Clear();

                foreach (var item in cutItems.Take(frontItemsCount))
                {
                    PushFront(item);
                }

                foreach (var item in cutItems.Skip(frontItemsCount).Reverse())
                {
                    PushBack(item);
                }
            }

            public T? GetMin()
            {
                var items = new List<T>();

                if (_frontStack.Any())
                {
                    items.Add(_frontStack.Peek().Min);
                }

                if (_backStack.Any())
                {
                    items.Add(_backStack.Peek().Min);
                }

                return items.Count > 0
                    ? items.Min()
                    : null as T?;
            }
        }

        private class LinkedMinList<T> : IMinList<T> where T : struct, IEquatable<T>
        {
            private readonly LinkedList<T> _list = new LinkedList<T>();

            public void PushFront(T x)
            {
                _list.AddFirst(x);
            }

            public void PushBack(T x)
            {
                _list.AddLast(x);
            }

            public void PopFront()
            {
                _list.RemoveFirst();
            }

            public void PopBack()
            {
                _list.RemoveLast();
            }

            public T? GetMin()
            {
                return _list.Count > 0
                    ? _list.Min()
                    : null as T?;
            }
        }
    }
}
