namespace AdventOfCode2022;

public static class Day7 {
    public static void Run(bool example) {
        var data = File.ReadAllLines(example ? "day7Example" : "day7Input");

        var rootFolder = new ElfFolder("/", null);
        var allFolders = new List<ElfFolder> { rootFolder };
        
        var currentFolder = rootFolder;
        foreach (var line in data) {
            if (Util.TryTrimStart(line, "$ cd ", out var goToFolder)) {
                if (goToFolder == "/")
                    currentFolder = rootFolder;
                else if (goToFolder == "..")
                    currentFolder = currentFolder!.parentFolder;
                else
                    currentFolder = currentFolder!.subfolders.First(folder => folder.name == goToFolder); // assumption: folders are listed before entered
            }
            else if (!line.Equals("$ ls", StringComparison.Ordinal)) {
                if (Util.TryTrimStart(line, "dir ", out var dirName)) {
                    var folder = new ElfFolder(dirName, currentFolder);
                    currentFolder!.subfolders.Add(folder); // assumption: folders are only listed once!
                    allFolders.Add(folder);
                }
                else {
                    var parts = line.Split();
                    var size = int.Parse(parts[0]);
                    var name = parts[1];
                    currentFolder!.files.Add(new ElfFile(name, size));
                }
            }
        }

        var sumOfSizeOfFoldersSized100kOrLess = 0;
        foreach (var folder in allFolders)
            if (folder.Size <= 100000)
                sumOfSizeOfFoldersSized100kOrLess += folder.Size;

        Console.Out.WriteLine("Puzzle 1: " + sumOfSizeOfFoldersSized100kOrLess);

        var requiredSpace = 30000000;
        var diskSpace = 70000000;
        var freeSpace = diskSpace - rootFolder.Size;

        var mustBeFreed = requiredSpace - freeSpace;

        Console.Out.WriteLine($"Puzzle 2, need to free {mustBeFreed}");

        var smallestFolderLargerThanMustBeFreed = allFolders.Where(f => f.Size >= mustBeFreed).MinBy(f => f.Size);
        Console.Out.WriteLine($"Puzzle 2, delete folder {smallestFolderLargerThanMustBeFreed!.name} with size {smallestFolderLargerThanMustBeFreed.Size}");
    }

    private static void PrintFolderWithSubFolders(ElfFolder folder, int indents = 0) {
        WriteIndents();
        Console.Out.WriteLine($"- {folder.name} (dir)");

        indents++;
        foreach (var subfolder in folder.subfolders) 
            PrintFolderWithSubFolders(subfolder, indents);
        foreach (var file in folder.files) {
            WriteIndents();
            Console.Out.WriteLine($"- {file.name} (file, size={file.size})");
        }

        void WriteIndents() {
            for (int i = 0; i < indents; i++)
                Console.Out.Write("  ");
        }
    }



    public class ElfFolder {
        public readonly ElfFolder? parentFolder;
        public readonly string name;
        public readonly List<ElfFolder> subfolders = new();
        public readonly List<ElfFile> files = new();

        public ElfFolder(string name, ElfFolder? parentFolder) {
            this.name = name;
            this.parentFolder = parentFolder;
        }

        private int cachedSize = -1; 
        public int Size {
            get {
                if (cachedSize != -1)
                    return cachedSize;
                cachedSize = files.Sum(f => f.size) + subfolders.Sum(f => f.Size);
                return cachedSize;
            }
        }
    }

    public readonly record struct ElfFile(string name, int size);
}