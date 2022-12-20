using System;
using System.Diagnostics;
using Microsoft.Toolkit.HighPerformance;

namespace AdventOfCode2022.Day17;
internal class Day17 : DayBase
{

    private Grid<sbyte> _map;

    private static readonly sbyte[][,] _pieces = new[]
    {
        new sbyte[,]
        {
            { 1 },
            { 1 },
            { 1 },
            { 1 }
        },

        new sbyte[,]
        {
            { 0,1,0 },
            { 1,1,1 },
            { 0,1,0 }
        },
        new sbyte[,]
        {
            { 0,0,1 },
            { 0,0,1 },
            { 1,1,1 }
        },
        new sbyte[,]
        {
            { 1,1,1,1 }
        },
        new sbyte[,]
        {
            { 1,1 },
            { 1,1 }
        }
    };

    private static (byte X, byte Y)[] _pieceSizes;
    private static int MaxY;

    //true = left, false = right
    private bool[] _moves;

    private async Task Init()
    {
        _map = new Grid<sbyte>();
        _map.CheckBounds(new Coord(0, 0));
        _map.CheckBounds(new Coord(8, 40000)); // Max height of the cave is max piece height * 2022 (number of pieces that will fall)

        var lines = await GetLines();

        _moves = new bool[lines[0].Length];
        _pieceSizes = new (byte X, byte Y)[_pieces.Length];

        for (int i = 0; i < _pieces.Length; i++)
        {
            _pieceSizes[i] = ((byte)_pieces[i].GetLength(0), (byte)_pieces[i].GetLength(1));
        }

        for (int i = 0; i < lines[0].Length; i++)
        {
            _moves[i] = lines[0][i] == '<';
        }

        _map.DefaultPrintConfig = (input) =>
        {
            return input switch
            {
                0 => ('.', Color.Grey),
                1 => ('#', Color.Green),
                2 => ('|', Color.White),
                3 => ('-', Color.White),
                4 => ('+', Color.White),
                5 => ('@', Color.Red),
                6 => ('#', Color.Red),
                7 => ('|', Color.Red),
                _ => (' ', Color.Yellow)
            };
        };
        _map.CanWrapY = true;
        ResetMap();

        foreach (var piece in _pieces)
        {
            var tempMap = new Grid<sbyte>();
            tempMap.CheckBounds(new Coord(0, 0));
            tempMap.CheckBounds(new Coord(piece.GetLength(0) - 1, piece.GetLength(1) - 1));
            tempMap.DefaultPrintConfig = _map.DefaultPrintConfig;
            tempMap.InitMap();
            tempMap.Map = piece;

            tempMap.PrintMap();
        }

        _map.PrintMap(null, false, false, _map.Bounds.maxY - 10, _map.Bounds.maxY);
    }

    private void ResetMap()
    {
        _map.InitMap();

        for (int y = _map.Bounds.maxY; y >= 0; y--)
        {
            if (y == _map.Bounds.maxY)
            {
                for (int x = 1; x < _map.Bounds.maxX; x++)
                {
                    _map[x, y] = 3;
                }

                _map[0, y] = 2;
                _map[_map.Bounds.maxX, y] = 2;

            }
            else
            {
                _map[0, y] = 2;
                _map[_map.Bounds.maxX, y] = 2;
            }
        }
    }

    public override async Task RunPart1()
    {
        PrintStart(1);
        await Init();

        //int movementIndex = 0;
        //int pieceIndex = 0;
        //int highestRock = _map.Bounds.maxY;
        //char[] movesChars = _moves.Select(x => x ? '<' : '>').ToArray();

        //for (int i = 0; i < 2022; i++)
        //{
        //    var piece = _pieces[pieceIndex++];
        //    if (pieceIndex > _pieces.Length - 1)
        //        pieceIndex = 0;

        //    var bottomLeftCord = new Coord(3, highestRock - 4);

        //    //AnsiConsole.MarkupLine($"Highest rock is now [yellow]{highestRock}[/]");

        //    while (true)
        //    {
        //        sbyte offset = (sbyte)(_moves[movementIndex++] ? -1 : 1);
        //        if (movementIndex > _moves.Length - 1)
        //            movementIndex = 0;

        //        //AnsiConsole.Markup($"Try to move [yellow]{(offset == 1 ? 'R' : 'L' )}[/] : ");

        //        //Debugging to show first state (doesn't actually try to move the piece)
        //        CanMove(bottomLeftCord, 0, 0, piece);
        //        Printmap(i);
        //        ResetTemp(bottomLeftCord, 0, 0, piece);


        //        if (CanMove(bottomLeftCord, offset, 0, piece))
        //        {
        //            //AnsiConsole.MarkupLine("[green]Success[/]");
        //            Printmap(i);
        //            ResetTemp(bottomLeftCord, offset, 0, piece);
        //            bottomLeftCord.X += offset;
        //        }
        //        else
        //        {
        //            //AnsiConsole.MarkupLine("[red]Failure[/]");
        //            Printmap(i);
        //            ResetTemp(bottomLeftCord, offset, 0, piece);
        //        }



        //        //AnsiConsole.Markup($"Try to move [yellow]Down[/] : ");

        //        if (CanMove(bottomLeftCord, 0, 1, piece))
        //        {
        //            //AnsiConsole.MarkupLine("[green]Success[/]");
        //            Printmap(i);
        //            ResetTemp(bottomLeftCord, 0, 1, piece);
        //            bottomLeftCord.Y += 1;
        //        }
        //        else
        //        {
        //            //AnsiConsole.MarkupLine("[red]Failure[/]");
        //            Printmap(i);
        //            ResetTemp(bottomLeftCord, 0, 1, piece);
        //            SetPiece(bottomLeftCord, piece, ref highestRock);
        //            Printmap(i);
        //            break;
        //        }

        //    }
        //}

        //_map.PrintMap(null, true, false);

        //AnsiConsole.MarkupLine($"The height of the rocks is [green]{_map.Bounds.maxY - highestRock}[/]");

        //void Printmap(int row)
        //{
        //    return;
        //    AnsiConsole.Clear();
        //    Console.WriteLine();
        //    Console.WriteLine();
        //    AnsiConsole.MarkupLine($"Rock # [green]{row}[/]");
        //   // AnsiConsole.MarkupLine($"[grey]{string.Join("",movesChars[(movementIndex-20 < 0 ? 0 : movementIndex - 20)..movementIndex])}[/][green]{movesChars[movementIndex]}[/][grey]{string.Join("", movesChars[movementIndex..( movementIndex + 20 > movesChars.Length ? movesChars.Length :  movementIndex +20)])}[/]");
        //    Console.WriteLine();
        //    _map.PrintMap(null, true, false, highestRock - 5, highestRock + 20);
        //    Thread.Sleep(50);
        //}
    }

    private static int GetY(int currentY)
    {
        if (currentY < 0) // -1001
        {
            return (currentY % MaxY) + MaxY;
        }
        if (currentY > MaxY - 1)
        {
            return (currentY % MaxY);
        }
        return currentY;
    }

    private void SetPiece(Span2D<sbyte> map, Coord bottomLeftCord, ReadOnlySpan2D<sbyte> piece,
        (int sizeX, int sizeY) pieceSize, ref int highestRock)
    {
        for (int y = 0; y < pieceSize.sizeY; y++)
        {
            int actualY = bottomLeftCord.Y + (y - pieceSize.sizeY + 1);
            actualY = GetY(actualY);

            for (int x = 0; x < pieceSize.sizeX; x++)
            {
                if (piece[x, y] == 0) continue;

                map[bottomLeftCord.X + x, actualY] = piece[x, y];

                if (bottomLeftCord.Y - y < highestRock)
                    highestRock = bottomLeftCord.Y - y;

                //_map.PrintMap(null, true, false, actualY - 20, actualY + 20);
            }
        }
    }

    private bool CanMove(Span2D<sbyte> map, Coord bottomLeftCord, sbyte xOffset, byte yOffset, ReadOnlySpan2D<sbyte> piece, (int sizeX, int sizeY) pieceSize)
    {
        //bool canMove = true;
        int actualY;

        for (int y = 0; y < pieceSize.sizeY; y++)
        {
            actualY = -999999;

            for (int x = 0; x < pieceSize.sizeX; x++)
            {
                if (piece[x, y] == 0) continue;

                if (actualY == -999999)
                {
                    actualY = bottomLeftCord.Y + yOffset + (y - pieceSize.sizeY + 1);
                    actualY = GetY(actualY);
                }

                //if (value == 5)
                //    value = 0;

                if (map[bottomLeftCord.X + x + xOffset, actualY] + 1 > 1)
                {
                    return false;
                }

                // for visualizing the movements, not really needed to calculate them.
                //_map[bottomLeftCord.X + x + xOffset, actualY] = (sbyte)(piece[x, y] == 1 ? _map[bottomLeftCord.X + x + xOffset, actualY] + 5 : _map[bottomLeftCord.X + x + xOffset, actualY]);
            }
        }

        return true;
    }

    private void ResetTemp(Span2D<sbyte> map, Coord bottomLeftCord, sbyte xOffset, byte yOffset, sbyte[,] piece)
    {
        for (int y = 0; y < piece.GetLength(1); y++)
        {
            int actualY = bottomLeftCord.Y + yOffset + (y - piece.GetLength(1) + 1);
            actualY = GetY(actualY);

            for (int x = 0; x < piece.GetLength(0); x++)
            {

                if (map[bottomLeftCord.X + x + xOffset, actualY] >= 5)
                {
                    map[bottomLeftCord.X + x + xOffset, actualY] -= 5;
                }
            }
        }
    }

    public override Task RunPart2()
    {
        PrintStart(2);
        Init().GetAwaiter().GetResult();

        int movementIndex = 0;
        int pieceIndex = 0;
        int highestRock = _map.Bounds.maxY;
        char[] movesChars = _moves.Select(x => x ? '<' : '>').ToArray();
        int loop = 0;
        short[] rowState = new short[100000];
        int rowStateIndex = 0;

        var map = new Span2D<sbyte>(_map.Map);
        MaxY = map.Width;
        var pieces = new Span<sbyte[,]>(_pieces);
        var pieceSizes = new Span<(byte, byte)>(_pieceSizes);
        //const long totalIterations = 20000;
        (int patternSize, int patternOffset, long _, long _) = Temp(0, 0);
        (int _, int _, long rockCount, long startRockCount) = Temp(patternSize, patternOffset);

        if (rockCount == 0)
        {
            rockCount = 1;
        }

        long totalIterations = 1000000000000 - startRockCount;

        long iterations = totalIterations / rockCount;
        long remainder = (totalIterations % rockCount);

        AnsiConsole.MarkupLine($"Remainder: [yellow]{remainder}[/] : ({totalIterations:N0} - {startRockCount}) % {rockCount}.  (earliest rock seen that starts the pattern)");
        AnsiConsole.MarkupLine($"Count: [yellow]{iterations}[/]");

        for (long i = 0; i < remainder; i++)
        {
            //if (i == 28) 
            //    Debugger.Break();
            var pieceSize = pieceSizes[pieceIndex];
            var piece = new ReadOnlySpan2D<sbyte>(_pieces[pieceIndex++]);
            if (pieceIndex > _pieces.Length - 1)
                pieceIndex = 0;

            // If we've looped around a few times, we should reset the highestRock down (to keep the numbers more managable. 
            if (highestRock < 0)
            {
                //AnsiConsole.MarkupLine($"Rock # [green]{i}[/].  Loop: [green]{loop}[/] Highest: [green]{highestRock}[/] Calc: [green]{((_map.Bounds.maxY+1) * (loop)) + (_map.Bounds.maxY - highestRock)}[/]");
                loop++;
                //Printmap(i);
                highestRock = (highestRock % _map.Bounds.maxY) + _map.Bounds.maxY + 1;
                //Printmap(i);
                //AnsiConsole.MarkupLine($"Rock # [green]{i}[/].  Loop: [green]{loop}[/] Highest: [green]{highestRock}[/] Calc: [green]{((_map.Bounds.maxY + 1) * (loop)) + (_map.Bounds.maxY - highestRock)}[/]");

            }

            var bottomLeftCord = new Coord(3, highestRock - 4);

            //Printmap(i);
            for (int y = bottomLeftCord.Y; y > bottomLeftCord.Y - 5; y--)
            {
                int actualY = GetY(y);
                short rowValue = 0;
                for (int x = 1; x < map.Height - 1; x++)
                {
                    if (map[x, actualY] == 1)
                    {

                        rowValue += (short)(1 << x - 1);
                    }
                    map[x, actualY] = 0;
                }

                if (rowValue > 0)
                {
                    rowState[rowStateIndex++] = rowValue;
                }
            }

            if (rowStateIndex == 99999)
                break;

            //AnsiConsole.MarkupLine($"Highest rock is now [yellow]{highestRock}[/]");

            short moveCount = 0;

            while (true)
            {
                sbyte offset = (sbyte)(_moves[movementIndex++] ? -1 : 1);
                if (movementIndex > _moves.Length - 1)
                    movementIndex = 0;

                //Debugging to show first state (doesn't actually try to move the piece)
                //CanMove(map, bottomLeftCord, 0, 0, piece);
                //Printmap(i);
                //ResetTemp(map, bottomLeftCord, 0, 0, piece);

                // First movement left or right is guaranteed to succeed, so don't run the code to check it
                if (moveCount < 1 || CanMove(map, bottomLeftCord, offset, 0, piece, pieceSize))
                {
                    //Printmap(i);
                    //ResetTemp(map,bottomLeftCord, offset, 0, piece);
                    bottomLeftCord.X += offset;
                }
                else
                {
                    //Printmap(i);
                    //ResetTemp(map, bottomLeftCord, offset, 0, piece);
                }


                // First 2 movements down are guaranteed to succeed, so don't run the code to check them
                if (moveCount <= 1 || CanMove(map, bottomLeftCord, 0, 1, piece, pieceSize))
                {
                    //Printmap(i);
                    //ResetTemp(map, bottomLeftCord, 0, 1, piece);
                    bottomLeftCord.Y += 1;
                }
                else
                {
                    //Printmap(i);
                    //ResetTemp(map, bottomLeftCord, 0, 1, piece);
                    SetPiece(map, bottomLeftCord, piece, pieceSize, ref highestRock);
                    break;
                }

                moveCount++;

            }
        }

        //PrintMap(null, true, false, highestRock - 25, highestRock + 25);

        long height = (MaxY - patternOffset - 1) + (patternSize * iterations) + (_map.Bounds.maxY - highestRock);
        //long final = 


        AnsiConsole.MarkupLine($"Height: [yellow]{height}[/]");

        if (height != 1514285714288)
        {
            AnsiConsole.MarkupLine($"Height should have been [yellow]{1514285714288}[/] but it was [red]{height}[/] which is off by [red]{1514285714288 - height}[/]");
        }


        return Task.CompletedTask;

        void Printmap(long row)
        {
            AnsiConsole.Clear();
            Console.WriteLine();
            Console.WriteLine();
            // AnsiConsole.MarkupLine($"[grey]{string.Join("",movesChars[(movementIndex-20 < 0 ? 0 : movementIndex - 20)..movementIndex])}[/][green]{movesChars[movementIndex]}[/][grey]{string.Join("", movesChars[movementIndex..( movementIndex + 20 > movesChars.Length ? movesChars.Length :  movementIndex +20)])}[/]");
            Console.WriteLine();
            PrintMap(null, true, false);
            Thread.Sleep(10);
        }
    }

    void PrintMap(Func<sbyte, (char letter, Color color)>? config = null, bool printRowLabel = false, bool printEmptyRow = true, int yMin = 0, int yMax = 0)
    {
        config ??= _map.DefaultPrintConfig;
        var Bounds = _map.Bounds;

        for (int yRow = Bounds.minY; yRow <= Bounds.maxY; yRow++)
        {


            bool hasData = false;

            if (!printEmptyRow)
            {
                for (int xCol = Bounds.minX; xCol <= Bounds.maxX; xCol++)
                {
                    if (!_map.Map[xCol, yRow].Equals(0))
                    {
                        hasData = true;
                        break;
                    }
                }
            }

            if (printEmptyRow || hasData && ((yMin == 0 && yMax == 0) || (yRow >= yMin && yRow <= yMax)))
            {
                if (printRowLabel)
                    AnsiConsole.Markup($"{yRow:0000;-000}");

                for (int xCol = Bounds.minX; xCol <= Bounds.maxX; xCol++)
                {
                    var setting = config(_map.Map[xCol, yRow]);

                    AnsiConsole.Markup($"[{setting.color.ToMarkup()}]{setting.letter}[/]");
                }
                Console.WriteLine();
            }
        }

        Console.WriteLine();
    }


    private (int patternSize, int patternOffset, long rockCount, long startRockCount) Temp(int patternSize, int patternOffset)
    {
        int movementIndex = 0;
        int pieceIndex = 0;
        int highestRock = _map.Bounds.maxY;
        short[] rowState = new short[100000];
        int rowStateIndex = 0;
        ResetMap();

        var map = new Span2D<sbyte>(_map.Map);
        MaxY = map.Width;
        var pieceSizes = new Span<(byte, byte)>(_pieceSizes);
        //const long totalIterations = 20000;
        long rockCount = 0;
        long startRockCount = 0;

        short moveCount;
        int loop = 0;
        for (long i = 0; i < 100000; i++)
        {
            var pieceSize = pieceSizes[pieceIndex];
            var piece = new ReadOnlySpan2D<sbyte>(_pieces[pieceIndex++]);
            if (pieceIndex > _pieces.Length - 1)
                pieceIndex = 0;

            // If we've looped around, we should reset the highestRock down (to keep the numbers more managable. 
            if (highestRock < 0)
            {
                loop++;
                highestRock = (highestRock % _map.Bounds.maxY) + _map.Bounds.maxY + 1;
            }

            var bottomLeftCord = new Coord(3, highestRock - 4);

            //Printmap(i);
            for (int y = bottomLeftCord.Y; y > bottomLeftCord.Y - 5; y--)
            {
                int actualY = GetY(y);
                short rowValue = 0;
                for (int x = 1; x < map.Height - 1; x++)
                {
                    if (map[x, actualY] == 1)
                    {

                        rowValue += (short)(1 << x - 1);
                    }
                    map[x, actualY] = 0;
                }

                if (rowValue > 0)
                {
                    rowState[rowStateIndex++] = rowValue;
                    if (rowStateIndex == 50000)
                        break;
                }
            }

            if (rowStateIndex == 50000)
                break;


            //AnsiConsole.MarkupLine($"Highest rock is now [yellow]{highestRock}[/]");

            moveCount = 0;

            while (true)
            {
                sbyte offset = (sbyte)(_moves[movementIndex++] ? -1 : 1);
                if (movementIndex > _moves.Length - 1)
                    movementIndex = 0;

                //Debugging to show first state (doesn't actually try to move the piece)
                //CanMove(map, bottomLeftCord, 0, 0, piece);
                //Printmap(i);
                //ResetTemp(map, bottomLeftCord, 0, 0, piece);

                // First movement left or right is guaranteed to succeed, so don't run the code to check it
                if (moveCount < 1 || CanMove(map, bottomLeftCord, offset, 0, piece, pieceSize))
                {
                    //Printmap(i);
                    //ResetTemp(map,bottomLeftCord, offset, 0, piece);
                    bottomLeftCord.X += offset;
                }
                else
                {
                    //Printmap(i);
                    //ResetTemp(map, bottomLeftCord, offset, 0, piece);
                }



                // First 2 movements down are guaranteed to succeed, so don't run the code to check them
                if (moveCount <= 1 || CanMove(map, bottomLeftCord, 0, 1, piece, pieceSize))
                {
                    //Printmap(i);
                    //ResetTemp(map, bottomLeftCord, 0, 1, piece);
                    bottomLeftCord.Y += 1;
                }
                else
                {
                    //Printmap(i);
                    //ResetTemp(map, bottomLeftCord, 0, 1, piece);
                    SetPiece(map, bottomLeftCord, piece, pieceSize, ref highestRock);
                    //PrintMap(null, true, false, 35600, 40000);
                    break;
                }

                moveCount++;

            }

            //Printmap(i);
            if (patternSize > 0)
            {
                if (highestRock == patternOffset && startRockCount == 0)
                {
                    if (loop > 0)
                        AnsiConsole.MarkupLine("[red]Had to loop to find a pattern, that shouldn't have happened.[/]");

                    startRockCount = i+1;
                    //PrintMap(null, true, false, highestRock - 75, highestRock + 75);
                    AnsiConsole.MarkupLine($"Pattern Started with rock count of [yellow]{startRockCount}[/].  Highest Rock is [yellow]{MaxY - highestRock}[/]");
                }

                if (highestRock <= patternOffset-1 - patternSize)
                {

                    if (loop > 0)
                        AnsiConsole.MarkupLine("[red]Had to loop to find a pattern, that shouldn't have happened.[/]");

                    rockCount = i - startRockCount;
                    AnsiConsole.MarkupLine($"Pattern Ended with rock count of [yellow]{i}[/]. Highest Rock is [yellow]{MaxY - highestRock} ({highestRock})[/]");
                    //PrintMap(null, true, false, highestRock - 20, highestRock+ 20);
                    break;
                }


            }

        }

        //PrintMap(null, true, false, highestRock - 25, highestRock + 25);

        if (rowState[0] == 0)
        {
            for (int y = MaxY; y > 0; y--)
            {
                int actualY = GetY(y);
                short rowValue = 0;
                for (int x = 1; x < map.Height - 1; x++)
                {
                    if (map[x, actualY] == 1)
                    {

                        rowValue += (short)(1 << x - 1);
                    }
                    map[x, actualY] = 0;
                }

                if (rowValue > 0)
                {
                    rowState[rowStateIndex++] = rowValue;
                }
            }
        }

        if (rockCount == 0)
        {
            // search through the state of the rows, if we find 2 sets of them that repeat for 10 entries, stop.
            bool match = true;
            int size = 30000;

            for (int i = 0; i <= rowStateIndex; i++)
            {
                for (int s = i + 1; s <= rowStateIndex; s++)
                {
                    if (rowState[s] == 0) continue;

                    match = true;
                    for (int y = 0; y < s; y++)
                    {
                        match &= rowState[i + y] == rowState[s + y];

                        if (!match)
                            break;
                        size = y;

                    }



                    if (match)
                    {
                        //PrintrowState(rowState, null, true, false, size - 20, size + 20);
                        patternOffset = MaxY - i - 1;
                        patternSize = size - i + 1;

                        //for (int y = 0; y <= size; y++)
                        //{
                        //    //AnsiConsole.Markup($"{rowState[s + y]:000}");
                        //    AnsiConsole.Markup("|");
                        //    for (int x = 1; x < map.Height - 1; x++)
                        //    {
                        //        if ((rowState[s + y] & (1 << x - 1)) != 0)
                        //        {
                        //            AnsiConsole.Markup("[green]#[/]");
                        //        }
                        //        else
                        //        {
                        //            AnsiConsole.Markup("[grey].[/]");
                        //        }
                        //    }
                        //    //AnsiConsole.MarkupLine("|");
                        //    AnsiConsole.MarkupLine($"| Lines {MaxY - (i + y) - 1} and {MaxY - (s + y) - 1}");
                        //}

                        break;
                    }
                }

                if (match)
                    break;
            }


            AnsiConsole.MarkupLine($"Pattern Offset: [yellow]{patternOffset}[/]");
            AnsiConsole.MarkupLine($"Pattern Size: [yellow]{patternSize}[/]");
        }
        else
        {
            AnsiConsole.MarkupLine($"Pattern Contains [yellow]{rockCount}[/] rocks.");
        }

        //PrintrowState(rowState, null, true, false, 0, 200);



        return (patternSize, patternOffset, rockCount, startRockCount);

        void PrintrowState(short[] rowState, Func<sbyte, (char letter, Color color)>? config = null, bool printRowLabel = false, bool printEmptyRow = true, int yMin = 0, int yMax = 0)
        {
            config ??= _map.DefaultPrintConfig;
            var Bounds = _map.Bounds;

            for (int yRow = Bounds.minY; yRow <= Bounds.maxY; yRow++)
            {


                bool hasData = false;

                for (int xCol = Bounds.minX; xCol <= Bounds.maxX; xCol++)
                {
                    if (rowState[yRow] != 0)
                    {
                        hasData = true;
                        break;
                    }
                }

                if (hasData && ((yMin == 0 && yMax == 0) || (yRow >= yMin && yRow <= yMax)))
                {
                    if (printRowLabel)
                        AnsiConsole.Markup($"{yRow:0000;-000}");
                    AnsiConsole.Markup("|");

                    for (int xCol = Bounds.minX + 1; xCol < Bounds.maxX; xCol++)
                    {

                        sbyte letter = 0;

                        if ((rowState[yRow] & 1 << xCol - 1) != 0)
                            letter = 1;

                        var setting = config(letter);

                        AnsiConsole.Markup($"[{setting.color.ToMarkup()}]{setting.letter}[/]");
                    }
                    AnsiConsole.Markup("|");

                    Console.WriteLine();
                }
            }

            Console.WriteLine();
        }
    }
}