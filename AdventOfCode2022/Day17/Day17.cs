using System.Diagnostics;

namespace AdventOfCode2022.Day17;
internal class Day17 : DayBase
{

    private Grid<sbyte> _map;

    private readonly sbyte[][,] _pieces = new[]
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

    //true = left, false = right
    private bool[] _moves;

    private async Task Init()
    {
        _map = new Grid<sbyte>();
        _map.CheckBounds(new Coord(0, 0));
        _map.CheckBounds(new Coord(8, (2022 * 4) + 5)); // Max height of the cave is max piece height * 2022 (number of pieces that will fall)

        var lines = await GetLines();

        _moves = new bool[lines[0].Length];

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
        _map.InitMap();

        for (int y = _map.Bounds.maxY; y > 0; y--)
        {
            if (y == _map.Bounds.maxY)
            {
                for (int x = 1; x < _map.Bounds.maxX; x++)
                {
                    _map[x, y] = 3;
                }

                _map[0, y] = 4;
                _map[_map.Bounds.maxX, y] = 4;
            }
            else
            {
                _map[0, y] = 2;
                _map[_map.Bounds.maxX, y] = 2;
            }
        }

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

    public override async Task RunPart1()
    {
        PrintStart(1);
        await Init();

        int movementIndex = 0;
        int pieceIndex = 0;
        int highestRock = _map.Bounds.maxY;
        char[] movesChars = _moves.Select(x => x ? '<' : '>').ToArray();

        for (int i = 0; i < 2022; i++)
        {
            var piece = _pieces[pieceIndex++];
            if (pieceIndex > _pieces.Length - 1)
                pieceIndex = 0;

            var bottomLeftCord = new Coord(3, highestRock - 4);

            //AnsiConsole.MarkupLine($"Highest rock is now [yellow]{highestRock}[/]");

            while (true)
            {
                sbyte offset = (sbyte)(_moves[movementIndex++] ? -1 : 1);
                if (movementIndex > _moves.Length - 1)
                    movementIndex = 0;

                //AnsiConsole.Markup($"Try to move [yellow]{(offset == 1 ? 'R' : 'L' )}[/] : ");

                //Debugging to show first state (doesn't actually try to move the piece)
                CanMove(bottomLeftCord, 0, 0, piece);
                Printmap(i);
                ResetTemp(bottomLeftCord, 0, 0, piece);


                if (CanMove(bottomLeftCord, offset, 0, piece))
                {
                    //AnsiConsole.MarkupLine("[green]Success[/]");
                    Printmap(i);
                    ResetTemp(bottomLeftCord, offset, 0, piece);
                    bottomLeftCord.X += offset;
                }
                else
                {
                    //AnsiConsole.MarkupLine("[red]Failure[/]");
                    Printmap(i);
                    ResetTemp(bottomLeftCord, offset, 0, piece);
                }



                //AnsiConsole.Markup($"Try to move [yellow]Down[/] : ");

                if (CanMove(bottomLeftCord, 0, 1, piece))
                {
                    //AnsiConsole.MarkupLine("[green]Success[/]");
                    Printmap(i);
                    ResetTemp(bottomLeftCord, 0, 1, piece);
                    bottomLeftCord.Y += 1;
                }
                else
                {
                    //AnsiConsole.MarkupLine("[red]Failure[/]");
                    Printmap(i);
                    ResetTemp(bottomLeftCord, 0, 1, piece);
                    SetPiece(bottomLeftCord, piece, ref highestRock);
                    Printmap(i);
                    break;
                }

            }
        }

        _map.PrintMap(null, true, false);

        AnsiConsole.MarkupLine($"The height of the rocks is [green]{_map.Bounds.maxY - highestRock}[/]");

        void Printmap(int row)
        {
            return;
            AnsiConsole.Clear();
            Console.WriteLine();
            Console.WriteLine();
            AnsiConsole.MarkupLine($"Rock # [green]{row}[/]");
           // AnsiConsole.MarkupLine($"[grey]{string.Join("",movesChars[(movementIndex-20 < 0 ? 0 : movementIndex - 20)..movementIndex])}[/][green]{movesChars[movementIndex]}[/][grey]{string.Join("", movesChars[movementIndex..( movementIndex + 20 > movesChars.Length ? movesChars.Length :  movementIndex +20)])}[/]");
            Console.WriteLine();
            _map.PrintMap(null, true, false, highestRock - 5, highestRock + 20);
            Thread.Sleep(50);
        }
    }

    private bool SetPiece(Coord bottomLeftCord, sbyte[,] piece, ref int highestRock)
    {
        for (int x = 0; x < piece.GetLength(0); x++)
        {
            for (int y = 0; y < piece.GetLength(1); y++)
            {
                int actualY = bottomLeftCord.Y + (y - piece.GetLength(1) + 1);

                if (piece[x, y] > 0)
                {
                    _map[bottomLeftCord.X + x, actualY] = piece[x, y];
                }
                if (bottomLeftCord.Y - y < highestRock)
                    highestRock = bottomLeftCord.Y - y;
            }
        }

        return true;
    }

    private bool CanMove(Coord bottomLeftCord, sbyte xOffset, byte yOffset, sbyte[,] piece)
    {
        bool canMove = true;
        for (int x = 0; x < piece.GetLength(0); x++)
        {
            for (int y = 0; y < piece.GetLength(1); y++)
            {
                int actualY = bottomLeftCord.Y + yOffset + (y - piece.GetLength(1) + 1);
                var value = _map[bottomLeftCord.X + x + xOffset, actualY] + piece[x, y];
                if (value == 5)
                    value = 0;

                if (value > 1)
                {
                    canMove = false;
                }

                // for visualizing the movements, not really needed to calculate them.
                //_map[bottomLeftCord.X + x + xOffset, actualY] = (sbyte)(piece[x, y] == 1 ? _map[bottomLeftCord.X + x + xOffset, actualY] + 5 : _map[bottomLeftCord.X + x + xOffset, actualY]);
            }
        }

        return canMove;
    }

    private void ResetTemp(Coord bottomLeftCord, sbyte xOffset, byte yOffset, sbyte[,] piece)
    {
        for (int x = 0; x < piece.GetLength(0); x++)
        {
            for (int y = 0; y < piece.GetLength(1); y++)
            {
                int actualY = bottomLeftCord.Y + yOffset + (y - piece.GetLength(1) + 1);

                if (_map[bottomLeftCord.X + x + xOffset, actualY] >= 5)
                {
                    _map[bottomLeftCord.X + x + xOffset, actualY] -= 5;
                }
            }
        }
    }

    public override async Task RunPart2()
    {
        PrintStart(2);
        await Init();

        int movementIndex = 0;
        int pieceIndex = 0;
        int highestRock = _map.Bounds.maxY;
        char[] movesChars = _moves.Select(x => x ? '<' : '>').ToArray();
        int loop = 0;

        for (int i = 0; i < 1_000_000_000; i++)
        {
            var piece = _pieces[pieceIndex++];
            if (pieceIndex > _pieces.Length - 1)
                pieceIndex = 0;

            // If we've looped around a few times, we should reset the highestRock down (to keep the numbers more managable. 
            if (highestRock < -_map.Bounds.maxY - 10)
            {
                loop++;
                highestRock = (highestRock % _map.Bounds.maxY) + _map.Bounds.maxY;
                
                
            }

            if (i % 200 == 0 && loop > 0)
            {
                // clear the rows ahead since we might have looped.
                for (int zy = highestRock; zy > highestRock - 1000; zy--)
                {
                    for (int x = 1; x < _map.Bounds.maxX; x++)
                    {
                        _map[x, zy] = 0;
                    }
                }
                //AnsiConsole.Clear();
                //_map.PrintMap();
            }
           

            var bottomLeftCord = new Coord(3, highestRock - 4);

            if (highestRock < -10000)
            {
                Debugger.Break();
            }
            //AnsiConsole.MarkupLine($"Highest rock is now [yellow]{highestRock}[/]");

            if (i % 1_000_000 == 0)
            {
                AnsiConsole.MarkupLine($"Rock # [yellow]{i}[/]");
            }

            while (true)
            {
                sbyte offset = (sbyte)(_moves[movementIndex++] ? -1 : 1);
                if (movementIndex > _moves.Length - 1)
                    movementIndex = 0;

                //AnsiConsole.Markup($"Try to move [yellow]{(offset == 1 ? 'R' : 'L' )}[/] : ");

                //Debugging to show first state (doesn't actually try to move the piece)
                CanMove(bottomLeftCord, 0, 0, piece);
                //Printmap(i);
                //ResetTemp(bottomLeftCord, 0, 0, piece);


                if (CanMove(bottomLeftCord, offset, 0, piece))
                {
                    //AnsiConsole.MarkupLine("[green]Success[/]");
                    //Printmap(i);
                    //ResetTemp(bottomLeftCord, offset, 0, piece);
                    bottomLeftCord.X += offset;
                }
                else
                {
                    //AnsiConsole.MarkupLine("[red]Failure[/]");
                    //Printmap(i);
                    //ResetTemp(bottomLeftCord, offset, 0, piece);
                }



                //AnsiConsole.Markup($"Try to move [yellow]Down[/] : ");

                if (CanMove(bottomLeftCord, 0, 1, piece))
                {
                    //AnsiConsole.MarkupLine("[green]Success[/]");
                    //Printmap(i);
                    //ResetTemp(bottomLeftCord, 0, 1, piece);
                    bottomLeftCord.Y += 1;
                }
                else
                {
                    //AnsiConsole.MarkupLine("[red]Failure[/]");
                    //Printmap(i);
                    //ResetTemp(bottomLeftCord, 0, 1, piece);
                    SetPiece(bottomLeftCord, piece, ref highestRock);
                    //Printmap(i);
                    break;
                }

            }
        }

        _map.PrintMap(null, true, false);

        AnsiConsole.MarkupLine($"The height of the rocks is [green]{_map.Bounds.maxY - highestRock}[/]");

        void Printmap(int row)
        {
            return;
            if (row < 5100) return;
            AnsiConsole.Clear();
            Console.WriteLine();
            Console.WriteLine();
            AnsiConsole.MarkupLine($"Rock # [green]{row}[/]");
            // AnsiConsole.MarkupLine($"[grey]{string.Join("",movesChars[(movementIndex-20 < 0 ? 0 : movementIndex - 20)..movementIndex])}[/][green]{movesChars[movementIndex]}[/][grey]{string.Join("", movesChars[movementIndex..( movementIndex + 20 > movesChars.Length ? movesChars.Length :  movementIndex +20)])}[/]");
            Console.WriteLine();
            _map.PrintMap(null, true, false, highestRock - 5, highestRock + 20);
            //Thread.Sleep(50);
        }
    }


}