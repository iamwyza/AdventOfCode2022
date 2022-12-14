using System.Text.RegularExpressions;
using Microsoft.VisualBasic;

namespace AdventOfCode2022.Day14
;
internal class Solution : DayBase
{
    private sbyte[,] _map;

    private (int minX, int minY, int maxX, int maxY) _bounds;
    private async Task Init()
    {
        var lines = await GetLines();
        _map = new sbyte[1000, 200];
        _bounds = new(500, 0, 0, 0);
        foreach (var line in lines)
        {
            (int X, int Y) position = (0, 0);
            foreach (var step in line.Split(" -> "))
            {
                var temp = step.Split(',');
                int targetX = int.Parse(temp[0]);
                int targetY = int.Parse(temp[1]);

                if (targetX > _bounds.maxX)
                    _bounds.maxX = targetX;
                if (targetY > _bounds.maxY)
                    _bounds.maxY = targetY;
                if (targetX < _bounds.minX)
                    _bounds.minX = targetX;
                if (targetY < _bounds.minY)
                    _bounds.minY = targetY;

                if (position.X == 0)
                {
                    position.X = targetX;
                    position.Y = targetY;
                    _map[targetX, targetY] = -1;
                    continue;
                }

                //AnsiConsole.MarkupLine($"Move From [yellow]{position.X}, {position.Y}[/] to [yellow]{targetX}, {targetY}[/]");

                while (targetX != position.X || targetY != position.Y)
                {
                    if (targetX > position.X)
                    {
                        position.X++;
                    }else if (targetX < position.X)
                    {
                        position.X--;
                    }else if (targetY > position.Y)
                    {
                        position.Y++;
                    }else if (targetY < position.Y)
                    {
                        position.Y--;
                    }
                    _map[position.X, position.Y] = -1;
                }
            }
        }
        
        //PrintCave();
    }

    public override async Task RunPart1()
    {
        PrintStart(1);
        await Init();

        (int X, int Y) sand = (500, 0);
        int grains = 0;
        while (true)
        {
            if (!MoveSand(ref sand))
            {
                grains++;
                _map[sand.X, sand.Y] = 1;
                sand = (500, 0);
                //PrintCave();
            }

            if (sand.X < _bounds.minX || sand.Y > _bounds.maxY) 
                break;
        }
        PrintCave();

        AnsiConsole.MarkupLine($"Total grains of sand: [green]{grains}[/]");
    }

    public override async Task RunPart2()
    {
        PrintStart(2);
        await Init();

        // Adds a floor 2 tiles below the perceived bottom
        _bounds.maxY += 2;
        _bounds.minX -= _bounds.maxY;
        _bounds.maxX += _bounds.maxY;

        for (int i = _bounds.minX; i <= _bounds.maxX; i++)
        {
            _map[i, _bounds.maxY] = -1;
        }

        PrintCave();


        (int X, int Y) sand = (500, 0);
        int grains = 0;
        while (true)
        {
            if (!MoveSand(ref sand))
            {
                grains++;
                _map[sand.X, sand.Y] = 1;
                if (sand.X == 500 && sand.Y == 0) break;
                sand = (500, 0);
                //PrintCave();
            }

            if (sand.X < _bounds.minX || sand.Y > _bounds.maxY)
                break;
        }
        PrintCave();

        AnsiConsole.MarkupLine($"Total grains of sand: [green]{grains}[/]");
    }

    private bool MoveSand(ref (int X, int Y) sand)
    {
        if (_map[sand.X, sand.Y + 1] == 0) // Down
        {
            sand.Y++;
        }
        else if (_map[sand.X - 1, sand.Y + 1] == 0) // Down Left
        {
            sand.X--;
            sand.Y++;
        }
        else if (_map[sand.X + 1, sand.Y + 1] == 0) // Down Right
        {
            sand.X++;
            sand.Y++;
        }
        else
        {
            return false;
        }
        return true;
    }
    
    


    private void PrintCave()
    {
        for (int yRow = _bounds.minY; yRow <= _bounds.maxY; yRow++)
        {
            for (int xCol = _bounds.minX; xCol <= _bounds.maxX; xCol++)
            {
                switch (_map[xCol, yRow])
                {
                    case -1: 
                        AnsiConsole.Markup("[red]#[/]");
                        break;
                    case 0:
                        AnsiConsole.Markup("[grey].[/]");
                        break;
                    case 1:
                        AnsiConsole.Markup("[white]o[/]");
                        break;

                }
            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }
}