using System;
using System.Collections.Generic;
using System.Linq;

namespace Sds9MinList
{
    class Program
    {
        private static void Main()
        {
            var list = new MyList<int>();

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

        private class MyList<T> where T : struct
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
