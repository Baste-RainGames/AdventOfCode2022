namespace AdventOfCode2022;

public static class Day2 {
    public static void Run(bool example) {
        var data = File.ReadAllLines(example ? "day2Example" : "day2Input");

        var scorePuzzle1 = 0;
        var scorePuzzle2 = 0;
        foreach (var line in data) {
            // puzzle 1:
            var opponentHand = line[0] - 'A'; // 0: rock, 1: paper, 2: scissors
            var myHand       = line[2] - 'X';
            
            scorePuzzle1 += myHand +  1; // own score
            scorePuzzle1 += (((myHand - opponentHand) + 4) % 3) * 3; // result score
            
            // puzzle 2:
            var desiredResult = myHand; // 0: loss, 1: draw, 2: win
            var ownHand = (opponentHand + desiredResult + 2) % 3;
            scorePuzzle2 += desiredResult * 3 + ownHand + 1;
        }

        Console.Out.WriteLine($"Problem 1 is: {scorePuzzle1}");
        Console.Out.WriteLine($"Problem 2 is: {scorePuzzle2}");
    }
}