using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sds10KingKonReception
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var inputArgs = Console.ReadLine()
                .Split(' ')
                .Select(int.Parse)
                .ToArray();

            IKingKonReception reception = new TrivialKingKonReception();

            var outputBuilder = new StringBuilder();

            var queryCount = inputArgs[0];

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
            public void Join(int time, int duration)
            {
                throw new NotImplementedException();
            }

            public void Cancel(int time)
            {
                throw new NotImplementedException();
            }

            public long Query(int time)
            {
                throw new NotImplementedException();
            }
        }
    }
}
