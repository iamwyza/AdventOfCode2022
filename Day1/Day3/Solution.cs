namespace AdventOfCode2022.Day3;
internal static class Solution
{
  public static async Task RunPart1()
    {
        Console.WriteLine("Day 3 - Part 1:");

        // ascii A = 64, a = 97
        // offset lowercase by -96, offset uppercase by -38 so that a = 1, and A = 27
        var lines = await File.ReadAllLinesAsync("Day3\\day3input.txt");

        var priorityTotal = 0;

        foreach (var line in lines)
        {
            var container1 = line[..(line.Length / 2)];
            var container2 = line[(line.Length / 2)..];
            priorityTotal += container1.Where(item => container2.Contains(item)).Distinct().Sum(item => item >= 97 ? item - 96 : item - 38);
        }

        Console.WriteLine($"Priority Total = {priorityTotal}");
    }

    public static async Task RunPart2()
    {
        Console.WriteLine("Day 3 - Part 2:");

        var lines = await File.ReadAllLinesAsync("Day3\\day3input.txt");
        var priorityTotal = 0;

        for (int i = 0; i < lines.Length; i+=3)
        {
            priorityTotal += lines[i].Where(item => lines[i+1].Contains(item) && lines[i+2].Contains(item)).Distinct().Sum(item => item >= 97 ? item - 96 : item - 38);
        }

        Console.WriteLine($"Priority Total = {priorityTotal}");
    }
}
