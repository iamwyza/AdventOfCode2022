namespace AdventOfCode2022.Day3;
internal class Solution : DayBase
{
    public override async Task RunPart1()
    {
        PrintStart(1);

        // ascii A = 64, a = 97
        // offset lowercase by -96, offset uppercase by -38 so that a = 1, and A = 27
        var priorityTotal = 0;

        foreach (var line in await GetLines())
        {
            var container1 = line[..(line.Length / 2)];
            var container2 = line[(line.Length / 2)..];
            priorityTotal += container1.Where(item => container2.Contains(item)).Distinct().Sum(item => item >= 97 ? item - 96 : item - 38);
        }

        Console.WriteLine($"Priority Total = {priorityTotal}");
    }

    public override async Task RunPart2()
    {
        PrintStart(2);

        var lines = await GetLines();
        var priorityTotal = 0;

        for (int i = 0; i < lines.Length; i += 3)
        {
            priorityTotal += lines[i].Where(item => lines[i + 1].Contains(item) && lines[i + 2].Contains(item)).Distinct().Sum(item => item >= 97 ? item - 96 : item - 38);
        }

        Console.WriteLine($"Priority Total = {priorityTotal}");
    }
}
