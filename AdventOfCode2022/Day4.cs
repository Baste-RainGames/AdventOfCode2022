namespace AdventOfCode2022;

public static class Day4 {
    public static void Run(bool example) {
        var data = File.ReadAllLines(example ? "day4Example" : "day4Input");

        var sectionPairs = data.Select(s => s.Split(',', '-'))
                               .Select(strs => Array.ConvertAll(strs, int.Parse))
                               .ToArray();

        var fullOverlaps = 0;
        var partialOverlaps = 0;
        foreach (var sectionPair in sectionPairs) {
            var (from0, to0, from1, to1) = (sectionPair[0], sectionPair[1], sectionPair[2], sectionPair[3]);

            if ((from0 >= from1 && to0 <= to1) || (from0 <= from1 && to0 >= to1))
                fullOverlaps++;

            if (!(to0 < from1 || to1 < from0))
                partialOverlaps++;
        }

        Console.Out.WriteLine("puzzle 1 (full overlaps):" + fullOverlaps);
        Console.Out.WriteLine("puzzle 2 (partial overlaps):" + partialOverlaps);
    }
}