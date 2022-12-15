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
        maxY = Math.Min(maxY, example ? 20 : 4000000);
        
        minX = Math.Max(0, minX);
        maxX = Math.Min(maxX, example ? 20 : 4000000);

        if (example) {
            if (minY != 0 || maxY != 20 || minX != 0 || maxX != 20)
                throw new();
        }
        else {
            if (minY != 0 || maxY != 4000000 || minX != 0 || maxX != 4000000)
                throw new();
        }


        var rowToCheck = example ? 10 : 2000000;

        var ranges = new List<Range>();
        foreach (var pair in pairs) {
            var rangeCoveredOnRow = RangeCoveredByPairOnRow(pair, rowToCheck);
            if (rangeCoveredOnRow.HasValue)
                ranges.Add(rangeCoveredOnRow.Value);
        }

        CombineRanges(ranges);

        var beaconsOnRow = new HashSet<int>();
        foreach (var pair in pairs)
            if (pair.beacon.y == rowToCheck)
                beaconsOnRow.Add(pair.beacon.x);
        
        Console.Out.WriteLine($"Puzzle 1: {(ranges.Sum(range => range.to - range.from + 1) - beaconsOnRow.Count)}");

        // Okay, the way to do this is to find the _range_ covered on the row for each pair.
        // Then, combine the ranges 
        // Then, check if the number of ranges is less than 1
        
        for (int y = minY; y < maxY; y++) {
            ranges.Clear();
            foreach (var pair in pairs) {
                var rangeCoveredOnRow = RangeCoveredByPairOnRow(pair, y);
                if (rangeCoveredOnRow.HasValue) {
                    var clampedRange = rangeCoveredOnRow.Value.Clamp(0, maxX);
                    
                    ranges.Add(clampedRange);
                }
            }
            CombineRanges(ranges);
            
            if ((y % 1000) == 0)
                Console.Out.WriteLine($"y: {y}, ranges: [{string.Join(',', ranges)}]");
            
            if (ranges.Count == 2 || ranges[0].from != 0 || ranges[0].to != 4000000) {
                Console.Out.WriteLine($"It's on row {y} with {ranges.Count} ranges: {string.Join(',', ranges)}");

                Point distressBeaconOn;
                if (ranges.Count == 2) {
                    var minRange = ranges[0].from < ranges[1].from ? ranges[0] : ranges[1];
                    distressBeaconOn = new Point(minRange.to + 1, y);
                }
                else if (ranges[0].from != 0) {
                    if (ranges[0].from != 1)
                        throw new();
                    distressBeaconOn = new Point(0, y);
                }
                else if (ranges[0].to != 4000000) {
                    if (ranges[0].to != 3999999)
                        throw new();
                    distressBeaconOn = new Point(4000000, y);
                }
                else
                    throw new();

                Console.Out.WriteLine("Distress beacon is on: " + distressBeaconOn);
                Console.Out.WriteLine($"Puzzle 2: {(4000000 * distressBeaconOn.x) + distressBeaconOn.y}");
                
                break;
            }
        }
    }

    private static void CombineRanges(List<Range> ranges) {
        bool didCombine = true;
        while (didCombine) {
            didCombine = false;
            
            for (int i = 0; i < ranges.Count; i++)
            for (int j = ranges.Count - 1; j > i; j--) {
                var r1 = ranges[i];
                var r2 = ranges[j];
                if (Overlaps(r1, r2)) {
                    ranges.RemoveAt(j);
                    ranges[i] = new(Math.Min(r1.from, r2.from), Math.Max(r1.to, r2.to));
                    didCombine = true;
                }
            }
        }
    }

    private static bool Overlaps(Range r1, Range r2) {
        if (r1.to < r2.from || r2.to < r1.from)
            return false;

        return true;
    }

    public static Range? RangeCoveredByPairOnRow(SensorBeaconPair pair, int row) {
        var pairDistance = pair.ManhattanDistance;
        var distanceToRow = Math.Abs(row - pair.sensor.y);

        var halfWidth = pairDistance - distanceToRow;
        var startX = pair.sensor.x - halfWidth;
        var endX = pair.sensor.x + halfWidth;

        return startX <= endX ? new Range(startX, endX) : null;
    }

    public record struct SensorBeaconPair(Point sensor, Point beacon) {
        public int ManhattanDistance => Math.Abs(sensor.x - beacon.x) + Math.Abs(sensor.y - beacon.y);
    }

    public record struct Range(int from, int to) {
        public Range Clamp(int minFrom, int maxTo) {
            return new Range(Math.Max(from, minFrom), Math.Min(to, maxTo));
        }
    }
}