namespace AdventOfCode2022;

public static class Util {
    public static bool TryTrimStart(string str, string start, out string remainder) {
        if (str.StartsWith(start, StringComparison.Ordinal)) {
            remainder = str.Substring(start.Length);
            return true;
        }

        remainder = null;
        return false;
    }

    public static int ParseInOnEndOfLineStartingWith(string text, string start) {
        if (!TryTrimStart(text, start, out var remainder))
            throw new Exception($"text \"{text}\" did not start with \"{start}\"");

        if (!int.TryParse(remainder, out var intVal))
            throw new Exception($"Could not parse {remainder} to int");

        return intVal;
    }
}