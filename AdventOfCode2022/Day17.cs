using static AdventOfCode2022.Day17.Space;

namespace AdventOfCode2022;

public static class Day17 {
    private const int chuteWidth = 7;

    public static void Run(bool example) {
        var data = File.ReadAllText(example ? "day17Example" : "day17Input");
        var jets = data.Select(t => t == '>').ToArray(); // true: push right

        var chute = new List<Space[]>();
        chute.Add(LineOf(Rock));

        var rockOrder = BuildRockOrder();

        var indexOfTopRocks = 0;
        var jetIndex = 0;
        var nextRockToFall = 0;

        var startTime = DateTime.Now;
        for (int i = 0; i < 2022; i++) {
            var rock = rockOrder[nextRockToFall];
            nextRockToFall = (nextRockToFall + 1) % rockOrder.Count;

            var fallFrom = new Point(2, indexOfTopRocks + 4);
            
            // DrawChute(chute, rock, fallFrom);
            MakeRockFall(chute, rock, fallFrom, jets, ref jetIndex, out var topOfFallenRock);
            // DrawChute(chute);

            if (topOfFallenRock > indexOfTopRocks)
                indexOfTopRocks = topOfFallenRock;
        }
        var endTime = DateTime.Now;
        var duration = (endTime - startTime);
        
        DrawChute(chute);
        Console.Out.WriteLine($"Puzzle 1: {indexOfTopRocks}");
        Console.Out.WriteLine($"It took {duration.TotalMilliseconds} milliseconds!");

        var durationOfAllRocks = (1000000000000 / 2022f) * duration;
        Console.Out.WriteLine($"Puzzle 2 should take {durationOfAllRocks.TotalSeconds} seconds");
        Console.Out.WriteLine(1000000000000);
        Console.Out.WriteLine(1514285714288);
        Console.Out.WriteLine(int.MaxValue);
    }

    private static void DrawChute(List<Space[]> chute, Space[,]? rock = null, Point rockAtPoint = default, bool waitForInput = true) {
        int rockBottom;
        int rockTop;
        int rockLeft;
        int rockRight;

        if (rock != null) {
            MakeRoomForRockInChute(chute, rock, rockAtPoint);
            rockBottom = rockAtPoint.y;
            rockTop = rockBottom + rock.GetLength(0) - 1;
            rockLeft = rockAtPoint.x;
            rockRight = rockAtPoint.x + rock.GetLength(1) - 1;
        }
        else {
            rockTop = rockBottom = rockLeft = rockRight = -1;
        }
        
        for (int y = chute.Count - 1; y >= 0; y--) {
            Space[] spaces = chute[y];
            Console.Out.Write(y == 0 ? '+' : '|');
            for (int x = 0; x < spaces.Length; x++) {
                var draw = spaces[x] switch {
                    Empty => '.',
                    Rock when y > 0 => '#',
                    Rock => '-',
                    _ => throw new ArgumentOutOfRangeException()
                };

                if (y >= rockBottom && y <= rockTop && x >= rockLeft && x <= rockRight) {
                    var rockY = y - rockBottom;
                    var rockX = x - rockLeft;

                    if (rock![rockY, rockX] == Rock) {
                        if (draw == '#')
                            throw new();
                        draw = '@';
                    }
                }
                Console.Out.Write(draw);
            }
            Console.Out.Write(y == 0 ? '+' : '|');
            Console.Out.Write('\n');
        }

        Console.Out.WriteLine();

        if (waitForInput) {
            Console.Out.WriteLine("Waiting for input...");
            Console.In.ReadLine();
        }
    }

    private static void MakeRockFall(List<Space[]> chute, Space[,] rock, Point currentRockPos, bool[] jets, ref int jetIndex, out int topOfRocks) {
        MakeRoomForRockInChute(chute, rock, currentRockPos);

        while (true) {
            var jetPushesRight = jets[jetIndex++];
            if (jetIndex == jets.Length)
                jetIndex = 0;

            var push = jetPushesRight ? new Point(1, 0) : new Point(-1, 0);

            if (CanPlaceRockAt(chute, rock, currentRockPos + push)) 
                currentRockPos += push;

            // DrawChute(chute, rock, currentRockPos);

            if (CanPlaceRockAt(chute, rock, currentRockPos + new Point(0, -1))) {
                currentRockPos.y--;
                // DrawChute(chute, rock, currentRockPos);
            }
            else
                break;
        }

        InsertRock(chute, rock, currentRockPos);
        topOfRocks = currentRockPos.y + rock.GetLength(0) - 1;
    }

    private static void MakeRoomForRockInChute(List<Space[]> chute, Space[,] rock, Point currentRockPos) {
        var requiredHeight = currentRockPos.y + rock.GetLength(0);
        while (chute.Count < requiredHeight)
            chute.Add(LineOf(Empty));
    }

    private static bool CanPlaceRockAt(List<Space[]> chute, Space[,] rock, Point currentRockPos) {
        var startInsertingAtY = currentRockPos.y;
        var startInsertingAtX = currentRockPos.x;
        
        for (int y = 0; y < rock.GetLength(0); y++)
        for (int x = 0; x < rock.GetLength(1); x++) {
            var insertingAtX = x + startInsertingAtX;
            if (insertingAtX is < 0 or >= chuteWidth)
                return false;
            
            if (rock[y, x] == Rock && chute[y + startInsertingAtY][insertingAtX] == Rock)
                return false;
        }

        return true;
    }
    
    private static void InsertRock(List<Space[]> chute, Space[,] rock, Point point) {
        var startInsertingAtY = point.y;
        var startInsertingAtX = point.x;

        for (int y = 0; y < rock.GetLength(0); y++)
        for (int x = 0; x < rock.GetLength(1); x++) {
            var rockSpace = rock[y, x];
            if (rockSpace == Rock)
                chute[y + startInsertingAtY][x + startInsertingAtX] = rockSpace;
        }
    }

    private static List<Space[,]> BuildRockOrder() {
        return new List<Space[,]> {
            // Note! Inverted on Y
            new Space[,]  {
                { Rock, Rock, Rock, Rock }
            },
            new Space[,]  {
                { Empty,   Rock, Empty   },
                { Rock, Rock, Rock },
                { Empty,   Rock, Empty   }
            },
            new Space[,]  {
                { Rock, Rock, Rock },
                { Empty,   Empty,   Rock },
                { Empty,   Empty,   Rock },
            },
            new Space[,]  {
                { Rock },
                { Rock },
                { Rock },
                { Rock }
            },
            new Space[,]  {
                { Rock, Rock },
                { Rock, Rock },
            },
        };
    }


    public static Space[] LineOf(Space space) {
        var line = new Space[chuteWidth];
        for (int i = 0; i < chuteWidth; i++)
            line[i] = space;
        return line;
    }


    public enum Space : byte {
        Empty,
        Rock
    }
}