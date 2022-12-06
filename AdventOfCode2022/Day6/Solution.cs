using System.Text.RegularExpressions;

namespace AdventOfCode2022.Day6;
internal class Solution : DayBase
{
    private static int FindStart(string input, int length)
    {
        for (int i = length; i < input.Length; i++)
        {
            if (input[(i - length)..i].Distinct().Count() == length)
            {
                return i;
            }
        }

        return -1;
    }
    

    public override async Task RunPart1()
    {
        PrintStart(1);
        var lines = await GetLines();

        var start = FindStart(lines[0], 4);
       
        Console.WriteLine($"Packet starts at {start}");
    }

    public override async Task RunPart2()
    {
        PrintStart(2);

        var lines = await GetLines();

        var start = FindStart(lines[0], 14);

        Console.WriteLine($"Packet starts at {start}");


    }
}
