using System.Collections;
using System.ComponentModel;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic;

namespace AdventOfCode2022.Day15;
internal class Solution : DayBase
{
    private Grid<bool> _map;

    List<(Coord signal, Coord beacon)> _beaconList;

    private (int minX, int minY, int maxX, int maxY) _bounds;
    private async Task Init()
    {
        var lines = await GetLines();
        _map = new Grid<bool>();
        _bounds = new(500, 0, 0, 0);

        _beaconList = new List<(Coord signal, Coord beacon)>();

        Regex parse = new Regex(@"^Sensor at x=(-?\d+), y=(-?\d+): closest beacon is at x=(-?\d+), y=(-?\d+)", RegexOptions.Compiled);

        foreach (var line in lines)
        {
            var match = parse.Match(line);

            Coord signal = new Coord { X = int.Parse(match.Groups[1].Value), Y = int.Parse(match.Groups[2].Value) };
            Coord beacon = new Coord { X = int.Parse(match.Groups[3].Value), Y = int.Parse(match.Groups[4].Value) };

            //_map.CheckBounds(signal);
            //_map.CheckBounds(beacon);

            _beaconList.Add((signal, beacon));
        }



        //foreach ((Coord sensor, Coord beacon) in _beaconList)
        //{
        //    //sensor.XOffset = _map.XOffset;
        //    //sensor.YOffset = _map.YOffset;

        //    //beacon.XOffset = _map.XOffset;
        //    //beacon.YOffset = _map.YOffset;

        //    _map[sensor] = -1;
        //    _map[beacon] = 1;
        //}

        //_map.DefaultPrintConfig = (input) =>
        //{
        //    return input switch
        //    {
        //        -1 => ('S', Color.Green),
        //        0 => ('.', Color.Grey),
        //        1 => ('B', Color.Red),
        //        2 => ('#', Color.Blue),
        //        _ => (' ', Color.Yellow)
        //    };
        //};

        //_map.PrintMap(null);
    }

    public override async Task RunPart1()
    {
        PrintStart(1);
        await Init();
        int inspectRow = 2_000_000;

        List<(Coord start, Coord end)> blockedCoords = new();

        foreach (var (signal, beacon) in _beaconList)
        {
            //if (signal.X != 8 && signal.X != 0) continue;

            var distance = Math.Abs(beacon.X - signal.X) + Math.Abs(beacon.Y - signal.Y);
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

        var offset = blockedCoords.Min(x => x.start.X) * -1;

        var max = blockedCoords.Max(x => x.end.X);

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
        await Init();

        int block = 40_000;

        _map.CheckBounds(new Coord(0, 0));
        _map.CheckBounds(new Coord(block, block));
        //_map.CalculateOffsets();
        _map.InitMap();
        Coord? found = null;
        bool needsReset = false;

        for (int zx = 0; zx < 4_000_000; zx+= block)
        {
            for (int zy = 0; zy < 4_000_000; zy += block)
            {
                Grid<bool>.XOffset = zx;
                Grid<bool>.YOffset = zy;

                AnsiConsole.MarkupLine($"Calculating Block [yellow]{zx},{zy}[/] to [yellow]{zx+block},{zy+block}[/]");

                if (needsReset)
                    _map.ResetMap(false);
                //List<(Coord start, Coord end)> blockedCoords = new();

                int changed = 0;

                Parallel.ForEach(_beaconList, new ParallelOptions{MaxDegreeOfParallelism = 10},b =>
                {
                    //if (signal.X != 8 && signal.X != 0) continue;

                    var (signal, beacon) = b;
                    var distance = Math.Abs(beacon.X - signal.X) + Math.Abs(beacon.Y - signal.Y);
                    AnsiConsole.MarkupLine($"Distance between [yellow]{beacon}[/] and [yellow]{signal}[/] is [white]{distance}[/]");
                    (int left, int right, int up, int down) = (signal.X - distance, signal.X + distance,
                        signal.Y - distance, signal.Y + distance);

                    //Console.WriteLine($"{up} and {down}");

                    //if (!(up < inspectRow && down > inspectRow))
                    //{
                    //    Console.WriteLine("skipping");
                    //    continue;
                    //}

                    Coord? begCoord = null;
                    Coord? endCoord = null;

                    // Start at the top.
                    Coord position = new Coord(signal.X, up);
                    Coord start = new Coord(position.X, position.Y);
                    bool first = true;
                    bool anyInBounds = false;

                    if (!(CohenSutherlandLineClip(signal.X, up, left, signal.Y, zx + block, zy + block, zx,
                              zy) // Top-to-left
                          || CohenSutherlandLineClip(signal.X, up, right, signal.Y, zx + block, zy + block, zx,
                              zy) // Top-to-right
                          || CohenSutherlandLineClip(signal.X, down, left, signal.Y, zx + block, zy + block, zx,
                              zy) // Bottom-to-left
                          || CohenSutherlandLineClip(signal.X, down, right, signal.Y, zx + block, zy + block, zx,
                              zy))) // Bottom-to-right
                    {
                        //AnsiConsole.WriteLine("nothing in bounds, skipping");
                        return;
                    }

                    (int X, int Y) topleft = ((signal.X - distance/ 2), (signal.Y - distance / 2));
                    (int X, int Y) topRight = ((signal.X + distance/ 2), (signal.Y - distance / 2));
                    (int X, int Y) bottomleft = ((signal.X - distance/ 2), (signal.Y + distance / 2));
                    (int X, int Y) bottomRight = ((signal.X + distance/ 2), (signal.Y + distance / 2));

                    if (zx > topleft.X && zx < topRight.X && zy < bottomleft.Y && zy > topRight.Y)
                    {
                        // This box is completely covered by this range, so we can safely skip it
                        return;
                    }

                    needsReset = true;

                    while (signal.X != position.X || signal.Y != position.Y)
                    {
                        //if (count == 10000) break;
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

                        if (CohenSutherlandLineClip(position.X + Grid<bool>.XOffset, position.Y + Grid<bool>.YOffset, position.X + Grid<bool>.XOffset, position.Y + Grid<bool>.YOffset, zx + block, zy + block, zx,
                                zy))
                        {
                            changed++;
                            _map[position.X, position.Y] = true;
                        }

                        if (position.X == start.X && position.Y == start.Y)
                        {
                            position.Y++;
                            start = new Coord(position.X, position.Y);
                        }
                    }
                });

                if (changed > 0)
                {
                    for (int yRow = _map.Bounds.minY; yRow <= _map.Bounds.maxY; yRow++)
                    {
                        for (int xCol = _map.Bounds.minX; xCol <= _map.Bounds.maxX; xCol++)
                        {
                            if (_map.Map[xCol, yRow])
                            {
                                found = new Coord(xCol, yRow);
                                break;
                            }
                        }
                        if (found != null) { break; }
                    }

                }
               
                if (found != null) { break; }
            }
            if (found != null) { break; }
        }

        AnsiConsole.MarkupLine($"Found the coordinate at [yellow]{found}[/]");
        //_map.PrintMap(null);

        //int numberOfBlocked = 0;

        //for (int i = _map.Bounds.minX; i < _map.Bounds.maxX; i++)
        //{
        //    var temp = _map[new Coord(i, 10)];

        //    if (temp == 2)
        //        numberOfBlocked++;
        //}

        //var offset = blockedCoords.Min(x => x.start.X) * -1;

        //var max = blockedCoords.Max(x => x.end.X);

        //bool[] blocked = new bool[max + offset + 1];

        //foreach (var coords in blockedCoords)
        //{
        //    for (int i = coords.start.X + offset; i < coords.end.X + offset; i++)
        //    {
        //        blocked[i] = true;
        //    }
        //}

        //var blockedCount = blocked.Where(x => x).Count();
        //AnsiConsole.MarkupLine($"Blocked Spaces for row = [yellow]{blockedCount}[/]");

    }

    [Flags]
    public enum Box
    {
        INSIDE = 0,
        LEFT = 1,
        RIGHT = 2,
        BOTTOM = 4,
        TOP = 8
    }

    // Cohen–Sutherland clipping algorithm clips a line from
    // P0 = (x0, y0) to P1 = (x1, y1) against a rectangle with 
    // diagonal from (xmin, ymin) to (xmax, ymax).
    bool CohenSutherlandLineClip(double x0, double y0, double x1, double y1, double xmax, double ymax, double xmin, double ymin)
    {
        // compute outcodes for P0, P1, and whatever point lies outside the clip rectangle
        Box outcode0 = ComputeOutCode(x0, y0, xmax, ymax, xmin, ymin);
        Box outcode1 = ComputeOutCode(x1, y1, xmax, ymax, xmin, ymin);
        bool accept = false;

        while (true)
        {
            if ((outcode0 | outcode1) == 0)
            {
                // bitwise OR is 0: both points inside window; trivially accept and exit loop
                accept = true;
                break;
            }
            else if ((outcode0 & outcode1) != 0)
            {
                // bitwise AND is not 0: both points share an outside zone (LEFT, RIGHT, TOP,
                // or BOTTOM), so both must be outside window; exit loop (accept is false)
                break;
            }
            else
            {
                // failed both tests, so calculate the line segment to clip
                // from an outside point to an intersection with clip edge
                double x = 0;
                double y = 0;

                // At least one endpoint is outside the clip rectangle; pick it.
                Box outcodeOut = outcode1 > outcode0 ? outcode1 : outcode0;

                // Now find the intersection point;
                // use formulas:
                //   slope = (y1 - y0) / (x1 - x0)
                //   x = x0 + (1 / slope) * (ym - y0), where ym is ymin or ymax
                //   y = y0 + slope * (xm - x0), where xm is xmin or xmax
                // No need to worry about divide-by-zero because, in each case, the
                // outcode bit being tested guarantees the denominator is non-zero
                if ((outcodeOut & Box.TOP) != 0)
                {           // point is above the clip window
                    x = x0 + (x1 - x0) * (ymax - y0) / (y1 - y0);
                    y = ymax;
                }
                else if ((outcodeOut & Box.BOTTOM) != 0)
                { // point is below the clip window
                    x = x0 + (x1 - x0) * (ymin - y0) / (y1 - y0);
                    y = ymin;
                }
                else if ((outcodeOut & Box.RIGHT) != 0)
                {  // point is to the right of clip window
                    y = y0 + (y1 - y0) * (xmax - x0) / (x1 - x0);
                    x = xmax;
                }
                else if ((outcodeOut & Box.LEFT) != 0)
                {   // point is to the left of clip window
                    y = y0 + (y1 - y0) * (xmin - x0) / (x1 - x0);
                    x = xmin;
                }

                // Now we move outside point to intersection point to clip
                // and get ready for next pass.
                if (outcodeOut == outcode0)
                {
                    x0 = x;
                    y0 = y;
                    outcode0 = ComputeOutCode(x0, y0, xmax, ymax, xmin, ymin);
                }
                else
                {
                    x1 = x;
                    y1 = y;
                    outcode1 = ComputeOutCode(x1, y1, xmax, ymax, xmin, ymin);
                }
            }
        }
        return accept;
    }

    Box ComputeOutCode(double x, double y, double xmax, double ymax, double xmin, double ymin)
    {
        Box code = Box.INSIDE;  // initialised as being inside of clip window

        if (x < xmin)           // to the left of clip window
            code |= Box.LEFT;
        else if (x > xmax)      // to the right of clip window
            code |= Box.RIGHT;
        if (y < ymin)           // below the clip window
            code |= Box.BOTTOM;
        else if (y > ymax)      // above the clip window
            code |= Box.TOP;

        return code;
    }
}