namespace AdventOfCode2022;

public static class Day13 {
    public static void Run(bool example) {
        var data = File.ReadAllLines(example ? "day13Example" : "day13Input");

        var index = 1;
        var indexSum = 0;
        
        for (int i = 0; i < data.Length;) {
            var leftInput = data[i++];
            var rightInput = data[i++];

            var listA = ReadList(leftInput);
            var listB = ReadList(rightInput);

            // Console.Out.WriteLine($"== Pair {index} ==");
            var comparison = Compare(listA, listB);
            // Console.Out.WriteLine();
            
            var correct = comparison < 0;
            if (correct) 
                indexSum += index;

            i++;
            index++;
        }

        Console.Out.WriteLine($"Puzzle 1: {indexSum}");

        var dp1 = ReadList("[[2]]");
        var dp2 = ReadList("[[6]]");
        
        var allLists = new List<DataList>();

        for (int i = 0; i < data.Length;) {
            var leftInput = data[i++];
            var rightInput = data[i++];

            allLists.Add(ReadList(leftInput));
            allLists.Add(ReadList(rightInput));
            i++;
        }

        allLists.Add(dp1);
        allLists.Add(dp2);

        allLists.Sort(Compare);

        // foreach (var list in allLists) {
        //     Console.Out.WriteLine(list);
        // }

        var index1 = allLists.IndexOf(dp1) + 1;
        var index2 = allLists.IndexOf(dp2) + 1;
        Console.Out.WriteLine($"Puzzle 2: {index1 * index2}");
    }

    private struct Token {
        public bool startList;
        public bool endList;
        public int value;

        public override string ToString() {
            return startList ? "[" : endList ? "]" : value.ToString();
        }
    }
    
    private static DataList ReadList(string leftInput) {
        var tokens = Tokenize(leftInput);

        if (!tokens[0].startList || !tokens[^1].endList)
            throw new();
        
        tokens.RemoveAt(0);
        tokens.RemoveAt(tokens.Count - 1);

        var outerList = new DataList();
        var stack = new Stack<DataList>();
        stack.Push(outerList);
        
        foreach (var token in tokens) {
            if (token.startList) {
                var newList = new DataList();
                stack.Peek().list.Add(newList);
                stack.Push(newList);
            }
            else if (token.endList) {
                stack.Pop();
            }
            else {
                stack.Peek().list.Add(new Data { value = token.value});
            }
        }

        return outerList;
    }

    private static List<Token> Tokenize(string leftInput) {
        var tokens = new List<Token>();

        for (var i = 0; i < leftInput.Length; i++) {
            var data = leftInput[i];
            if (data == '[')
                tokens.Add(new Token { startList = true });
            else if (data == ']')
                tokens.Add(new Token { endList = true });
            else if (data is >= '0' and <= '9') {
                var integer = data - '0';
                var lookAhead = leftInput[i + 1];
                if (lookAhead is >= '0' and <= '9') {
                    // support just 0 - 99
                    tokens.Add(new Token { value = 10 * integer + (lookAhead - '0') });
                    i++;
                }
                else
                    tokens.Add(new Token { value = integer });
            }
        }

        return tokens;
    }

    private static int Compare(DataList listA, DataList listB) => Compare(listA, listB, 0, 0, false, false);
    private static int Compare(DataList listA, DataList listB, int listIndex, int debugIndent, bool printList = true, bool printAnything = false) {
        var indentStr = "";
        for (int i = 0; i < debugIndent; i++) 
            indentStr += "  ";

        if (printList)
            Write($"{indentStr}- Compare {listA} vs {listB}");

        if (listA.list.Count <= listIndex && listB.list.Count > listIndex) {
            Write(indentStr + "  - Left side ran out of items, so inputs are in the right order");
            return -1;
        }
        if (listB.list.Count <= listIndex && listA.list.Count > listIndex) {
            Write(indentStr + "  - Right side ran out of items, so inputs are not in the right order");
            return 1;
        }
        if (listA.list.Count == listIndex && listB.list.Count == listIndex) {
            return 0;
        }

        var aElement = listA.list[listIndex];
        var bElement = listB.list[listIndex];

        if (aElement is Data dataA && bElement is Data dataB) {
            Write($"{indentStr}  - Compare {dataA} vs {dataB}");
            
            if (dataA.value < dataB.value) {
                Write(indentStr + "  - Left side is smaller, so inputs are in the right order");
                return -1;
            }

            if (dataB.value < dataA.value) {
                Write(indentStr + "  - Right side is smaller, so inputs are not in the right order");
                return 1;
            }

            return Compare(listA, listB, listIndex + 1, debugIndent, false);
        }

        var dataListA = aElement as DataList;
        var dataListB = bElement as DataList;

        if (dataListA == null) {
            Write(indentStr + $"- Mixed types; convert left to {dataListA} and retry comparison");
            dataListA = new DataList { list = new() { (Data) aElement } };
        }

        if (dataListB == null) {
            Write(indentStr + $"- Mixed types; convert right to {dataListB} and retry comparison");
            dataListB = new DataList { list = new() { (Data) bElement } };
        }

        var innerCompare = Compare(dataListA, dataListB, 0, debugIndent + 1);
        if (innerCompare != 0)
            return innerCompare;
        return Compare(listA, listB, listIndex + 1, debugIndent);

        void Write(string str)  {
            if (printAnything)
                Console.Out.WriteLine(str);
        }
    }

    public abstract class DataEntry {
        
    }

    public class DataList : DataEntry {
        public List<DataEntry> list = new();

        public override string ToString() {
            return $"[{string.Join(',', list)}]";
        }
    }

    public class Data : DataEntry {
        public int value;

        public override string ToString() {
            return value.ToString();
        }
    }
}