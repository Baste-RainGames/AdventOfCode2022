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
}