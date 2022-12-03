using Console = System.Console;

namespace AdventOfCode2022;

public static class Day3 {
    public static void Run(bool example) {
        var data = File.ReadAllLines(example ? "day3Example" : "day3Input");

        var prioritySum = 0;
        foreach (var rucksack in data) {
            var compartmentSize = rucksack.Length / 2;
            var firstCompartment = rucksack.Take(compartmentSize);
            var secondCompartment = rucksack.Skip(compartmentSize).Take(compartmentSize);

            var sharedElements = firstCompartment.Intersect(secondCompartment);
            var singleSharedElement = sharedElements.Single();

            var priority = GetPriority(singleSharedElement);
            prioritySum += priority;
        }

        Console.Out.WriteLine("first puzzle: " + prioritySum);

        var groupPrioritySum = 0;
        for (int i = 0; i < data.Length; i+= 3) {
            var rucksack0 = data[i + 0];
            var rucksack1 = data[i + 1];
            var rucksack2 = data[i + 2];

            var sharedElement = rucksack0.Intersect(rucksack1).Intersect(rucksack2).Single();

            groupPrioritySum += GetPriority(sharedElement);
        }
        
        Console.Out.WriteLine("second puzzle: " + groupPrioritySum);
    }

    private static int GetPriority(char item) {
        return item <= 'Z' ? item - 'A' + 27 : item - 'a' + 1;
    }
}