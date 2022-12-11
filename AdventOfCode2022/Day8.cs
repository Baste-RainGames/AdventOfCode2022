namespace AdventOfCode2022;

public static class Day8 {
    public static void Run(bool example) {
        var data = File.ReadAllLines(example ? "day8Example" : "day8Input");

        var (numColumns, numRows) = (data[0].Length, data.Length);

        var trees = new Tree[numColumns, numRows];
        for (int y = 0; y < numRows; y++)
        for (int x = 0; x < numColumns; x++)
            trees[y, x].height = data[y][x] - '0';

        for (int y = 0; y < numRows; y++) {
            var tallestOnRow = -1;
            for (int x = 0; x < numColumns; x++) {
                trees[y, x].tallestLeft = tallestOnRow;
                tallestOnRow = Math.Max(tallestOnRow, trees[y, x].height);
            }
        }

        for (int y = 0; y < numRows; y++) {
            var tallestOnRow = -1;
            for (int x = numColumns - 1; x >= 0; x--) {
                trees[y, x].tallestRight = tallestOnRow;
                tallestOnRow = Math.Max(tallestOnRow, trees[y, x].height);
            }
        }

        for (int x = 0; x < numColumns; x++) {
            var tallestOnColumn = -1;
            for (int y = 0; y < numRows; y++) {
                trees[y, x].tallestUp = tallestOnColumn;
                tallestOnColumn = Math.Max(tallestOnColumn, trees[y, x].height);
            }
        }
        
        for (int x = 0; x < numColumns; x++) {
            var tallestOnColumn = -1;
            for (int y = numRows - 1; y >= 0; y--) {
                trees[y, x].tallestDown = tallestOnColumn;
                tallestOnColumn = Math.Max(tallestOnColumn, trees[y, x].height);
            }
        }

        var countVisible = 0;
        for (int y = 0; y < numRows; y++)
        for (int x = 0; x < numColumns; x++)
            if (trees[y, x].Visible)
                countVisible++;

        Console.Out.WriteLine($"Puzzle 1: {countVisible}");
        
        // Brute approach
        int maxScenicness = 0;
        for (int y = 0; y < numRows; y++)
        for (int x = 0; x < numColumns; x++) {
            var height = trees[y, x].height;

            var numVisibleRight = 0; 
            for (int x2 = x + 1; x2 < numColumns; x2++) {
                numVisibleRight++;
                if (trees[y, x2].height >= height)
                    break;
            }
            
            var numVisibleLeft = 0; 
            for (int x2 = x - 1; x2 >= 0; x2--) {
                numVisibleLeft++;
                if (trees[y, x2].height >= height)
                    break;
            }

            var numVisibleDown = 0;
            for (int y2 = y + 1; y2 < numRows; y2++) {
                numVisibleDown++;
                if (trees[y2, x].height >= height)
                    break;
            }
            
            var numVisibleUp = 0;
            for (int y2 = y - 1; y2 >= 0; y2--) {
                numVisibleUp++;
                if (trees[y2, x].height >= height)
                    break;
            }

            trees[y, x].scenicness = numVisibleUp * numVisibleRight * numVisibleDown * numVisibleLeft;
            if (trees[y, x].scenicness > maxScenicness)
                maxScenicness = trees[y, x].scenicness;
        }

        Console.Out.WriteLine("Puzzle 2: " + maxScenicness);
    }

    private static void PrintTree(Tree[,] trees, int row, int column) {
        var tree = trees[row, column];

        Console.Out.WriteLine("height: " + tree.height);
        Console.Out.WriteLine("tallestUp: " + tree.tallestUp);
        Console.Out.WriteLine("tallestRight: " + tree.tallestRight);
        Console.Out.WriteLine("tallestDown: " + tree.tallestDown);
        Console.Out.WriteLine("tallestLeft: " + tree.tallestLeft);
    }

    private static void PrintTreeHeights(Tree[,] trees) {
        for (int y = 0; y < trees.GetLength(0); y++) {
            for (int x = 0; x < trees.GetLength(1); x++)
                Console.Out.Write(trees[y, x].height);
            Console.Out.WriteLine("");
        }
    }

    private static void PrintTreesVisible(Tree[,] trees) {
        for (int y = 0; y < trees.GetLength(0); y++) {
            for (int x = 0; x < trees.GetLength(1); x++)
                Console.Out.Write(trees[y, x].Visible ? "V" : "H");
            Console.Out.WriteLine("");
        }
    }
    
    public struct Tree {
        public int height;
        public int tallestUp;
        public int tallestRight;
        public int tallestDown;
        public int tallestLeft;
        public int scenicness;

        public bool Visible => height > tallestUp    ||
                               height > tallestRight ||
                               height > tallestDown  ||
                               height > tallestLeft;
    }
}