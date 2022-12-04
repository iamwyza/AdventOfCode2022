namespace AdventOfCode2022;
public interface IDay
{
    int Day { get; set; }
    Task RunPart1();
    Task RunPart2();
}
