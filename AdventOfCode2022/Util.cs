namespace AdventOfCode2022;

public static class Util {
    public static bool TryTrimStart(string str, string start, out string remainder) {
        if (str.StartsWith(start, StringComparison.Ordinal)) {
            remainder = str.Substring(start.Length);
            return true;
        }

        remainder = string.Empty;
        return false;
    }

    public static int ParseInOnEndOfLineStartingWith(string text, string start) {
        if (!TryTrimStart(text, start, out var remainder))
            throw new Exception($"text \"{text}\" did not start with \"{start}\"");

        if (!int.TryParse(remainder, out var intVal))
            throw new Exception($"Could not parse {remainder} to int");

        return intVal;
    }

    public static T RemoveAndReturnFirst<T>(List<T> list) {
        var t = list[0];
        list.RemoveAt(0);
        return t;
    }
}

public record struct Point(int x, int y) {
    public static Point operator +(Point a, Point b) {
        return new Point(a.x + b.x, a.y + b.y);
    }
    public static Point operator -(Point a, Point b) {
        return new Point(a.x - b.x, a.y - b.y);
    }
}