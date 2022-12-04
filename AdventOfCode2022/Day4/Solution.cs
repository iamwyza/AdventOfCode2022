namespace AdventOfCode2022.Day4;
internal class Solution : DayBase
{
    public override async Task RunPart1()
    {
        PrintStart(1);

        var total = 0;

        foreach (var line in await GetLines())
        {
            var ranges = line.Split(',');
            var elf1 = ranges[0].Split('-').Select(int.Parse).ToArray();
            var elf2 = ranges[1].Split('-').Select(int.Parse).ToArray();

            if ((elf1[0] >= elf2[0] && elf1[1] <= elf2[1]) ||
                (elf2[0] >= elf1[0] && elf2[1] <= elf1[1]))
            {
                total++;
            }
        }

        Console.WriteLine($"Total number of elves completely duplicated = {total}");
    }

    public override async Task RunPart2()
    {
        PrintStart(2);

        var total = 0;

        foreach (var line in await GetLines())
        {
            var ranges = line.Split(',');
            var elf1 = ranges[0].Split('-').Select(int.Parse).ToArray();
            var elf2 = ranges[1].Split('-').Select(int.Parse).ToArray();

            if (
                (elf1[0] >= elf2[0] && elf1[0] <= elf2[1]) ||
                (elf1[1] >= elf2[0] && elf1[1] <= elf2[1]) ||
                (elf2[0] >= elf1[0] && elf2[0] <= elf1[1]) ||
                (elf2[1] >= elf1[0] && elf2[1] <= elf1[1]))
            {
                total++;
            }
        }

        Console.WriteLine($"Total number of elves partially duplicated = {total}");
    }
}
