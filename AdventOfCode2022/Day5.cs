using System.Text.RegularExpressions;

namespace AdventOfCode2022;

public static class Day5 {
    public static void Run(bool example) {
        var data = File.ReadAllLines(example ? "day5Example" : "day5Input");

        var startOfCommands = Array.FindIndex(data, line => line.StartsWith("move"));
        var numStacks = data[startOfCommands - 2].Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;

        var puzzle1Stacks = new Stack<char>[numStacks];
        var puzzle2Stacks = new Stack<char>[numStacks];
        
        for (int i = 0; i < numStacks; i++) 
            puzzle1Stacks[i] = new();
        for (int i = 0; i < numStacks; i++) 
            puzzle2Stacks[i] = new();

        for (int i = startOfCommands - 3; i >= 0; i--) {
            var line = data[i];

            for (int j = 0; j < numStacks; j++) {
                // Using the fact that crates have a single-character identifier
                var indexOfCrateOnLine = 1 + (4 * j);
                if (indexOfCrateOnLine < line.Length) {
                    var maybeCrate = line[indexOfCrateOnLine];
                    if (maybeCrate != ' ') {
                        puzzle1Stacks[j].Push(maybeCrate);
                        puzzle2Stacks[j].Push(maybeCrate);
                    }
                }
            }
        }

        for (int i = startOfCommands; i < data.Length; i++) {
            var command = new MoveCommand(data[i]);
            command.ApplyToInPuzzle1Manner(puzzle1Stacks);
            command.ApplyToInPuzzle2Manner(puzzle2Stacks);
        }

        var puzzle1Result = "";
        foreach (var stack in puzzle1Stacks) 
            puzzle1Result += stack.Peek();
        var puzzle2Result = "";
        foreach (var stack in puzzle2Stacks) 
            puzzle2Result += stack.Peek();

        Console.Out.WriteLine($"Puzzle 1: {puzzle1Result}");
        Console.Out.WriteLine($"Puzzle 2: {puzzle2Result}");
    }

    private static void PrintStacks(Stack<char>[] stacks) {
        for (var i = 0; i < stacks.Length; i++) {
            var stack = stacks[i];
            var reversed = stack.ToList();
            reversed.Reverse();

            Console.Out.WriteLine($"{(i + 1)}: " + String.Join(',', reversed));
        }
        Console.Out.WriteLine("");
    }

    public struct MoveCommand {
        public int fromStack;
        public int toStack;
        public int count;

        private static Regex regex = new Regex(@"move (\d+) from (\d+) to (\d)+"); 
        public MoveCommand(string commandStr) {
            var match = regex.Match(commandStr);

            count     = int.Parse(match.Groups[1].Value);
            fromStack = int.Parse(match.Groups[2].Value) - 1; // 1-index to 0-index
            toStack   = int.Parse(match.Groups[3].Value) - 1; // 1-index to 0-index
        }

        public void ApplyToInPuzzle1Manner(Stack<char>[] stacks) {
            for (int i = 0; i < count; i++)
                stacks[toStack].Push(stacks[fromStack].Pop());
        }

        private static Stack<char> transferStack = new ();
        public void ApplyToInPuzzle2Manner(Stack<char>[] stacks) {
            for (int i = 0; i < count; i++)
                transferStack.Push(stacks[fromStack].Pop());
            for (int i = 0; i < count; i++) 
                stacks[toStack].Push(transferStack.Pop());
        }
    }
}