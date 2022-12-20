using System.Text.RegularExpressions;

namespace AdventOfCode2022;

public static class Day16 {
    public static void Run(bool example) {
        var data = File.ReadAllLines(example ? "day16Example" : "day16Input");

        var valves = new List<Valve>();
        
        foreach (var line in data) 
            valves.Add(new Valve(line.Substring(6, 2)));

        var regex = new Regex(@"Valve [A-Z]{2} has flow rate=(\d+); tunnels? leads? to valves? (.*)");
        for (var i = 0; i < data.Length; i++) {
            var line = data[i];
            var valve = valves[i];

            var match = regex.Match(line);
            // Console.Out.WriteLine(line);
            // Console.Out.WriteLine(match.Groups[0]);
            // Console.Out.WriteLine(match.Groups[1]);
            // Console.Out.WriteLine(match.Groups[2]);
            
            var flowRate = int.Parse(match.Groups[1].Value);
            var leadsToStr = match.Groups[2].Value;

            valve.flowRate = flowRate;
            valve.pathsTo = leadsToStr.Split(',', StringSplitOptions.TrimEntries).Select(name => valves.First(valve => valve.name == name)).ToList();
        }

        // foreach (var valve in valves) {
        //     Console.Out.WriteLine($"Valve {valve.name} has flow rate={valve.flowRate}; tunnels lead to valves {string.Join(", ", valve.pathsTo.Select(valve => valve.name))}");
        // }

        foreach (var valve in valves) {
            var visited = new HashSet<Valve> { valve };
            var next = new Queue<Valve>();
            var nextNext = new Queue<Valve>();
            next.Enqueue(valve);
            var distance = 0;

            while (true) {
                while (next.Count > 0) {
                    var current = next.Dequeue();
                    valve.distanceTo[current] = distance;

                    foreach (var neighbour in current.pathsTo) {
                        if (!visited.Contains(neighbour)) {
                            visited.Add(neighbour);
                            nextNext.Enqueue(neighbour);
                        }
                    }
                }

                if (nextNext.Count == 0)
                    break;
                distance++;
                (next, nextNext) = (nextNext, next);
            }
        }

        var valvesToConsider = valves.Where(v => v.flowRate > 0).ToList();

        var startValve = valves.Find(v => v.name == "AA");
        var (max, maxList) = GetMaxSumVisitingAValve(startValve!, valves, new List<OpenEvent>(), 0, 30);

        foreach (var openEvent in maxList) {
            Console.Out.WriteLine(
                $"Open {openEvent.valve.name} at time {openEvent.minute}. Released for each minute is {maxList.Where(e => e.minute <= openEvent.minute).Sum(e => e.valve.flowRate)}, total released so far is {GetPressureReleasedByEvents(maxList, openEvent.minute)}");
        }

        Console.Out.WriteLine("max: " + max);
    }

    public static (int maxSum, List<OpenEvent> openEventsForMaxSum) GetMaxSumVisitingAValve(Valve fromValve, List<Valve> valves, List<OpenEvent> earlierEvents, int currentMinute, int totalNumMinutes) {
        if (earlierEvents.Count == valves.Count || currentMinute == totalNumMinutes) {
            var maxFromEnd = GetPressureReleasedByEvents(earlierEvents, totalNumMinutes);
            return (maxFromEnd, new List<OpenEvent>(earlierEvents));
        }

        var max = 0;
        List<OpenEvent> maxList = null!;
        foreach (var valve in valves) {
            if (!earlierEvents.Any(e => e.valve == valve)) {
                var wouldOpenAtMinute = currentMinute + fromValve.distanceTo[valve] + 1;
                if (wouldOpenAtMinute <= totalNumMinutes) {
                    earlierEvents.Add(new OpenEvent { valve = valve, minute = wouldOpenAtMinute });
                    var (maxSumFromGoingThereNow, maxListFromGoingThereNow) = GetMaxSumVisitingAValve(valve, valves, earlierEvents, wouldOpenAtMinute, totalNumMinutes);
                    earlierEvents.RemoveAt(earlierEvents.Count - 1);

                    if (maxSumFromGoingThereNow > max) {
                        max = maxSumFromGoingThereNow;
                        maxList = maxListFromGoingThereNow;
                    }
                }
            }
        }

        return (max, maxList);
    }

    public class Valve {
        public string name;
        public int flowRate;
        public List<Valve> pathsTo = new();
        public Dictionary<Valve, int> distanceTo = new();

        public Valve(string name) {
            this.name = name;
        }
    }

    public struct OpenEvent {
        public Valve valve;
        public int minute;
    }

    public static int GetPressureReleasedByEvents(List<OpenEvent> events, int totalNumMinutes) {
        var totalReleased = 0;
        foreach (var openEvent in events) {
            var numMinutesOpen = totalNumMinutes - openEvent.minute;
            if (numMinutesOpen > 0)
                totalReleased += (openEvent.valve.flowRate * numMinutesOpen);
        }

        return totalReleased;
    }
}