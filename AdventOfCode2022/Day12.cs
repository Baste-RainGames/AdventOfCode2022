namespace AdventOfCode2022;

public static class Day12 {
    public static void Run(bool example) {
        var data = File.ReadAllLines(example ? "day12Example" : "day12Input");

        var heights = new char[data.Length, data[0].Length];
        (int x, int y) start = default;
        (int x, int y) end   = default;

        for (int y = 0; y < heights.GetLength(0); y++)
        for (int x = 0; x < heights.GetLength(1); x++) {
            var character = data[y][x];
            char height ;
            switch (character) {
                case 'S':
                    height = 'a';
                    start = (x, y);
                    break;
                case 'E':
                    height = 'z';
                    end = (x, y);
                    break;
                case var c:
                    height = c;
                    break;
            }

            heights[y, x] = height;
        }
        
        for (int y = 0; y < heights.GetLength(0); y++) {
            for (int x = 0; x < heights.GetLength(1); x++)
                Console.Out.Write((x, y) == start ? 'S' : (x, y) == end ? 'E' : heights[y, x]);
            Console.Out.WriteLine();
        }

        Console.Out.WriteLine($"Puzzle 1: {BFS(heights, start, end)}");

        var shortest = int.MaxValue;
        for (int y = 0; y < heights.GetLength(0); y++)
        for (int x = 0; x < heights.GetLength(1); x++) {
            if (heights[y, x] == 'a')
                shortest = Math.Min(shortest, BFS(heights, (x, y), end));
        }

        Console.Out.WriteLine($"Puzzle 2: {shortest}");
    }

    private static int BFS(char[,] heights, (int x, int y) start, (int x, int y) end) {
        var visited = new bool[heights.GetLength(0), heights.GetLength(1)];
        var currentDepth = new Queue<(int x, int y)>();
        var nextDepth    = new Queue<(int x, int y)>();

        var numSteps = 0;

        visited[start.y, start.x] = true;
        currentDepth.Enqueue(start);

        var maxNumSteps = heights.GetLength(0) * heights.GetLength(1);

        while (true) {
            while (currentDepth.Count > 0) {
                var currentTile = currentDepth.Dequeue();

                if (currentTile == end) {
                    return numSteps;
                }

                var (right, left, up, down) = GetUpRightLeftDown(currentTile);
                VisitIfReachableAndNotVisited(right, currentTile, heights, visited, nextDepth);
                VisitIfReachableAndNotVisited(left,  currentTile, heights, visited, nextDepth);
                VisitIfReachableAndNotVisited(up,    currentTile, heights, visited, nextDepth);
                VisitIfReachableAndNotVisited(down,  currentTile, heights, visited, nextDepth);
            }

            (currentDepth, nextDepth) = (nextDepth, currentDepth);
            numSteps++;

            if (numSteps > maxNumSteps)
                return int.MaxValue; // Not reachable, lol!
        }
    }

    private static void VisitIfReachableAndNotVisited((int x, int y) tile, (int x, int y) currentTile, char[,] heights, bool[,] visited, Queue<(int x, int y)> nextDepth) {
        if (NotInBounds(tile, heights))
            return;
        if (visited[tile.y, tile.x])
            return;

        var currentHeight = heights[currentTile.y, currentTile.x];
        var tileHeight    = heights[tile       .y, tile       .x];

        if (tileHeight > currentHeight + 1)
            return;

        visited[tile.y, tile.x] = true;
        nextDepth.Enqueue(tile);
    }

    private static bool NotInBounds((int x, int y) tile, char[,] heights) {
        return tile.x < 0 || tile.y < 0 || tile.y >= heights.GetLength(0) || tile.x >= heights.GetLength(1);
    }

    private static ((int x, int y) right, (int x, int y) left, (int x, int y) up, (int x, int y) down) GetUpRightLeftDown((int x, int y) currentTile) {
        var (x, y) = currentTile;
        return ((x + 1, y), 
                (x - 1, y), 
                (x,     y - 1), 
                (x,     y + 1));
    }
}