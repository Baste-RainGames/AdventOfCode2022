namespace AdventOfCode2022;

public static class Day9 {
    public static void Run(bool example) {
        var dataP1 = File.ReadAllLines(example ? "day9Example1" : "day9Input");
        var dataP2 = File.ReadAllLines(example ? "day9Example2" : "day9Input");

        Console.Out.WriteLine("Puzzle 1: " + FindNumVisitedByTailEnd(2, dataP1));
        Console.Out.WriteLine("Puzzle 2: " + FindNumVisitedByTailEnd(10, dataP2));
    }

    private static int FindNumVisitedByTailEnd(int numSegments, string[] data, bool draw = false, bool step = false) {
        var body = new (int x, int y)[numSegments];

        var visited = new HashSet<(int x, int y)> {
            body[^1]
        };

        if (draw) {
            DrawBoard(visited, body, -11, 14, -15, 5);
            if (step)
                Console.In.ReadLine();
        }

        foreach (var line in data) {
            var parts = line.Split();
            var (dir, amount) = (parts[0][0], int.Parse(parts[1]));

            for (int i = 0; i < amount; i++) {
                var head = body[0];
                switch (dir) {
                    case 'R':
                        head = (head.x + 1, head.y);
                        break;
                    case 'U':
                        head = (head.x, head.y - 1);
                        break;
                    case 'L':
                        head = (head.x - 1, head.y);
                        break;
                    case 'D':
                        head = (head.x, head.y + 1);
                        break;
                }
                body[0] = head;

                for (int j = 0; j < body.Length - 1; j++) {
                    var front = body[j];
                    var back = body[j + 1];

                    (int x, int y) delta = (front.x - back.x, front.y - back.y);

                    if (Math.Abs(delta.x) > 1 || Math.Abs(delta.y) > 1) {
                        (int x, int y) deltaClamped = (Math.Clamp(delta.x, -1, 1), Math.Clamp(delta.y, -1, 1));
                        back = (back.x + deltaClamped.x, back.y + deltaClamped.y);
                    }

                    body[j + 1] = back;
                }

                visited.Add(body[^1]);
            }

            if (draw) {
                Console.Out.WriteLine("\n" + line + "\n");
                DrawBoard(visited, body, -11, 14, -15, 5);

                if (step)
                    Console.In.ReadLine();
            }
        }

        return visited.Count;
    }

    private static void DrawBoard(HashSet<(int x, int y)> visited, (int x, int y)[] body, int? xMin = null, int? xMax = null, int? yMin = null, int? yMax = null) {
        var combined = visited.Concat(body).ToList();
        xMax ??= combined.Select(pair => pair.x).Max();
        yMax ??= combined.Select(pair => pair.y).Max();
        xMin ??= combined.Select(pair => pair.x).Min();
        yMin ??= combined.Select(pair => pair.y).Min();

        var grid = new char[yMax.Value - yMin.Value + 1, xMax.Value - xMin.Value + 1];
        for (int y = 0; y < grid.GetLength(0); y++)
        for (int x = 0; x < grid.GetLength(1); x++) {
            grid[y, x] = '.';
        }

        foreach (var (x, y) in visited) {
            var (gridX, gridY) = (x - xMin.Value, y - yMin.Value);
            grid[gridY, gridX] = '#';
        }

        for (var i = 0; i < body.Length; i++) {
            var (x, y) = body[i];
            var (gridX, gridY) = (x - xMin.Value, y - yMin.Value);

            grid[gridY, gridX] = i == 0 ? 'H' : (char) (i + '0');
        }

        for (int y = 0; y < grid.GetLength(0); y++) {
            for (int x = 0; x < grid.GetLength(1); x++) {
                Console.Out.Write(grid[y, x]);
            }

            Console.Out.WriteLine();
        }

        Console.Out.WriteLine();
    }
}