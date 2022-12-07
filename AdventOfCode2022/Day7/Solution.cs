using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic.CompilerServices;

namespace AdventOfCode2022.Day7;
internal class Solution : DayBase
{
    private Dir _root = new Dir { Name = "/" };

    private void PopulateFileSystem(string[] lines)
    {
        var currentDir = _root;

        foreach (var line in lines)
        {
            if (line[0] == '$') // Command
            {
                var parts = line.Split(" ");
                if (parts[1] == "cd")
                {
                    switch (parts[2])
                    {
                        case "..": currentDir = currentDir.ParentDir;
                            break;
                        case "/": currentDir = _root;
                            break;
                        default:
                            currentDir.Directories.TryAdd(parts[2], new Dir(currentDir) { Name = parts[2] });
                            currentDir = currentDir.Directories[parts[2]];
                            break;
                    }
                }
            }
            else if (line.StartsWith("dir")) // Directory
            {
                var parts = line.Split(" ");
                currentDir.Directories.TryAdd(parts[1], new Dir(currentDir) { Name = parts[1] });
            }
            else // File
            {
                var parts = line.Split(" ");
                currentDir.Files.Add(parts[1], double.Parse(parts[0]));
            }
        }
    }

    public override async Task RunPart1()
    {
        PrintStart(1);
        var lines = await GetLines();

        PopulateFileSystem(lines);
        Console.WriteLine(JsonSerializer.Serialize(_root, new JsonSerializerOptions { WriteIndented = true }));
        double total = 0;
        _root.Directories.Select(x => GetFilesLessThan(x.Value, 100_000, 1, ref total)).Sum();
        Console.WriteLine($"Sum total of files with size of files less 100_000: {total}");
    }

    private double GetFilesLessThan(Dir directory, double size, int depth, ref double total)
    {
        string indent = "".PadLeft(depth*2);
        double temp = 0;
        Console.WriteLine("SIZE=" + size);
        Console.WriteLine(indent + directory.Name + "/");

        foreach (var dir in directory.Directories)
        {
            Console.WriteLine("DIRLOOP:" + dir.Key);
            temp += GetFilesLessThan(dir.Value, size, ++depth, ref total);
            Console.WriteLine("Total after Dir: " + total);
        }

        foreach (var file in directory.Files)
        {
            Console.WriteLine($"{indent}  {file.Key} = {file.Value}");
            temp += file.Value;
            Console.WriteLine("Total after file=" + total);
        }

        Console.WriteLine("TOTAL: " + total);

        total += temp > size ? 0 : temp;
        return temp;
    }


    private double SetDirectorySizes(Dir directory, int depth)
    {
        string indent = "".PadLeft(depth * 2);
        double temp = 0;

        foreach (var dir in directory.Directories)
        {
            temp += SetDirectorySizes(dir.Value, ++depth);
        }

        foreach (var file in directory.Files)
        {
            temp += file.Value;
        }

        directory.Size = temp;

        return temp;
    }

    private double GetSmallestDirectoryBiggerThanSize(Dir directory, double size, ref double smallest)
    {
        foreach (var dir in directory.Directories)
        {
            double temp = GetSmallestDirectoryBiggerThanSize(dir.Value, size, ref smallest);

            if (temp >= size && temp < smallest)
            {
                smallest = temp;
            }
        }

        return directory.Size;

    }


    public override async Task RunPart2()
    {
        PrintStart(2);
        var lines = await GetLines();

        PopulateFileSystem(lines);
        double smallest = double.MaxValue;
        double totalAmount = 70000000;
        
        SetDirectorySizes(_root, 0);
        double needed = (totalAmount - _root.Size - 30000000) * -1;

        GetSmallestDirectoryBiggerThanSize(_root, needed, ref smallest);
        //File.WriteAllText("out.json", JsonSerializer.Serialize(_root, new JsonSerializerOptions { WriteIndented = true }));

        Console.WriteLine($"Smallest directory that can be removed that is at least {needed}: {smallest}");

    }

    public class Dir
    {
        public string Name { get; set; }
        public Dictionary<string, double> Files { get; set; } = new();
        public Dictionary<string, Dir> Directories { get; set; } = new();

        public double Size { get; set; }

        [JsonIgnore] 
        public Dir? ParentDir { get; }

        public Dir(Dir parentDir)
        {
            ParentDir = parentDir;
        }

        public Dir() {}
    }
}
