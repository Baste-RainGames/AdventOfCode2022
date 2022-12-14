using AdventOfCode2022;

var dayHandler = new Action<bool>[] {
    Day1.Run,
    Day2.Run,
    Day3.Run,
    Day4.Run,
    Day5.Run,
    Day6.Run,
    Day7.Run,
    Day8.Run,
    Day9.Run,
    Day10.Run,
    Day11.Run,
    Day12.Run,
    Day13.Run,
    Day14.Run,
    Day15.Run,
    Day16.Run,
    Day17.Run,
    Day18.Run,
    Day19.Run,
    Day20.Run,
    Day21.Run,
    Day22.Run,
    Day23.Run,
    Day24.Run,
};


Console.Out.WriteLine("Welcome to Advent Of Code 2022");
while (true) {
    Console.Out.WriteLine("\nWhich day do you want to run?");

    var dayInput = Console.In.ReadLine();

    if (dayInput is string inputStr) {
        if (inputStr.ToLower() == "exit")
            return;

        if (int.TryParse(inputStr, out var day)) {
            if (day is < 1 or > 24) {
                Console.Out.WriteLine($"Day must be between 1 and 24, you wrote {day}s, silly!");
                continue;
            }
            
            Console.Out.WriteLine($"Do you want to run Day {day} with:\n" +
                                  $"Example input? Write 'e'\n" +
                                  $"Real input? Write 'r'\n" +
                                  $"Both? Write 'b' or nothing");

            // var doExample = false;
            var typeInput = Console.In.ReadLine();
            if (typeInput == null) {
                Console.Out.WriteLine("No input to read, end of stream?");
                continue;
            }

            WhatToDo whatToDo = typeInput.ToLowerInvariant() switch {
                "e" or "example" => WhatToDo.Example,
                "r" or "real"    => WhatToDo.Real,
                ""  or "both"    => WhatToDo.Both,
                _                => WhatToDo.Nothing
            };

            if (whatToDo == WhatToDo.Nothing) {
                Console.Out.WriteLine($"I don't understand {typeInput}");
                continue;
            }
            
            if (whatToDo != WhatToDo.Real) {
                Console.Out.WriteLine($"Running day {day} with example input");
                dayHandler[day - 1].Invoke(true);
            }
            if (whatToDo != WhatToDo.Example) {
                Console.Out.WriteLine($"Running day {day} with real input");
                dayHandler[day - 1].Invoke(false);
            }
        }
        // else if (inputStr == "meta") {
        //     for (int i = 2; i <= 24; i++) {
        //         var examplePath = $@"C:\Users\bnbua\RiderProjects\AdventOfCode2022\AdventOfCode2022\Day{i}Example";
        //         var inputPath = $@"C:\Users\bnbua\RiderProjects\AdventOfCode2022\AdventOfCode2022\Day{i}Input";
        //         File.WriteAllText(examplePath, "Nothing Here Yet");
        //         File.WriteAllText(inputPath, "Nothing Here Yet");
        //     }
        //
        //     return;
        // }
        else {
            Console.Out.WriteLine($"I don't understand \"{inputStr}\"");
        }
    }
}

enum WhatToDo {
    Example,
    Real,
    Both,
    Nothing
}