DayBase.ScanForSolutions();

var choice = "";

string? last = null;


while (choice != "0")
{
    var prompt = new SelectionPrompt<string>()
        .Title(
            "Enter Day and Part to run.  Example: Day 1 - Part 1 would be 1-1.  Day 3 - Part 2 would be 3-2.  Enter 0 for exit;?")
        .PageSize(4)
        .MoreChoicesText("[grey](Move up and down to reveal more fruits)[/]");
    if (!string.IsNullOrEmpty(last))
    {
        prompt.AddChoice(last);
    }
    prompt.AddChoices("0")
    .AddChoices(DayBase.Days.Keys.OrderByDescending(x => x)
        .SelectMany(x => new string[] { x + "-" + 1, x + "-" + 2 }).ToArray());


    choice = AnsiConsole.Prompt(
        prompt
    );

    if (choice == "0")
        break;

    AnsiConsole.Clear();

    var input = choice?.Split('-', 2).Select(int.Parse).ToArray();

    if (input![1] == 1)
    {
        await DayBase.Days[input[0]].RunPart1();
    }
    else
    {
        await DayBase.Days[input[0]].RunPart2();
    }

    Console.WriteLine();

    last = choice;
}