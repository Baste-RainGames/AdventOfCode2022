namespace AdventOfCode2022;

public static class Day11 {
    public static void Run(bool example) {
        var data = File.ReadAllLines(example ? "day11Example" : "day11Input");

        Console.Out.WriteLine($"Puzzle 1: Monkey business is {DoDay(data, true,  20)}");
        Console.Out.WriteLine($"Puzzle 2: Monkey business is {DoDay(data, false, 10000)}");
    }

    private static long DoDay(string[] data, bool worryLowers, int numRounds) {
        var monkeys = new List<Monkey>();
        var throwToOnTrue = new Dictionary<Monkey, int>();
        var throwToOnFalse = new Dictionary<Monkey, int>();

        var index = 0;
        while (index < data.Length) {
            var monkey = new Monkey(monkeys.Count);
            monkeys.Add(monkey);
            index++; // skip monkey name

            foreach (var item in FindMonkeyItems(data, index++))
                monkey.items.Enqueue(item);
            monkey.operation = FindOperation(data, index++);
            monkey.divisibleBy = Util.ParseInOnEndOfLineStartingWith(data[index++], "  Test: divisible by ");
            throwToOnTrue[monkey] = Util.ParseInOnEndOfLineStartingWith(data[index++], "    If true: throw to monkey ");
            throwToOnFalse[monkey] = Util.ParseInOnEndOfLineStartingWith(data[index++], "    If false: throw to monkey ");

            index++; // skip empty line between monkeys
        }

        foreach (var monkey in monkeys) {
            monkey.throwToOnTrue = monkeys[throwToOnTrue[monkey]];
            monkey.throwToOnFalse = monkeys[throwToOnFalse[monkey]];
        }

        var divisors = monkeys.Select(monkey => monkey.divisibleBy).Distinct();
        var divisorProduct = 1L;
        foreach (var divisor in divisors) {
            divisorProduct *= divisor;
        }

        for (int i = 1; i <= numRounds; i++) {
            var debug = i == 1 || i == 20 || (i % 1000) == 0;

            DoRound(i, debug ? 1 : 0, monkeys, worryLowers, divisorProduct);
        }

        monkeys.Sort((m0, m1) => m1.numInspections.CompareTo(m0.numInspections));

        return monkeys[0].numInspections * monkeys[1].numInspections;

        static void DoRound(int round, int debugLevel, List<Monkey> monkeys, bool worryLowers, long modBy) {
            if (debugLevel >= 2)
                Console.Out.WriteLine("Round " + round);
            
            foreach (var monkey in monkeys)
                DoTurn(monkey, debugLevel, worryLowers, modBy);

            if (debugLevel >= 1) {
                Console.Out.WriteLine($"After round {round}, items are:");
                foreach (var monkey in monkeys)
                    Console.Out.WriteLine($"Monkey {monkey.monkeyIndex}  Items: {string.Join(", ", monkey.items.Select(item => item.worryLevel))}");
                
                Console.Out.WriteLine($"After round {round}, inspections are:");
                foreach (var monkey in monkeys)
                    Console.Out.WriteLine($"Monkey {monkey.monkeyIndex}  Inspections: {monkey.numInspections}");
            }
        }

        static void DoTurn(Monkey monkey, int debugLevel, bool worryLowers, long modBy) {
            if (monkey.items.Count == 0) {
                if (debugLevel >= 2)
                    Console.Out.WriteLine($"Monkey {monkey.monkeyIndex} has no items");
                return;
            }

            if (debugLevel >= 2)
                Console.Out.WriteLine($"Monkey {monkey.monkeyIndex}:");
            while (monkey.items.Count > 0) {
                var item = monkey.items.Dequeue();
                monkey.numInspections++;
                if (debugLevel >= 2)
                    Console.Out.WriteLine($"  Monkey inspects an item with a worry level of {item.worryLevel}.");

                item.worryLevel = monkey.operation.Result(item);
                if (debugLevel >= 2)
                    Console.Out.WriteLine($"    Worry level is {monkey.operation.OpName} by {monkey.operation.OperandBName} to {item.worryLevel}.");

                item.worryLevel %= modBy;

                if (worryLowers) {
                    item.worryLevel /= 3;
                    if (debugLevel >= 2)
                        Console.Out.WriteLine($"    Monkey gets bored with item. Worry level is divided by 3 to {item.worryLevel}.");
                }

                var isDivisible = (item.worryLevel % monkey.divisibleBy) == 0;
                if (debugLevel >= 2)
                    Console.Out.WriteLine($"    Current worry level is {(isDivisible ? "divisible" : "not divisible")} by {monkey.divisibleBy}");

                var throwTo = isDivisible ? monkey.throwToOnTrue : monkey.throwToOnFalse;
                if (debugLevel >= 2)
                    Console.Out.WriteLine($"    Item with worry level {item.worryLevel} is thrown to monkey {throwTo.monkeyIndex}.");

                throwTo.items.Enqueue(item);
            }
        }
    }

    private static void PrintMonkey(Monkey monkey) {
        Console.Out.WriteLine($"Monkey {monkey.monkeyIndex}");
        Console.Out.WriteLine($"  Items: {string.Join(", ", monkey.items.Select(item => item.worryLevel))}");
        Console.Out.WriteLine($"  Operation: {monkey.operation}");
        Console.Out.WriteLine($"  Test: divisible by {monkey.divisibleBy}");
        Console.Out.WriteLine($"    If true, throw to {monkey.throwToOnTrue.monkeyIndex}");
        Console.Out.WriteLine($"    If false, throw to {monkey.throwToOnFalse.monkeyIndex}");
    }

    private static Operation FindOperation(string[] data, int index) {
        var opLine = data[index];
        if (!Util.TryTrimStart(opLine, "  Operation: new = ", out var opData))
            throw new Exception("Wrong input " + opLine);

        return new Operation(opData.Split());
    }
    
    private static IEnumerable<Item> FindMonkeyItems(string[] data, int index) {
        var itemLine = data[index];
        if (!Util.TryTrimStart(itemLine, "  Starting items: ", out var itemData))
            throw new Exception("Wrong input " + itemLine);
        return itemData.Split(", ").Select(long.Parse).Select(worry => new Item { worryLevel = worry });
    }

    public class Monkey {
        public Queue<Item> items = new();
        public Operation operation;
        public long divisibleBy;
        public Monkey throwToOnTrue;
        public Monkey throwToOnFalse;

        public long numInspections;
        public readonly int monkeyIndex;

        public Monkey(int monkeyIndex) {
            this.monkeyIndex = monkeyIndex;
        }
    }

    public class Item {
        public long worryLevel;
    }

    public class Operation {
        private Operand operandA;
        private Operand operandB;
        private Op op;
        
        private enum Op {
            Pluss,
            Multiply,
        }
        
        public Operation(string[] parts) {
            operandA = new Operand(parts[0]);
            operandB = new Operand(parts[2]);
            op = parts[1] switch {
                "+" => Op.Pluss,
                "*" => Op.Multiply,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public string OpName {
            get {
                return op switch {
                    Op.Pluss => "+" ,
                    Op.Multiply => "*" ,
                    _ => throw new ArgumentOutOfRangeException(nameof(op), op, $"Unknown op {op}")
                };
            }
        }

        public string OperandBName => operandB.ToString("itself");

        public long Result(Item item) {
            var a = operandA.GetValue(item);
            var b = operandB.GetValue(item);

            return op switch {
                Op.Pluss    => checked(a + b),
                Op.Multiply => checked(a * b),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public override string ToString() {
            return operandA.ToString("old") + (op == Op.Multiply ? " * " : " + ") + operandB.ToString("old");
        }
    }

    public struct Operand {
        private long? literal;

        public Operand(string op) {
            if (op == "old")
                literal = null;
            else if (long.TryParse(op, out var v))
                literal = v;
            else throw new Exception(op);
        }

        public long GetValue(Item item) => literal ?? item.worryLevel;

        public override string ToString() {
            return literal.HasValue ? literal.Value.ToString() : "old";
        }

        public string ToString(string notLiteralName) {
            return literal.HasValue ? literal.Value.ToString() : notLiteralName;
        }
    }
}