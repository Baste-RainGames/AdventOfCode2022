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

    public static T RemoveAndReturnFirst<T>(List<T> list) {
        var t = list[0];
        list.RemoveAt(0);
        return t;
    }
}