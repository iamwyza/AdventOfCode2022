using System.Text.RegularExpressions;

namespace AdventOfCode2022.Day15;
internal class Day15 : DayBase
{
    private Grid<sbyte> _map;

    List<(Coord signal, Coord beacon, int distance)> _beaconList;

    private (int minX, int minY, int maxX, int maxY) _bounds;
    private async Task Init( bool initMap)
    {
        var lines = await GetLines();
        _map = new Grid<sbyte>();
        _bounds = new(500, 0, 0, 0);

        _beaconList = new List<(Coord signal, Coord beacon, int distance)>();

        Regex parse = new Regex(@"^Sensor at x=(-?\d+), y=(-?\d+): closest beacon is at x=(-?\d+), y=(-?\d+)", RegexOptions.Compiled);

        foreach (var line in lines)
        {
            var match = parse.Match(line);

            Coord signal = new Coord { X = int.Parse(match.Groups[1].Value), Y = int.Parse(match.Groups[2].Value) };
            Coord beacon = new Coord { X = int.Parse(match.Groups[3].Value), Y = int.Parse(match.Groups[4].Value) };
            var distance = Math.Abs(beacon.X - signal.X) + Math.Abs(beacon.Y - signal.Y);

            if (initMap)
            {
                _map.CheckBounds(signal);
                _map.CheckBounds(beacon);
            }


            AnsiConsole.MarkupLine($"Distance between [yellow]{beacon}[/] and [yellow]{signal}[/] is [white]{distance}[/]");

            _beaconList.Add((signal, beacon, distance));
        }

        if (initMap)
        {
            _map.CalculateOffsets();

            _map.InitMap();

            foreach ((Coord sensor, Coord beacon, int distance) in _beaconList)
            {
                //sensor.XOffset = _map.XOffset;
                //sensor.YOffset = _map.YOffset;

                //beacon.XOffset = _map.XOffset;
                //beacon.YOffset = _map.YOffset;

                _map[sensor] = -1;
                _map[beacon] = 1;
            }

            _map.DefaultPrintConfig = (input) =>
            {
                return input switch
                {
                    -1 => ('S', Color.Green),
                    0 => ('.', Color.Grey),
                    1 => ('B', Color.Red),
                    2 => ('#', Color.Blue),
                    _ => (' ', Color.Yellow)
                };
            };

            _map.PrintMap(null);
        }
        
    }

    public override async Task RunPart1()
    {
        PrintStart(1);
        await Init(false);
        int inspectRow = 10;

        List<(Coord start, Coord end)> blockedCoords = new();

        foreach (var (signal, beacon, distance) in _beaconList)
        {
            //if (signal.X != 8 && signal.X != 0) continue;

            //AnsiConsole.MarkupLine($"Distance between [yellow]{beacon}[/] and [yellow]{signal}[/] is [white]{distance}[/]");
            (int left, int right, int up, int down) = (signal.X - distance, signal.X + distance, signal.Y - distance, signal.Y + distance);

            Console.WriteLine($"{up} and {down}");

            if (!(up < inspectRow && down > inspectRow))
            {
                Console.WriteLine("skipping");
                continue;
            }

            Coord? begCoord = null;
            Coord? endCoord = null;

            // Start at the top.
            Coord position = new Coord(signal.X, up);
            Coord start = new Coord(position.X, position.Y);
            bool first = true;

            while (signal.X != position.X || signal.Y != position.Y)
            {
                //AnsiConsole.MarkupLine($"{position}");
                // Down and to the right
                if (position.X >= signal.X && position.Y < signal.Y)
                {
                    position.X++;
                    position.Y++;
                }
                // Down and to the left
                else if (position.X > signal.X && position.Y >= signal.Y)
                {
                    position.X--;
                    position.Y++;
                }
                // Up and to the left
                else if (position.X <= signal.X && position.Y > signal.Y)
                {
                    position.X--;
                    position.Y--;
                }
                // Up and to the right
                else if (position.X <= signal.X && position.Y <= signal.Y)
                {
                    position.X++;
                    position.Y--;
                }



                if (position.Y == inspectRow)
                {
                    if (first)
                    {
                        endCoord = new Coord(position.X, position.Y);
                        first = false;
                    }
                    else
                    {
                        begCoord = new Coord(position.X, position.Y);
                        break;
                    }

                }

            }

            if (begCoord != null && endCoord != null)
            {
                blockedCoords.Add((begCoord.Value, endCoord.Value));
            }
        }

        var offset = blockedCoords.Any() ? blockedCoords.Min(x => x.start.X) * -1 : 0;

        var max = blockedCoords.Any() ? blockedCoords.Max(x => x.end.X) : _map.Bounds.maxX;

        bool[] blocked = new bool[max + offset + 1];

        foreach (var coords in blockedCoords)
        {
            for (int i = coords.start.X + offset; i < coords.end.X + offset; i++)
            {
                blocked[i] = true;
            }
        }

        var blockedCount = blocked.Where(x => x).Count();
        AnsiConsole.MarkupLine($"Blocked Spaces for row = [yellow]{blockedCount}[/]");
    }

    public override async Task RunPart2()
    {
        PrintStart(2);
        await Init(false);

        int upperBound = 4_000_000;
        int lowerBound = 0;

        //_map.CalculateOffsets();
        Coord? found = null;
        bool needsReset = false;

        for (int zy = 0; zy < upperBound; zy += 1)
        {
            List<(long min, long max)> ranges = new();

            //AnsiConsole.MarkupLine($"Calculating line [yellow]{zy}[/]");

            int changed = 0;

            foreach (var (signal, beacon, distance) in _beaconList)
            {
                //if (signal.X != 8) continue;

                var yOffset = Math.Abs(zy - signal.Y);
                if (yOffset > distance)
                    continue;

                var xOffset = distance - yOffset;
                ranges.Add((signal.X - xOffset, signal.X + xOffset));
            }

            //foreach ((int min, int max) in ranges.OrderBy(x => x.min))
            //{
            //    for(int i = min; i <= max; i++)
            //    {
            //        if (i >= lowerBound && i <= upperBound)
            //        {
            //            if (_map[i, zy] == 0)
            //                _map[i, zy] = 2;
            //        }
            //    }
            //}

            //_map.PrintMap(null);

            //if (zy == 11) Debugger.Break();

            if (FindBeacon(zy, ranges))
            {
                break;
            }
            
        }

        //AnsiConsole.MarkupLine($"Found the coordinate at [yellow]{found}[/]");

        bool FindBeacon(int y, List<(long min, long max)> ranges)
        {
            long endOfLine = 0;
            foreach ((long min, long max) in ranges.OrderBy(x => x.min))
            {
                //AnsiConsole.MarkupLine($"{min} to {endOfLine} is ({min - endOfLine})");
                if ((min - endOfLine) > 1 && min >= lowerBound && min - 1 <= upperBound && y > 0)
                {
                    AnsiConsole.MarkupLine($"Gap found at [red]{min - 1}, {y}[/]");
                    AnsiConsole.MarkupLine($"Frequency is [green]{((min - 1) * 4000000) + y}[/]");
                    return true;
                }
                if (max > endOfLine)
                    endOfLine = max;
            }
            return false;
        }
    }
}