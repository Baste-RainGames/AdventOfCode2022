using System.Text.RegularExpressions;

namespace AdventOfCode2022;

public static class Day15 {
    public static void Run(bool example) {
        var data = File.ReadAllLines(example ? "day15Example" : "day15Input");

        var regex = new Regex(@"Sensor at x=(-?\d*), y=(-?\d*): closest beacon is at x=(-?\d*), y=(-?\d*)");
        var pairs = new List<SensorBeaconPair>();

        foreach (var line in data) {
            var match = regex.Match(line);

            var groups = match.Groups;
            var sensorX = int.Parse(groups[1].Value);
            var sensorY = int.Parse(groups[2].Value);
            var beaconX = int.Parse(groups[3].Value);
            var beaconY = int.Parse(groups[4].Value);
            
            pairs.Add(new SensorBeaconPair(new Point(sensorX, sensorY), new Point(beaconX, beaconY) ));
        }

        var maxX = pairs.Max(p => Math.Max(Math.Max(p.beacon.x, p.sensor.x), p.sensor.x + p.ManhattanDistance));
        var minX = pairs.Min(p => Math.Min(Math.Min(p.beacon.x, p.sensor.x), p.sensor.x - p.ManhattanDistance));
        var maxY = pairs.Max(p => Math.Max(Math.Max(p.beacon.y, p.sensor.y), p.sensor.y + p.ManhattanDistance));
        var minY = pairs.Min(p => Math.Min(Math.Min(p.beacon.y, p.sensor.y), p.sensor.y - p.ManhattanDistance));

        minY = Math.Max(0, minY);
        maxY = Math.Min(maxY, 4000000);
        
        minX = Math.Max(0, minX);
        maxX = Math.Min(maxX, 4000000);

        if (example) {
            var toConsider = pairs.First(p => p.sensor == new Point(8, 7));
            for (int y = minY; y <= maxY; y++) {
                var covered = CoveredByPairOnRow(toConsider, y);
                Console.Out.Write(y);
                Console.Out.Write(' ');
                for (int x = minX; x <= maxX; x++) {
                    var point = new Point(x, y);
                    if (pairs.Any(p => p.sensor == point))
                        Console.Out.Write('S');
                    else if (pairs.Any(p => p.beacon == point))
                        Console.Out.Write('B');
                    else if (covered.Contains(x))
                        Console.Out.Write('#');
                    else
                        Console.Out.Write('.');
                }

                Console.Out.WriteLine();
            }
        }

        var rowToCheck = example ? 10 : 2000000;
        var coveredOnRow = new HashSet<int>();

        foreach (var pair in pairs)
        foreach (var point in CoveredByPairOnRow(pair, rowToCheck))
            coveredOnRow.Add(point);

        var beaconsOnRow = new HashSet<int>();
        foreach (var pair in pairs)
            if (pair.beacon.y == rowToCheck)
                beaconsOnRow.Add(pair.beacon.x);

        Console.Out.WriteLine($"Puzzle 1: {(coveredOnRow.Count - beaconsOnRow.Count)}");

        Console.Out.WriteLine("How much for Puzzle 2, really?");
        Console.Out.WriteLine($"x: {minX} - {maxX}, y: {minY} - {maxY}");
        Console.Out.WriteLine((maxX - minX) * (maxY - minY));

        var dumbArray = new byte[maxX - minX + 1, maxY - minY + 1];
        var coveredByPair = new List<Point>();
        for (var i = 0; i < pairs.Count; i++) {
            var pair = pairs[i];
            Console.Out.WriteLine($"Pair {i} out of {pairs.Count}");
            FindCoveredByPair(pair, coveredByPair);
            foreach (var point in coveredByPair) 
                dumbArray[point.x - minX, point.y - minY] = 1;
        }

        Point result = default;
        
        for (int x = minX; x <= maxX; x++)
        for (int y = minY; y <= maxY; y++) {
            
        }
    }

    private static void FindCoveredByPair(SensorBeaconPair pair, List<Point> result) {
        var pairDistance = pair.ManhattanDistance;
        result.Clear();

        var startRow = pair.sensor.y - pairDistance;
        var endRow = pair.sensor.y + pairDistance;
        
        for (int row = startRow; row <= endRow; row++) {
            var distanceToRow = Math.Abs(row - pair.sensor.y);

            var halfWidth = pairDistance - distanceToRow;
            if (halfWidth >= 0) {
                var startX = pair.sensor.x - halfWidth;
                var endX = pair.sensor.x + halfWidth;

                for (int x = startX; x <= endX; x++) {
                    result.Add(new Point(x, row));
                }
            }
        }
    }

    public static List<int> CoveredByPairOnRow(SensorBeaconPair pair, int row) {
        var result = new List<int>();

        var pairDistance = pair.ManhattanDistance;
        var distanceToRow = Math.Abs(row - pair.sensor.y);

        var halfWidth = pairDistance - distanceToRow;
        if (halfWidth >= 0) {
            var startX = pair.sensor.x - halfWidth;
            var endX = pair.sensor.x + halfWidth;

            for (int x = startX; x <= endX; x++) {
                result.Add(x);
            }
        }

        return result;
    }

    public record struct SensorBeaconPair(Point sensor, Point beacon) {
        public int ManhattanDistance => Math.Abs(sensor.x - beacon.x) + Math.Abs(sensor.y - beacon.y);
    }

    public record struct Point(int x, int y) {
        public static Point operator +(Point a, Point b) {
            return new Point(a.x + b.x, a.y + b.y);
        }
        public static Point operator -(Point a, Point b) {
            return new Point(a.x - b.x, a.y - b.y);
        }
    }
}