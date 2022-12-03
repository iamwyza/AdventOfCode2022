Console.WriteLine("Enter Day and Part to run.  Example: Day 1 - Part 1 would be 1-1.  Day 3 - Part 2 would be 3-2.");
string? dayAndPartToRun = Console.ReadLine();
switch (dayAndPartToRun)
{
    case "1-1": await AdventOfCode2022.Day1.Solution.RunPart1(); break;
    case "1-2": await AdventOfCode2022.Day1.Solution.RunPart2(); break;
    case "2-1": await AdventOfCode2022.Day2.Solution.RunPart1(); break;
    case "2-2": await AdventOfCode2022.Day2.Solution.RunPart2(); break;
    case "3-1": await AdventOfCode2022.Day3.Solution.RunPart1(); break;
    case "3-2": await AdventOfCode2022.Day3.Solution.RunPart2(); break;
    default: Console.WriteLine($"{dayAndPartToRun} is not a valid combination."); break;
}