namespace AdventOfCode2022;

public static class Day1 {
    public static void Run(bool example) {
        var data = File.ReadAllLines(example ? "day1Example" : "day1Input");

        var sum = 0;

        List<int> elves = new();

        foreach (var line in data) {
            if (string.IsNullOrEmpty(line)) {
                elves.Add(sum);
                sum = 0;
            }
            else {
                var cals = int.Parse(line);
                sum += cals;
            }
        }

        if (sum != 0)
            elves.Add(sum);

        elves.Sort();

        Console.Out.WriteLine($"Problem 1: {elves[^1]}");
        Console.Out.WriteLine($"Problem 2: {elves[^1] + elves[^2] + elves[^3]}");
    }
}