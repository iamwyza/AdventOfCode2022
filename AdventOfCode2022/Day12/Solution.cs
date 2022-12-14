namespace AdventOfCode2022.Day12;
internal class Solution : DayBase
{
    private int[,] _map;
    private int _rows;
    private int _cols;
    private (int X, int Y) _destination;
    private (int X, int Y) _start;

    private async Task Init()
    {
        var lines = await GetLines();
        _rows = lines.Length;
        _cols = lines[0].Length;

        _map = new int[_rows, _cols];


        for (int y = 0; y < _rows; y++)
        {
            for (int x = 0; x < _cols; x++)
            {
                switch (lines[y][x])
                {
                    case 'S':
                        _start = (x, y);
                        _map[y, x] = 1;
                        break;
                    case 'E':
                        _destination = (x, y);
                        _map[y, x] = 26;
                        break;
                    default:
                        _map[y, x] = lines[y][x] - 96;
                        break;
                }
                //Console.Write(_trees[y, x]);
            }
            //Console.WriteLine();
        }
    }


    public override async Task RunPart1()
    {
        PrintStart(1);
        await Init();

        AnsiConsole.MarkupLine($"Trying to get to [yellow]{_destination.Y}[/],[yellow]{_destination.X}[/] from [yellow]{_start.Y}[/],[yellow]{_start.X}[/]");

        MakeGraph();

        var test = Move2((_start.X, _start.Y));
        var best = test((_destination.X, _destination.Y)).Count() - 1;

        AnsiConsole.MarkupLine($"Lowest number of moves is [green]{best}[/]");

    }

    public override async Task RunPart2()
    {
        PrintStart(2);
        await Init();


        AnsiConsole.MarkupLine($"Trying to get to [yellow]{_destination.Y}[/],[yellow]{_destination.X}[/] from [yellow]{_start.Y}[/],[yellow]{_start.X}[/]");

        MakeGraph();
        List<(int X, int Y)> aCells = new List<(int X, int Y)>();
        for (int x = 0; x < _cols; x++)
        {
            for (int y = 0; y < _rows; y++)
            {
                if (_map[y, x] == 1)
                {
                    aCells.Add((x, y));
                }
            }
        }

        var best = 1000000;

        foreach (var cell in aCells)
        {
            var test = Move2((cell));

            var distance = test(_destination).Count() - 1;
            if (distance > 0)
            {
                AnsiConsole.MarkupLine($"Distance from [yellow]{cell.Y}[/],[yellow]{cell.X}[/] to [yellow]{_destination.Y}[/],[yellow]{_destination.X}[/] is {distance}");
                if (best > distance)
                    best = distance;
            }
        }


        AnsiConsole.MarkupLine($"Lowest number of moves is [green]{best}[/]");
    }

    private Queue<(int X, int Y)> _cells;

    public Dictionary<(int X, int Y), List<(int X, int Y)>> _mapGraph;

    private void MakeGraph()
    {
        _mapGraph = new Dictionary<(int X, int Y), List<(int X, int Y)>>();

        for (int x = 0; x < _cols; x++)
        {
            for (int y = 0; y < _rows; y++)
            {
                var key = (x, y);
                _mapGraph.Add(key, new());

                if (x > 0 && _map[y, x - 1] - _map[y, x] <= 1)
                {
                    _mapGraph[key].Add((x - 1, y));
                }

                if (x < _cols - 1 && _map[y, x + 1] - _map[y, x] <= 1)
                {
                    _mapGraph[key].Add((x + 1, y));
                }

                if (y > 0 && _map[y - 1, x] - _map[y, x] <= 1)
                {
                    _mapGraph[key].Add((x, y - 1));
                }

                if (y < _rows - 1 && _map[y + 1, x] - _map[y, x] <= 1)
                {
                    _mapGraph[key].Add((x, y + 1));
                }
            }
        }
    }

    private Func<(int X, int Y), IEnumerable<(int X, int Y)>> Move2((int X, int Y) start)
    {
        var previous = new Dictionary<(int X, int Y), (int X, int Y)>();

        _cells = new Queue<(int X, int Y)>();
        _cells.Enqueue(start);

        while (_cells.Count > 0)
        {
            (int x, int y) vertex = _cells.Dequeue();

            foreach (var cell in _mapGraph[vertex])
            {
                if (previous.ContainsKey(cell))
                    continue;

                previous[cell] = vertex;
                _cells.Enqueue(cell);
            }
        }

        Func<(int X, int Y), IEnumerable<(int X, int Y)>> shortestPath = v => {
            var path = new List<(int X, int Y)> { };

            var current = v;
            while (!current.Equals(start))
            {
                path.Add(current);
                if (!previous.ContainsKey(current))
                    return Array.Empty<(int X, int Y)>();

                current = previous[current];
            };

            path.Add(start);
            path.Reverse();

            return path;
        };

        return shortestPath;

    }

    private void PrintTemp(char[,] temp)
    {
        for (int yRow = 0; yRow < _map.GetLength(0); yRow++)
        {
            for (int xCol = 0; xCol < _map.GetLength(1); xCol++)
            {
                Console.Write(temp[yRow, xCol] == '\0' ? '.' : temp[yRow, xCol]);
            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }

}
