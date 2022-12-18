namespace AdventOfCode2022;

public static class Day18 {
    public static void Run(bool example) {
        var data = File.ReadAllLines(example ? "day18Example" : "day18Input");

        var points = data.Select(ParsePoint).ToList();

        var xMax = points.Max(p => p.x) + 1;
        var yMax = points.Max(p => p.y) + 1;
        var zMax = points.Max(p => p.z) + 1;
        var grid = new bool[xMax, yMax, zMax];

        foreach (var point in points)
            grid[point.x, point.y, point.z] = true;

        Console.Out.WriteLine("Puzzle 1: " + CalcSurfacesOnGrid(grid, xMax, yMax, zMax));

        var filledGrid = CloseInteriorSurfaces(xMax, yMax, zMax, grid);

        Console.Out.WriteLine("Puzzle 2: " + CalcSurfacesOnGrid(filledGrid, xMax, yMax, zMax));
    }

    private static bool[,,] CloseInteriorSurfaces(int xMax, int yMax, int zMax, bool[,,] grid) {
        // This is wrong! If there's an l-shape into the shape, that won't work. Correct approach is to BFS from the outside of the shape
        var closed = new bool[xMax, yMax, zMax];
        for (int x = 0; x < xMax; x++)
        for (int y = 0; y < yMax; y++)
        for (int z = 0; z < zMax; z++)
            closed[x, y, z] = true; // completely fill the closed shape 

        // Shoot rays from all 6 directions, and remove fill until we hit interior fill
        for (int x = 0; x < xMax; x++)
        for (int y = 0; y < yMax; y++) {
            // z forwards
            for (int z = 0; z < zMax; z++) {
                if (grid[x, y, z])
                    break;
                closed[x, y, z] = false;
            }
            // z backwards
            for (int z = zMax - 1; z >= 0; z--) {
                if (grid[x, y, z])
                    break;
                closed[x, y, z] = false;
            }
        }
        
        for (int x = 0; x < xMax; x++)
        for (int z = 0; z < zMax; z++) {
            // y forwards
            for (int y = 0; y < yMax; y++) {
                if (grid[x, y, z])
                    break;
                closed[x, y, z] = false;
            }
            // y backwards
            for (int y = yMax - 1; y >= 0; y--) {
                if (grid[x, y, z])
                    break;
                closed[x, y, z] = false;
            }
        }

        for (int y = 0; y < yMax; y++)
        for (int z = 0; z < zMax; z++) {
            // x forwards
            for (int x = 0; x < xMax; x++) {
                if (grid[x, y, z])
                    break;
                closed[x, y, z] = false;
            }
            
            // x backwards
            for (int x = xMax - 1; x >= 0; x--) {
                if (grid[x, y, z])
                    break;
                closed[x, y, z] = false;
            }
        }

        return closed;

        // for (int x = 0; x < xMax; x++)
        // for (int y = 0; y < yMax; y++)
        // for (int z = 0; z < zMax; z++) {
        //     if (!grid[x, y, z]) {
        //         if (LaveInAllDirections(grid, x, y, z))
        //             grid[x, y, z] = true;
        //     }
        // }
    }

    private static int CalcSurfacesOnGrid(   bool[,,] grid, int xMax, int yMax, int zMax) {
        var numSurfaces = 0;
        for (int x = 0; x < xMax; x++)
        for (int y = 0; y < yMax; y++)
        for (int z = 0; z < zMax; z++) {
            if (grid[x, y, z]) {
                if (OutOfBoundsOrEmpty(grid, x - 1, y, z)) numSurfaces++;
                if (OutOfBoundsOrEmpty(grid, x + 1, y, z)) numSurfaces++;
                if (OutOfBoundsOrEmpty(grid, x, y + 1, z)) numSurfaces++;
                if (OutOfBoundsOrEmpty(grid, x, y - 1, z)) numSurfaces++;
                if (OutOfBoundsOrEmpty(grid, x, y, z + 1)) numSurfaces++;
                if (OutOfBoundsOrEmpty(grid, x, y, z - 1)) numSurfaces++;
            }
        }

        return numSurfaces;
    }

    private static bool OutOfBoundsOrEmpty(bool[,,] grid, int x, int y, int z) {
        if (x < 0 || y < 0 || z < 0 || x >= grid.GetLength(0) || y >= grid.GetLength(1) || z >= grid.GetLength(2))
            return true;
        return !grid[x, y, z];
    }

    public static Vector3 ParsePoint(string str) {
        var parts = str.Split(',');
        return new Vector3(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2]));
    }
}