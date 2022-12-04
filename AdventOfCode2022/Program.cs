using System.Text.RegularExpressions;

DayBase.ScanForSolutions();

Regex validationRegex = new Regex(@"^\d+-\d+", RegexOptions.Compiled);

Console.WriteLine("Enter Day and Part to run.  Example: Day 1 - Part 1 would be 1-1.  Day 3 - Part 2 would be 3-2.  Enter 0 for exit;");
PrintDays();

string? dayAndPartToRun = "";

while ((dayAndPartToRun = Console.ReadLine()) != "0")
{
    if (!validationRegex.IsMatch(dayAndPartToRun ?? ""))
    {
        Console.WriteLine("Invalid input.  Format: Day-Part.  Example: 1-1");
        continue;
    }

    var input = dayAndPartToRun?.Split('-', 2).Select(int.Parse).ToArray();
    if (input == null)
    {
        Console.WriteLine("Invalid input.  Format: Day-Part.  Example: 1-1");
        return;
    }

    if (input[1] == 1)
    {
        await DayBase.Days[input[1]].RunPart1();
    }
    else
    {
        await DayBase.Days[input[1]].RunPart2();
    }
    PrintDays();
}

void PrintDays()
{
    Console.WriteLine("\n\nAvailable Days:");

    foreach (int day in DayBase.Days.Keys.OrderBy(x => x))
    {
        Console.WriteLine($"\t{day}-1");
        Console.WriteLine($"\t{day}-2");
    }
    Console.WriteLine("\n 0 to Exit;");
}