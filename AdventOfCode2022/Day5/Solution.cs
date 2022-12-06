using System.Text.RegularExpressions;

namespace AdventOfCode2022.Day5;
internal class Solution : DayBase
{
    private readonly Regex _moveRegex = new Regex(@"move (\d+) from (\d+) to (\d+)", RegexOptions.Compiled);

    private static (List<char>[] crates, int startMoveLine) GetCrates(string[] lines)
    {
        Regex letterRegex = new Regex(@"\w", RegexOptions.Compiled);

        List<char>[] crates = new List<char>[10];
        for (var i = 0; i < 10; i++)
        {
            crates[i] = new List<char>();
        }

        
        int startMoveLine = 0;

        // Get starting crate layout - this leaves the order of the crates bottom to top
        foreach (var line in lines)
        {

            if (line[1] == '1')
            {
                startMoveLine += 2;
                break;
            }

            startMoveLine++;


            for (int i = 1; i < line.Length; i += 4)
            {
                if (letterRegex.IsMatch(line[i].ToString()))
                {
                    crates[i / 4].Add(line[i]);
                }
            }
        }

        // now make the crates top to bottom
        foreach (var crate in crates)
        {
            crate.Reverse();
        }

        return (crates, startMoveLine);
    }

    public override async Task RunPart1()
    {
        PrintStart(1);
        var lines = await GetLines();

        var (crates, startMoveLine) = GetCrates(lines);
        
        foreach (var line in lines.Skip(startMoveLine))
        {
            var match = _moveRegex.Match(line);
            var count = int.Parse(match.Groups[1].Value);
            var from = int.Parse(match.Groups[2].Value)-1; // crates are 0 indexed, so we need to offset what is in the file
            var to = int.Parse(match.Groups[3].Value)-1;

            for (int i = 0; i < count; i++)
            {
                //PrintState(crates);
                crates[to].Add(crates[from].Last());
                crates[from].RemoveAt(crates[from].Count-1);
            }

        }

        PrintState(crates);
        Console.WriteLine(new string(crates.Where(y => y.Any()).Select(x => x.Last()).ToArray()));
    }

    public override async Task RunPart2()
    {
        PrintStart(2);


        var lines = await GetLines();

        var (crates, startMoveLine) = GetCrates(lines);

        foreach (var line in lines.Skip(startMoveLine))
        {
            //Console.WriteLine(line);
            var match = _moveRegex.Match(line);
            var count = int.Parse(match.Groups[1].Value);
            var from = int.Parse(match.Groups[2].Value) - 1; // crates are 0 indexed, so we need to offset what is in the file
            var to = int.Parse(match.Groups[3].Value) - 1;

            //PrintState(crates);
            crates[to].AddRange(crates[from].GetRange(crates[from].Count-count, count));
            crates[from].RemoveRange(crates[from].Count - count, count);

        }

        PrintState(crates);

        Console.WriteLine(new string(crates.Where(y => y.Any()).Select(x => x.Last()).ToArray()));
    }

    private static void PrintState(List<char>[] crates)
    {
        var maxHeight = crates.Max(y => y.Count);

        for (int level = maxHeight; level >= 0; level--)
        {
            for (int i = 0; i < crates.Length; i++)
            {
                if (crates[i].Count > level)
                {
                    Console.Write($"[{crates[i][level]}] ");
                }
                else
                {
                    Console.Write("    ");
                }
            }
            Console.WriteLine();
        }

        for (int i = 0; i < crates.Length; i++)
        {
            Console.Write($" {i}  ");
        }
        Console.WriteLine();
    }
}
