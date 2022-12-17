namespace AdventOfCode2022.Day10
;
internal class Day10 : DayBase
{
    private List<int?> _instructions;
    private async Task Init()
    {
        var lines = await GetLines();
       _instructions = new List<int?>();
       foreach (var line in lines)
       {
           if (line == "noop")
           {
               _instructions.Add(null);
           }
           else
           {
               _instructions.Add(int.Parse(line.Split(" ")[1]));
           }
       }
    }

    public override async Task RunPart1()
    {
        PrintStart(1);
        await Init();

        int clock = 1;
        int signal = 1;
        int strength = 0;
        foreach (var instruction in _instructions)
        {
            Console.WriteLine($"Instruction = {instruction}");

            if (instruction != null)
            {
                Tick(ref clock, ref signal, ref strength);
                Tick(ref clock, ref signal, ref strength);
                signal += instruction.Value;
            }
            else
            {
                Tick(ref clock, ref signal, ref strength);
            }
        }

        Console.WriteLine($"{clock}: {signal}");

        Console.WriteLine($"Signal Strength: {strength}");

    }

    private void Tick(ref int clock, ref int signal, ref int strength)
    {

        Console.WriteLine($"{clock}: {signal}");
        if (clock == 0 || (clock + 20) % 40 == 0)
        {
            Console.WriteLine($"Strength at {clock} = {strength + (signal * clock)} ({strength} + ({signal * clock}))");
            strength += signal * clock;
        }
        clock++;
    }

    public override async Task RunPart2()
    {
        PrintStart(2);
        await Init();
        row = "";
        int clock = 1;
        int signal = 1;
        foreach (var instruction in _instructions)
        {
            //Console.WriteLine($"Instruction = {instruction}");

            if (instruction != null)
            {
                Tick2(ref clock, ref signal);
                Tick2(ref clock, ref signal);
                signal += instruction.Value;
            }
            else
            {
                Tick2(ref clock, ref signal);
            }

            //if (clock > 40) break;
        }

//
    }

    private string row = "";

    private void Tick2(ref int clock, ref int signal)
    {
        if (false) // Debugging 
        {
            Console.WriteLine($"Start cycle   {clock,-4}:");
            Console.WriteLine($"During cycle  {clock,-4}: CRT draws pixel in position {clock - 1}");
            Console.WriteLine($"Current CRT Row   : {row}");
            Console.WriteLine();

            for (int i = 0; i < 40; i++)
            {
                if (i == signal - 1 || i == signal || i == signal + 1)
                {
                    Console.Write('#');
                }
                else
                {
                    Console.Write('.');
                }
            }
            Console.WriteLine();
            Console.WriteLine();

            for (int i = 0; i < 40; i++)
            {
                if ((clock-1) % 40 == i)
                {
                    Console.Write('*');
                }
                else { Console.Write('.');}
            }
            Console.WriteLine();
        }

        //Console.WriteLine($"{clock}: {signal}");
        int pixel = (clock - 1) % 40 ;
        if (pixel - 1 <= signal && pixel + 1 >= signal)
        {
            row += "[blue]#[/]";
        }
        else
        {
            row += "[grey27].[/]";
        }
        if (clock % 40  == 0)
        {
            AnsiConsole.MarkupLine(row);
            row = string.Empty;
        }
        clock++;
    }

}
