namespace AdventOfCode2022;

public static class Day14 {
    public static void Run(bool example) {
        var data = File.ReadAllLines(example ? "day14Example" : "day14Input");

        var filledPoints = new HashSet<Point>();

        foreach (var line in data) {
            var parts = line.Split(" -> ").Select(ParseCoordinate).ToArray();

            for (int i = 0; i < parts.Length - 1; i++) {
                var current = parts[i];
                var target = parts[i + 1];

                var deltaX = Math.Clamp(target.x - current.x, -1, 1);
                var deltaY = Math.Clamp(target.y - current.y, -1, 1);

                filledPoints.Add(current);
                do {
                    current.x += deltaX;
                    current.y += deltaY;
                    filledPoints.Add(current);
                }
                while (current != target);
            }
        }

        var minX = filledPoints.Min(point => point.x);
        var maxX = filledPoints.Max(point => point.x);
        var maxY = filledPoints.Max(point => point.y);
        
        var hasRocks = new HashSet<Point>(filledPoints);
        var lastPath = new HashSet<Point>();

        loopStart:
        // DrawGrid();
        lastPath.Clear();
        var sandPos = new Point(500, 0);

        while (sandPos.y <= maxY) {
            if (!filledPoints.Contains(new Point(sandPos.x, sandPos.y + 1)))
                sandPos.y ++;
            else if (!filledPoints.Contains(new Point(sandPos.x - 1, sandPos.y + 1))) {
                sandPos.x--;
                sandPos.y++;
            }
            else if (!filledPoints.Contains(new Point(sandPos.x + 1, sandPos.y + 1))) {
                sandPos.x++;
                sandPos.y++;
            }
            else {
                filledPoints.Add(sandPos);
                goto loopStart;
            }
            lastPath.Add(sandPos);
        }

        DrawGrid();
        Console.Out.WriteLine($"Puzzle 1 units of sand: {(filledPoints.Count - hasRocks.Count)}");

        filledPoints = new HashSet<Point>(hasRocks);
        
        DrawGrid();

        Console.In.ReadLine();
        
        loop2Start:
        // DrawGrid();
        sandPos = new Point(500, 0);
        if (filledPoints.Contains(sandPos))
            goto loop2End;

        while (true) {
            if (!filledPoints.Contains(new Point(sandPos.x, sandPos.y + 1)))
                sandPos.y ++;
            else if (!filledPoints.Contains(new Point(sandPos.x - 1, sandPos.y + 1))) {
                sandPos.x--;
                sandPos.y++;
            }
            else if (!filledPoints.Contains(new Point(sandPos.x + 1, sandPos.y + 1))) {
                sandPos.x++;
                sandPos.y++;
            }
            else {
                filledPoints.Add(sandPos);
                goto loop2Start;
            }

            if (sandPos.y == maxY + 1) {
                filledPoints.Add(sandPos);
                goto loop2Start;
            }
        }

        throw new();
        
        loop2End:
        lastPath.Clear();
        DrawGrid();
        Console.Out.WriteLine($"Puzzle 1 units of sand: {(filledPoints.Count - hasRocks.Count)}");
        
        void DrawGrid() {
            for (int y = 0; y <= maxY; y++) {
                Console.Out.Write(y);
                Console.Out.Write(' ');
                for (int x = minX; x <= maxX; x++) {
                    var point = new Point(x, y);
                    Console.Out.Write(point == new Point(500, 0)   ? '+' :
                                      lastPath.Contains(point)     ? '~' :
                                      hasRocks.Contains(point)     ? '#' : 
                                      filledPoints.Contains(point) ? 'o' : 
                                                                     '.');
                }

                Console.Out.WriteLine();
            }
        }
    }


    private static Point ParseCoordinate(string arg) {
        var split = arg.Split(',');
        if (split.Length != 2)
            throw new();

        return new Point(int.Parse(split[0]), int.Parse(split[1]));
    }

    public record struct Point(int x, int y);
}