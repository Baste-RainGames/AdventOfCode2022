using static AdventOfCode2022.Day17.Space;

namespace AdventOfCode2022;

public static class Day17 {
    private const int chuteWidth = 7;

    public static void Run(bool example) {
        if (example) {
            Console.Out.WriteLine("Code stopped working at the example long ago! Values from the real input has been hard-coded");
            return;
        }

        // DoTheTest(new List<int> { 1, 2, 3, 1, 2, 3 });
        // DoTheTest(new List<int> { 1, 2, 3, 4, 5, 6 });
        // DoTheTest(new List<int> { 1, 2, 1, 2 });
        // DoTheTest(new List<int> { 1, 2, 7, 4, 1, 2, 4, 4, 6 });
        // DoTheTest(new List<int> { 1, 2, 7, 4, 1, 2, 4, 4, 6, 1, 2, 7, 4, 1, 2, 4, 4, 6 });
        // DoTheTest(new List<int> { 3, 3, 1, 2, 7, 4, 1, 2, 4, 4, 6, 1, 2, 7, 4, 1, 2, 4, 4, 6 });
        // return;

        long numRocksFallenBeforeFirstLoop = 1730;
        long heightBeforeFirstLoop = 2773;
        long numRocksFallenEachLoop = 1720;
        long heightGainedEachLoop = 2738;

        long numRocksToFall = 1_000_000_000_000;
        Console.Out.WriteLine(long.MaxValue);
        Console.Out.WriteLine(numRocksToFall);
        
        long numRocksToFallAfterFirstLoop = numRocksToFall - numRocksFallenBeforeFirstLoop;
        long numLoops = numRocksToFallAfterFirstLoop / numRocksFallenEachLoop;
        long numRocksFallenFromLoops = (numLoops * numRocksFallenEachLoop);
        long numLeftOverToFall = (numRocksToFallAfterFirstLoop - numRocksFallenFromLoops);

        Console.Out.WriteLine("Num loops: " + numLoops);
        Console.Out.WriteLine("Num fallen each loop: " + numRocksFallenEachLoop);

        Console.Out.WriteLine();
        Console.Out.WriteLine("Num fallen before loops: " + numRocksFallenBeforeFirstLoop);
        Console.Out.WriteLine("Num rocks fallen from loops: " + numRocksFallenFromLoops);
        Console.Out.WriteLine("Num left over: " + numLeftOverToFall);

        Console.Out.WriteLine("Checksum: " + (numRocksFallenBeforeFirstLoop + numLeftOverToFall + numRocksFallenFromLoops));

        Console.Out.WriteLine($"Height after final loop: {heightBeforeFirstLoop + (numLoops * heightGainedEachLoop)}");

        var startMeasuringAt = numRocksFallenBeforeFirstLoop + numRocksFallenEachLoop;
        var endMeasuringAt = startMeasuringAt + numLeftOverToFall;
        var expectedHeightAtMeasureStart = heightBeforeFirstLoop + heightGainedEachLoop;
        Console.Out.WriteLine($"For safety, measure height after {startMeasuringAt}");
        Console.Out.WriteLine("The height then better be " + expectedHeightAtMeasureStart);
        Console.Out.WriteLine($"Then, measure {numLeftOverToFall} more, up to {endMeasuringAt}");

        var data = File.ReadAllText(example ? "day17Example" : "day17Input");
        var jets = data.Select(t => t == '>').ToArray(); // true: push right
        
        var chute = new List<Space[]>();
        chute.Add(LineOf(Rock));

        var rockOrder = BuildRockOrder();

        var topOfTopmostRock = 0;
        var jetIndex = 0;
        var nextRockToFall = 0;
        //
        var loopAtJetIndex = new List<int>();
        int heightLastTime = 0;
        int numRocksLastTime = 0;
        int numRocksFallen = 0;

        int heightAfterStartToMeasure = 0;
        
        var startTime = DateTime.Now;
        for (int num = 0; num < endMeasuringAt; num++) {
            var rock = rockOrder[nextRockToFall];
            nextRockToFall = (nextRockToFall + 1) % rockOrder.Count;

            var fallFrom = new Point(2, topOfTopmostRock + 4);
            
            // DrawChute(chute, rock, fallFrom);
            var before = jetIndex;
            MakeRockFall(chute, rock, fallFrom, jets, ref jetIndex, out var topOfLastFallenRock);
            numRocksFallen++;
            // DrawChute(chute);

            if (topOfLastFallenRock > topOfTopmostRock)
                topOfTopmostRock = topOfLastFallenRock;

            if (num == 2021) {
                Console.Out.WriteLine($"Puzzle 1: {topOfTopmostRock}");
            }

            // Console.Out.WriteLine(topOfTopmostRock);
            
            // if (num == numRocksFallenBeforeFirstLoop - 1) {
            //     Console.Out.WriteLine($"{num}: Should be {heightBeforeFirstLoop}, is {topOfTopmostRock}");
            //     Console.In.ReadLine();
            // }

            if (num == startMeasuringAt - 1) {
                // Console.Out.WriteLine("expecting it to be " + expectedHeightAtMeasureStart);
                heightAfterStartToMeasure = topOfTopmostRock;
                if (heightAfterStartToMeasure != expectedHeightAtMeasureStart)
                    throw new();
                // else
                    // Console.Out.WriteLine("Is good!");
            }

            // if (jetIndex == 9) {
            //     var growthInHeight = topOfTopmostRock - heightLastTime;
            //     var growthInNumRocks = numRocksFallen - numRocksLastTime;
            //     heightLastTime = topOfTopmostRock;
            //     numRocksLastTime = numRocksFallen;
            //     Console.Out.WriteLine($"At jet index 9. Num fallen rocks is {numRocksFallen}. " +
            //                           $"Top of fallen rock is: {topOfTopmostRock}. Growth since last is: {growthInHeight}. Increase in rocks fallen is: {growthInNumRocks}");
            // }

            // if (jetIndex < before) {
            //     // Console.Out.WriteLine("jetIndex " + jetIndex);
            //     loopAtJetIndex.Add(jetIndex);
            //     var foundALoopAfter = DoTheTest(loopAtJetIndex);
            //     
            //     if (foundALoopAfter.HasValue)
            //         return;
            //
            //     // DrawChute(chute, null, default, false, topOfFallenRock, topOfFallenRock);
            //
            //     Console.Out.WriteLine("Continue?");
            //     if (Console.In.ReadLine() == "stop")
            //         break;
            //
            //     // To go fast, need to find repeating pattern. 
            //     // patterns will repeat at multiples of the number of jets.
            //     //
            //     // Every time we loop the jet, start looking backwards. 
            //     // When we find a line that's equal to the last added line, check if the sequence starting at that line and further back is equal to the sequence from
            //     // the current loop and back
            // }
        }

        var increase = topOfTopmostRock - heightAfterStartToMeasure;
        
        var endTime = DateTime.Now;
        var duration = (endTime - startTime);

        Console.Out.WriteLine($"It took {duration.TotalMilliseconds} milliseconds!");
        
        Console.Out.WriteLine($"It increased by {increase} between first and second loop");
        Console.Out.WriteLine($"So we expect the total (Puzzle 2) to be {increase + heightBeforeFirstLoop + (numLoops * heightGainedEachLoop)}");

        Console.Out.WriteLine("Just the number of loops times the height gained each loop is " + (numLoops * heightGainedEachLoop));

        var durationOfAllRocks = (1000000000000 / 2022f) * duration;
        Console.Out.WriteLine($"Puzzle 2 should take {durationOfAllRocks.TotalSeconds} seconds if we brute it");
        // Console.Out.WriteLine(1000000000000);
        // Console.Out.WriteLine(1514285714288);
        // Console.Out.WriteLine(int.MaxValue);
        // //
        // int? allEqualLength = null;
        // int? allEqualAt = null;
        //
        // for (int length = 5; length < chute.Count; length++) {
        //     for (int j = 100 + length; j < chute.Count; j++) {
        //         var allEqual = true;
        //         for (int i = 0; i < length; i++) {
        //             if (!EqualLines(chute[100 + i], chute[j + i])) {
        //                 allEqual = false;
        //                 break;
        //             }
        //         }
        //
        //         if (allEqual) {
        //             allEqualAt = j;
        //             allEqualLength = length;
        //             goto end;
        //         }
        //     }
        // }
        //
        // end:
        //
        // if (allEqualAt is int atSpot) {
        //     Console.Out.WriteLine($"All equal at {atSpot}, length of repear is {allEqualLength}");
        //     Console.Out.WriteLine("Start: ");
        //
        //     DrawChute(chute, null, default, false, 105, 95);
        //     Console.Out.WriteLine("at " + atSpot);
        //     DrawChute(chute, null, default, false, atSpot + 5, atSpot - 5);
        // }
    }

    private static int? DoTheTest(List<int> loopAtJetIndex) {
        Console.Out.WriteLine("testing " + string.Join(", ", loopAtJetIndex));
        if (loopAtJetIndex.Count <= 3) {
            Console.Out.WriteLine("Too short list");
            return null;
        }

        var jetIndex = loopAtJetIndex[^1];
        for (int i = loopAtJetIndex.Count - 3; i >= (loopAtJetIndex.Count / 2) - 1; i--) {
            if (loopAtJetIndex[i] == jetIndex) {
                // Console.Out.WriteLine($"Potential loop at index {i}");
                
                // Go backwards from both jetIndex and here, check if always the same

                var numBackToPotentialLoop = loopAtJetIndex.Count - i - 1;
                var allEqual = true;
                for (int j = 0; j < numBackToPotentialLoop - 1; j++) {
                    var fromFrontIndex = loopAtJetIndex.Count - 2 - j;
                    var fromBackIndex = i - 1 - j;
                    
                    var fromFront = loopAtJetIndex[fromFrontIndex];
                    var fromBack = loopAtJetIndex[fromBackIndex];

                    // Console.Out.WriteLine($"Testing if indices {fromFrontIndex} and {fromBackIndex} has the same values. Values are {fromFront} and {fromBack}");

                    if (fromFront != fromBack) {
                        // Console.Out.WriteLine("Nope, bailing!");
                        allEqual = false;
                        break;
                    }
                }

                if (allEqual) {
                    Console.Out.WriteLine($"  Found loop of length {numBackToPotentialLoop}");
                    Console.Out.WriteLine("  It is: ");
                    Console.Out.WriteLine("  " + string.Join(", ", loopAtJetIndex.Select(i => i.ToString()).ToArray(), i + 1, numBackToPotentialLoop));
                    return numBackToPotentialLoop;
                }
            }
        }

        Console.Out.WriteLine("no loop!");
        return null;
    }

    private static bool EqualLines(Space[] spaces, Space[] spaces1) {
        for (int i = 0; i < chuteWidth; i++)
            if (spaces[i] != spaces1[i])
                return false;
        return true;
    }

    private static void DrawChute(List<Space[]> chute, Space[,]? rock = null, Point rockAtPoint = default, bool waitForInput = false, int? from = null, int? to = null) {
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
            if (y > from || y < to)
                continue;
            
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