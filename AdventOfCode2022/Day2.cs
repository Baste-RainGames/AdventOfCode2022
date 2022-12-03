namespace AdventOfCode2022;
using static Day2.Hand;
using static Day2.Result;

public static class Day2 {
    public static void Run(bool example) {
        var data = File.ReadAllLines(example ? "day2Example" : "day2Input");

        var score = 0;
        foreach (var line in data) {
            // 0: rock, 1: paper, 2: scissors
            var opponentHand = line[0] - 'A';
            var myHand       = line[2] - 'X';

            score += myHand +  1; // own score
            score += (((myHand - opponentHand) + 4) % 3) * 3; // result score
        }

        Console.Out.WriteLine($"Problem 1 is: {score}");

        score = 0;
        foreach (var line in data) {
            var opponentHand = (Hand) (line[0] - 'A');
            var desiredResult = line[2] switch {
                'X' => Result.Loss,
                'Y' => Result.Draw,
                'Z' => Result.Win,
            };

            var ownHand = opponentHand switch {
                Rock     when desiredResult is Loss => Scissors,
                Rock     when desiredResult is Draw => Rock,
                Rock     when desiredResult is Win  => Paper,
                Paper    when desiredResult is Loss => Rock,
                Paper    when desiredResult is Draw => Paper,
                Paper    when desiredResult is Win  => Scissors,
                Scissors when desiredResult is Loss => Paper,
                Scissors when desiredResult is Draw => Scissors,
                Scissors when desiredResult is Win  => Rock,
            };

            score += (int) desiredResult + (int) ownHand + 1;
        }
        
        Console.Out.WriteLine($"Problem 2 is: {score}");
    }

    public static Hand GetHand(char c) {
        return (Hand)(c - 'A');
    }

    public enum Hand {
        Rock     = 0,
        Paper    = 1,
        Scissors = 2
    }

    public enum Result {
        Loss = 0,
        Draw = 3,
        Win = 6
    }
}