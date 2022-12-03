namespace AdventOfCode2022.Day1;

internal static class Solution
{
    public static async Task RunPart1()
    {
        Console.WriteLine("Day 1 - Part 1:");
        var lines = await File.ReadAllLinesAsync("day1input.txt");

        // part1
        int runningTotal = 0;
        int max = 0;

        foreach (var line in lines)
        {
            if (string.IsNullOrEmpty(line))
            {
                if (runningTotal > max)
                {
                    max = runningTotal;
                }

                runningTotal = 0;
                continue;
            }

            runningTotal += int.Parse(line);
        }

        Console.WriteLine($"Part 1: {max}");

       

    }

    public static async Task RunPart2()
    {
        Console.WriteLine("Day 1 - Part 2:");
        var lines = await File.ReadAllLinesAsync("day1input.txt");
        Dictionary<int, int> totals = new() { { 0, 0 } };
        int elfIndex = 0;
        foreach (var line in lines)
        {
            if (string.IsNullOrEmpty(line))
            {
                elfIndex++;
                totals.Add(elfIndex, 0);
                continue;
            }

            totals[elfIndex] += int.Parse(line);
        }

        int top3 = totals.OrderByDescending(x => x.Value).Take(3).Sum(y => y.Value);
        Console.WriteLine($"Part 2: {top3}");
    }
}
