namespace AdventOfCode2022;

public static class Day11 {
    public static void Run(bool example) {
        var data = File.ReadAllLines(example ? "day11Example" : "day11Input");

        var monkeys = new List<Monkey>();
        var throwToOnTrue = new Dictionary<Monkey, int>();
        var throwToOnFalse = new Dictionary<Monkey, int>();

        var index = 0;
        while (index < data.Length) {
            var monkey = new Monkey();
            monkeys.Add(monkey);

            foreach (var item in FindMonkeyItems(data, ++index))
                monkey.items.Enqueue(item);
            monkey.operation = FindOperation(data, ++index);
            monkey.divisibleBy = Util.ParseInOnEndOfLineStartingWith(data[++index], "  Test: divisible by ");
            throwToOnTrue[monkey] = Util.ParseInOnEndOfLineStartingWith(data[++index], "    If true: throw to monkey ");
            throwToOnFalse[monkey] = Util.ParseInOnEndOfLineStartingWith(data[++index], "    If false: throw to monkey ");

            ++index; // skip line
            ++index; // skip next monkey
        }

        foreach (var monkey in monkeys) {
            monkey.throwToOnTrue = monkeys[throwToOnTrue[monkey]];
            monkey.throwToOnFalse = monkeys[throwToOnFalse[monkey]];
        }

        foreach (var monkey in monkeys) {
            PrintMonkey(monkey);
            Console.Out.WriteLine();
        }
        
        void PrintMonkey(Monkey monkey) {
            Console.Out.WriteLine($"Monkey {monkeys.IndexOf(monkey)}");
            Console.Out.WriteLine("  Items: " + string.Join(", ", monkey.items.Select(item => item.worryLevel)));
            Console.Out.WriteLine("  Operation: " + monkey.operation);
            Console.Out.WriteLine("  Test: divisible by " + monkey.divisibleBy);
            Console.Out.WriteLine("    If true, throw to " + monkeys.IndexOf(monkey.throwToOnTrue));
            Console.Out.WriteLine("    If false, throw to " + monkeys.IndexOf(monkey.throwToOnFalse));
        }
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
        return itemData.Split(", ").Select(int.Parse).Select(worry => new Item { worryLevel = worry });
    }

    public class Monkey {
        public Queue<Item> items = new();
        public Operation operation;
        public int divisibleBy;
        public Monkey throwToOnTrue;
        public Monkey throwToOnFalse;
    }

    public class Item {
        public int worryLevel;
    }

    public class Operation {
        public Operation(string[] parts) {
            operandA = new Operand(parts[0]);
            operandB = new Operand(parts[2]);
            op = parts[1] switch {
                "+" => Op.Pluss,
                "*" => Op.Multiply,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public int Result(Item item) {
            var a = operandA.GetValue(item);
            var b = operandB.GetValue(item);

            return op switch {
                Op.Pluss    => a + b,
                Op.Multiply => a * b,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        
        private Operand operandA;
        private Operand operandB;
        private Op op;
        private enum Op {
            Pluss,
            Multiply,
        }

        public override string ToString() {
            return operandA + (op == Op.Multiply ? " * " : " + ") + operandB;
        }
    }

    public struct Operand {
        private int? literal;

        public Operand(string op) {
            if (op == "old")
                literal = null;
            else if (int.TryParse(op, out var v))
                literal = v;
            else throw new Exception(op);
        }

        public int GetValue(Item item) => literal ?? item.worryLevel;

        public override string ToString() {
            return literal.HasValue ? literal.Value.ToString() : "old";
        }
    }
}