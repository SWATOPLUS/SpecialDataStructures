using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sds10KingKonReception.TestGenerator
{
    internal class Program
    {
        private const string OutputFileName = "input.txt";
        private static int QueryCount = 100_000;
        private static int MaxInTime = 1_000_000;
        private static int MaxDurationTime = 1_000_000;

        private static readonly Random Random = new Random();

        private static void Main(string[] args)
        {
            if (args.Length == 3)
            {
                QueryCount = int.Parse(args[0]);
                MaxInTime = int.Parse(args[1]);
                MaxDurationTime = int.Parse(args[2]);
            }

            var commands = BuildCommands();
            var lines = new[] {QueryCount.ToString()}
                .Concat(commands);

            File.WriteAllLines(OutputFileName, lines);
        }

        private static IEnumerable<string> BuildCommands()
        {
            var inTimes = new Dictionary<int, int>();

            foreach (var index in Enumerable.Range(0, QueryCount))
            {
                var randomRange = inTimes.Any() ? 3 : 2;

                var result = null as string;

                switch (Random.Next(randomRange))
                {
                    case 0:
                        result = $"? {Random.Next(1, MaxInTime + 1)}";
                        break;
                    case 1:
					    /*
                        var possibleInTimes = Enumerable
                            .Range(1, MaxInTime)
                            .Except(inTimes.Keys)
                            .ToArray();

                        var inTimeIndex = Random.Next(possibleInTimes.Length);
                        var inTime = possibleInTimes[inTimeIndex];
						*/
						
						var inTime = Random.Next(1, MaxInTime + 1);
					
						while(inTimes.ContainsKey(inTime))
                        {
                            inTime = Random.Next(1, MaxInTime + 1);
                        }
						
                        inTimes.Add(inTime, index);

                        result = $"+ {inTime} {Random.Next(1, MaxDurationTime + 1)}";
                        break;
                    case 2:
                        var skipTime = inTimes.ToArray()[Random.Next(inTimes.Count)];
                        inTimes.Remove(skipTime.Key);

                        result = $"- {skipTime.Value + 1}";
                        break;
                        
                }

                yield return result;
            }
        }

    }
}
