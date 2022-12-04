using System.Reflection;

namespace AdventOfCode2022;
public abstract class DayBase : IDay
{
    public static Dictionary<int, IDay> Days = new ();

    // Poor man's DI ;) 
    public static void ScanForSolutions()
    {
        var solutions = Assembly.GetExecutingAssembly().GetTypes().Where(x => x.BaseType == typeof(DayBase));
        foreach (var solution in solutions)
        {
            var day = int.Parse(solution.Namespace?.Replace("AdventOfCode2022.Day", "") ?? "-1");
            if (day <= 0) continue;

            if (Activator.CreateInstance(solution) is not IDay instance)
            {
                throw new Exception($"Couldn't create instance for type {solution.FullName}");
            }

            instance.Day = day;

            Days.Add(day, instance);
        }
    }

    public int Day { get; set; }

    public void PrintStart(int part)
    {
        Console.WriteLine($"Day {Day} - Part {part}:");
    }

    public async Task<string[]> GetLines()
    {
        return await File.ReadAllLinesAsync($"Day{Day}\\day{Day}input.txt");
    }

    public abstract Task RunPart1();
    public abstract Task RunPart2();
}
