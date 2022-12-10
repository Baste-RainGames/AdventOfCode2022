namespace AdventOfCode2022;

public static class Day10 {
    public static void Run(bool example) {
        var data = File.ReadAllLines(example ? "day10Example" : "day10Input");

        var width = 40;
        var height = 6;
        var crt = new char[height, width];

        var signalStrengthSum = RunProgram(data, crt);

        Console.Out.WriteLine("Puzzle 1: " + signalStrengthSum);
        Console.Out.WriteLine("Puzzle 2:");
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++)
                Console.Out.Write(crt[y, x]);
            Console.Out.WriteLine();
        }
    }

    private static int RunProgram(string[] data, char[,] crt) {
        var signalStrengthSum = 0;

        var x = 1;
        var cycle = 1;

        var crtX = 0;
        var crtY = 0;

        for (var i = 0; i < data.Length; i++) {
            var line = data[i];

            if (Util.TryTrimStart(line, "addx ", out var toAddStr)) {
                IncrementCycle();
                IncrementCycle();
                var toAdd = int.Parse(toAddStr);
                x += toAdd;
            }
            else {
                IncrementCycle();
            }

            void IncrementCycle() {
                // Console.Out.WriteLine($"during cycle {cycle}, x is {x}");
                if ((cycle % 40) == 20) {
                    var signalStrength = x * cycle;
                    Console.Out.WriteLine($"Signal strength during cycle {cycle} is {signalStrength}");
                    signalStrengthSum += signalStrength;
                }

                crt[crtY, crtX] = crtX >= x - 1 && crtX <= x + 1 ? '#' : '.';
                crtX++;
                if (crtX == crt.GetLength(1)) {
                    crtX = 0;
                    crtY++;
                    if (crtY == crt.GetLength(0))
                        crtY = 0;
                }

                cycle++;
            }
        }

        return signalStrengthSum;
    }
}