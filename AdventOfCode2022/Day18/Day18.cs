using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.VisualBasic;

namespace AdventOfCode2022.Day18;
internal class Day18 : DayBase
{
    private sbyte[,,] _space;
    private sbyte _maxX = 0;
    private sbyte _maxY = 0;
    private sbyte _maxZ = 0;
    private async Task Init()
    {
        var lines = await GetLines();



        foreach (var line in lines)
        {
            var temp = line.Split(',').Select(sbyte.Parse).ToArray();

            if (temp[0] > _maxX)
            {
                _maxX = temp[0];
            }

            if (temp[1] > _maxY)
            {
                _maxY = temp[1];
            }

            if (temp[2] > _maxZ)
            {
                _maxZ = temp[2];
            }
        }

        _space = new sbyte[_maxX + 1, _maxY + 1, _maxZ + 1];

        foreach (var line in lines)
        {
            var temp = line.Split(',').Select(sbyte.Parse).ToArray();
            _space[temp[0], temp[1], temp[2]] = 7; // 6 sides + 1 to indicate it's a cube at all
        }

    }

    public override async Task RunPart1()
    {
        PrintStart(1);
        await Init();
        int totalSides = 0;

        for (sbyte z = 0; z <= _maxZ; z++)
        {
            for (sbyte y = 0; y <= _maxY; y++)
            {
                for (sbyte x = 0; x <= _maxX; x++)
                {
                    if (_space[x, y, z] == 0) continue;

                    //PrintGridSliced(x,y,z);

                    if (x > 0)
                    {
                        if (_space[x - 1, y, z] != 0)
                        {
                            _space[x, y, z]--;
                        }
                    }

                    if (x < _maxX)
                    {
                        if (_space[x + 1, y, z] != 0)
                        {
                            _space[x, y, z]--;
                        }
                    }

                    if (y > 0)
                    {
                        if (_space[x, y - 1, z] != 0)
                        {
                            _space[x, y, z]--;
                        }
                    }

                    if (y < _maxY)
                    {
                        if (_space[x, y + 1, z] != 0)
                        {
                            _space[x, y, z]--;
                        }
                    }

                    if (z > 0)
                    {
                        if (_space[x, y, z - 1] != 0)
                        {
                            _space[x, y, z]--;
                        }
                    }

                    if (z < _maxZ)
                    {
                        if (_space[x, y, z + 1] != 0)
                        {
                            _space[x, y, z]--;
                        }
                    }
                    AnsiConsole.MarkupLine($"The cube at [green]{x}, {y}, {z}[/] has [green]{_space[x, y, z] - 1}[/] sides exposed.");

                    totalSides += _space[x, y, z] - 1;
                    //PrintGridSliced(x,y,z);

                }
            }
        }

        PrintGridSliced(0, 0, 0);

        AnsiConsole.MarkupLine($"There were [green]{totalSides}[/] exposed");
    }


    private void PrintGridSliced(sbyte highlightX, sbyte highlightY, sbyte highlightZ)
    {
        sbyte zStart = 0;
        sbyte zEnd = _maxZ;
        if (highlightZ != 0)
        {
            zStart = (sbyte)(highlightZ - 1);
            zEnd = (sbyte)(highlightZ < _maxZ ? highlightZ + 1 : _maxZ);
        }
        for (sbyte z = zStart; z <= zEnd; z++)
        {

            AnsiConsole.MarkupLine($"Layer [red]{z}[/]");
            Console.WriteLine('+' + string.Concat(Enumerable.Repeat('-', _maxX)) + '+');

            for (sbyte y = 0; y <= _maxY; y++)
            {
                Console.Write('|');
                for (sbyte x = 0; x <= _maxX; x++)
                {
                    var value = _space[x, y, z];

                    if (value > 0)
                    {
                        if (x == highlightX && y == highlightY && z == highlightZ)
                        {
                            AnsiConsole.Markup($"[green]{value}[/]");
                        }
                        else
                        {
                            Console.Write(value);

                        }
                    }
                    else if (value < 0)
                    {
                        AnsiConsole.Markup($"[darkblue]#[/]");
                    }
                    else
                    {
                        Console.Write(' ');
                    }
                }
                Console.Write('|');
                Console.WriteLine();
            }
            Console.WriteLine('+' + string.Concat(Enumerable.Repeat('-', _maxX)) + '+');

            Console.WriteLine();
        }
    }

    public override async Task RunPart2()
    {
        PrintStart(2);
        await Init();

        int totalSides = 0;

        _space[0, 0, 0] = -1; // Gas

        for (sbyte z = 0; z <= _maxZ; z++)
        {
            for (sbyte y = 0; y <= _maxY; y++)
            {
                for (sbyte x = 0; x <= _maxX; x++)
                {
                    if (x == 0 && y == 0 && z == 0) continue; // Skip the first space

                    if (_space[x, y, z] != 0) continue; //if this space already has a cube or gas in it, skip that too

                    if (x > 0)
                    {
                        if (_space[x - 1, y, z] == -1)
                        {
                            _space[x, y, z] = -1;
                        }
                    }

                    if (x < _maxX)
                    {
                        if (_space[x + 1, y, z] == -1)
                        {
                            _space[x, y, z] = -1;
                        }
                    }

                    if (y > 0)
                    {
                        if (_space[x, y - 1, z] == -1)
                        {
                            _space[x, y, z] = -1;
                        }
                    }

                    if (y < _maxY)
                    {
                        if (_space[x, y + 1, z] == -1)
                        {
                            _space[x, y, z] = -1;
                        }
                    }

                    if (z > 0)
                    {
                        if (_space[x, y, z - 1] == -1)
                        {
                            _space[x, y, z] = -1;
                        }
                    }

                    if (z < _maxZ)
                    {
                        if (_space[x, y, z + 1] == -1)
                        {
                            _space[x, y, z] = -1;
                        }
                    }
                }
            }
        }

        for (sbyte z = _maxZ; z >= 0; z--)
        {
            for (sbyte y = _maxY; y >= 0; y--)
            {
                for (sbyte x = _maxX; x >= 0; x--)
                {
                    if (x == _maxZ && y == _maxY && z == _maxX) continue; // Skip the first space

                    if (_space[x, y, z] != 0) continue; //if this space already has a cube or gas in it, skip that too

                    if (x > 0)
                    {
                        if (_space[x - 1, y, z] == -1)
                        {
                            _space[x, y, z] = -1;
                        }
                    }

                    if (x < _maxX)
                    {
                        if (_space[x + 1, y, z] == -1)
                        {
                            _space[x, y, z] = -1;
                        }
                    }

                    if (y > 0)
                    {
                        if (_space[x, y - 1, z] == -1)
                        {
                            _space[x, y, z] = -1;
                        }
                    }

                    if (y < _maxY)
                    {
                        if (_space[x, y + 1, z] == -1)
                        {
                            _space[x, y, z] = -1;
                        }
                    }

                    if (z > 0)
                    {
                        if (_space[x, y, z - 1] == -1)
                        {
                            _space[x, y, z] = -1;
                        }
                    }

                    if (z < _maxZ)
                    {
                        if (_space[x, y, z + 1] == -1)
                        {
                            _space[x, y, z] = -1;
                        }
                    }
                }
            }
        }

        for (sbyte z = 0; z <= _maxZ; z++)
        {
            for (sbyte y = 0; y <= _maxY; y++)
            {
                for (sbyte x = 0; x <= _maxX; x++)
                {
                    if (_space[x, y, z] <= 0) continue;

                    //PrintGridSliced(x,y,z);

                    if (x > 0)
                    {
                        if (_space[x - 1, y, z] != -1)
                        {
                            _space[x, y, z]--;
                        }
                    }

                    if (x < _maxX)
                    {
                        if (_space[x + 1, y, z] != -1)
                        {
                            _space[x, y, z]--;
                        }
                    }

                    if (y > 0)
                    {
                        if (_space[x, y - 1, z] != -1)
                        {
                            _space[x, y, z]--;
                        }
                    }

                    if (y < _maxY)
                    {
                        if (_space[x, y + 1, z] != -1)
                        {
                            _space[x, y, z]--;
                        }
                    }

                    if (z > 0)
                    {
                        if (_space[x, y, z - 1] != -1)
                        {
                            _space[x, y, z]--;
                        }
                    }

                    if (z < _maxZ)
                    {
                        if (_space[x, y, z + 1] != -1)
                        {
                            _space[x, y, z]--;
                        }
                    }
                    AnsiConsole.MarkupLine($"The cube at [green]{x}, {y}, {z}[/] has [green]{_space[x, y, z] - 1}[/] sides exposed.");

                    totalSides += _space[x, y, z] - 1;
                    //PrintGridSliced(x,y,z);

                }
            }
        }

        PrintGridSliced(0, 0, 0);

        AnsiConsole.MarkupLine($"There were [green]{totalSides}[/] exposed");
    }
}