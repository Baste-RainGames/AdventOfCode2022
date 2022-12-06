namespace AdventOfCode2022;

public static class Day6 {
    public static void Run(bool example) {
        var data = File.ReadAllText(example ? "day6Example" : "day6Input");

        Console.Out.WriteLine($"Puzzle 1: {IndexAfterFirstDistinctSequenceOfLength(data, 4)}");
        Console.Out.WriteLine($"Puzzle 2: {IndexAfterFirstDistinctSequenceOfLength(data, 14)}");
    }

    private static int IndexAfterFirstDistinctSequenceOfLength(string data, int length) {
        var list = new List<Char>(length);

        for (int i = 0; i < length; i++) 
            list.Add(data[i]);

        for (int i = length; i < data.Length; i++) {
            if (list.Distinct().Count() == length)
                return i;
            list.RemoveAt(0);
            list.Add(data[i]);
        }

        return -1;
    }
}