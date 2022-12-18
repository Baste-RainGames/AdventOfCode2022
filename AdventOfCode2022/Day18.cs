using System.Collections;

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
        /*
1  procedure BFS(G, root) is
 2      let Q be a queue
 3      label root as explored
 4      Q.enqueue(root)
 5      while Q is not empty do
 6          v := Q.dequeue()
 7          if v is the goal then
 8              return v
 9          for all edges from v to w in G.adjacentEdges(v) do
10              if w is not labeled as explored then
11                  label w as explored
12                  w.parent := v
13                  Q.enqueue(w)
         */

        var q = new Queue<Vector3>();
        var visited = new bool[xMax, yMax, zMax];

        if (grid[0, 0, 0])
            throw new();
        
        q.Enqueue(new Vector3(0, 0, 0));
        while (q.Count > 0) {
            var current = q.Dequeue();
            foreach (var neighbour in NeighboursInGridWithNoLava(grid, current)) {
                if (!visited[neighbour.x, neighbour.y, neighbour.z]) {
                    visited[neighbour.x, neighbour.y, neighbour.z] = true;
                    q.Enqueue(neighbour);
                }
            }

            IEnumerable<Vector3> NeighboursInGridWithNoLava(bool[,,] lava, Vector3 point) {
                var x = point.x;
                var y = point.y;
                var z = point.z;
                
                if (InBoundsAndEmpty(grid, x - 1, y, z)) yield return new Vector3(x - 1, y, z);
                if (InBoundsAndEmpty(grid, x + 1, y, z)) yield return new Vector3(x + 1, y, z);
                if (InBoundsAndEmpty(grid, x, y + 1, z)) yield return new Vector3(x, y + 1, z);
                if (InBoundsAndEmpty(grid, x, y - 1, z)) yield return new Vector3(x, y - 1, z);
                if (InBoundsAndEmpty(grid, x, y, z + 1)) yield return new Vector3(x, y, z + 1);
                if (InBoundsAndEmpty(grid, x, y, z - 1)) yield return new Vector3(x, y, z - 1);
            }
        }

        var closed = new bool[xMax, yMax, zMax];
        for (int x = 0; x < xMax; x++)
        for (int y = 0; y < yMax; y++)
        for (int z = 0; z < zMax; z++)
            closed[x, y, z] = !visited[x, y, z];
        return closed;
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
    
    private static bool InBoundsAndEmpty(bool[,,] grid, int x, int y, int z) {
        if (x < 0 || y < 0 || z < 0 || x >= grid.GetLength(0) || y >= grid.GetLength(1) || z >= grid.GetLength(2))
            return false;
        return !grid[x, y, z];
    }

    public static Vector3 ParsePoint(string str) {
        var parts = str.Split(',');
        return new Vector3(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2]));
    }
}