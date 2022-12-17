namespace AdventOfCode2022.Day00;
internal class Day00 : DayBase
{
    private Grid<sbyte> _map;

    private async Task Init()
    {
        var _map = new Grid<sbyte>();

        var lines = await GetLines();

        foreach (var line in lines)
        {

        }

        _map.InitMap();

        _map.PrintMap();
    }

    public override async Task RunPart1()
    {
        PrintStart(1);
        await Init();

    }

    public override async Task RunPart2()
    {
        PrintStart(2);
        await Init();
    }
}