namespace AdventOfCode2022.Day01;

internal class Day01 : DayBase
{
    public override async Task RunPart1()
    {
        PrintStart(1);
        
        int runningTotal = 0;
        int max = 0;

        foreach (var line in await GetLines())
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

    public override async Task RunPart2()
    {
        PrintStart(2);
        Dictionary<int, int> totals = new() { { 0, 0 } };
        int elfIndex = 0;
        foreach (var line in await GetLines())
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
