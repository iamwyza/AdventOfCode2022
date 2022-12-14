using System.Reflection.Metadata;

namespace AdventOfCode2022.Day8;
internal class Solution : DayBase
{
    private int[,] _trees;
    private int[,] _scores;
    private bool[,] _visibleTrees;
    private int _rows;
    private int _cols;

    private async Task Init()
    {
        var lines = await GetLines();
        _rows = lines.Length;
        _cols = lines[0].Length;

        _trees = new int[_rows, _cols];
        _visibleTrees = new bool[_rows, _cols];
        _scores = new int[_rows, _cols];


        for (int y = 0; y < _rows; y++)
        {
            for (int x = 0; x < _cols; x++)
            {
                _trees[y, x] = int.Parse(lines[y][x].ToString());
                //Console.Write(_trees[y, x]);
            }
            //Console.WriteLine();
        }
    }

    public override async Task RunPart1()
    {
        PrintStart(1);
        await Init();

        Console.WriteLine();
        // LTR
        for (int y = 0; y < _rows; y++)
        {
            int highest = -1;

            for (int x = 0; x < _cols; x++)
            {
                if (CheckTree(x, y, ref highest))
                    break;
            }
        }

        PrintTrees(_visibleTrees);

        // RTL
        for (int y = 0; y < _rows; y++)
        {
            int highest = -1;

            for (int x = _cols - 1; x > 0; x--)
            {
                if (CheckTree(x, y, ref highest))
                    break;
            }
        }

        PrintTrees(_visibleTrees);


        // TTB
        for (int x = 0; x < _cols; x++)
        {
            int highest = -1;

            for (int y = 0; y < _rows; y++)
            {
                if (CheckTree(x, y, ref highest))
                    break;
            }
        }

        PrintTrees(_visibleTrees);


        // BTT
        for (int x = 0; x < _cols; x++)
        {
            int highest = -1;

            for (int y = _rows - 1; y > 0; y--)
            {
                if (CheckTree(x, y, ref highest))
                    break;
            }
        }

        PrintTrees(_visibleTrees);


        var visible = 0;
        for (int y = 0; y < _rows; y++)
        {
            for (int x = 0; x < _cols; x++)
            {
                if (_visibleTrees[x, y])
                {
                    visible++;
                }
            }
            Console.WriteLine();
        }


        Console.WriteLine($"Visible Trees = {visible}");
    }

    public override async Task RunPart2()
    {
        PrintStart(2);
        await Init();

        int highestScore = 0;
        (int X, int Y) bestCoords = (0, 0);
        bool[,] bestVisibleTrees = new bool[0,0];

        for (int y = 0; y < _rows; y++)
        {
            for (int x = 0; x < _cols; x++)
            {
               // Console.WriteLine($"{x}, {y} ({_trees[y,x]})");
                _scores[y, x] = Walk(x, y, Direction.Left) 
                                * Walk(x, y, Direction.Right)
                                * Walk(x, y, Direction.Up)
                                * Walk(x, y, Direction.Down);

                if (_scores[y, x] > highestScore)
                {
                    highestScore = _scores[y, x];
                    bestCoords.X = x; 
                    bestCoords.Y = y;
                    bestVisibleTrees = _visibleTrees;
                }
                _visibleTrees = new bool[_rows, _cols];
            }
        }

        _visibleTrees = bestVisibleTrees;
        Console.WriteLine($"Best Scoring Tree {highestScore}");

        PrintTreesCanvas(bestCoords.X, bestCoords.Y);
    }

    private int Walk(int xStart, int yStart, Direction direction)
    {
        int offset;
        int distance = 0;

        switch (direction)
        {
            case Direction.Left:
            case Direction.Up:
                offset = -1;
                break;
            case Direction.Right:
            case Direction.Down:
                offset = 1;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
        }

        int highest = _trees[yStart, xStart];

        if (direction is Direction.Left or Direction.Right)
        {
            int x = xStart;
            while (true)
            {
                x += offset;

                if (x < 0 || x > _trees.GetLength(0) - 1) break;
                distance++;
                if (CheckTree2(x, yStart, ref highest)) break;
            }
        }
        else
        {
            int y = yStart;
            while (true)
            {
                y += offset;

                if (y < 0 || y > _trees.GetLength(1) - 1) break;
                distance++;
                if (CheckTree2(xStart, y, ref highest)) break;

            }
        }

        //Console.WriteLine($"{direction} = {distance}");

        return distance;
    }

    enum Direction
    {
        Left,
        Right,
        Up,
        Down
    }

    // Returns true if tree is max height so we can stop checking along this path.
    private bool CheckTree(int x, int y, ref int highest)
    {
        int tree = _trees[y, x];

        if (tree <= highest)
            return false;

        _visibleTrees[y, x] = true;
        highest = tree;
        if (highest == 9)
        {
            return true;
        }

        return false;
    }

    // Returns true if tree is max height so we can stop checking along this path.
    private bool CheckTree2(int x, int y, ref int highest)
    {
        int tree = _trees[y, x];

        if (tree >= highest)
            return true;

        _visibleTrees[y, x] = true;

        return false;
    }

    private void PrintTrees(bool[,] trees)
    {
        return;
        for (int x = 0; x < trees.GetLength(0); x++)
        {
            for (int y = 0; y < trees.GetLength(1); y++)
            {
                Console.Write(trees[x, y] ? 'T' : 'F');
            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }


    private void PrintTreesCanvas(int startX, int startY)
    {
        var canvas = new Canvas(_cols, _rows);
        for (int y = 0; y<_trees.GetLength(0); y++)
        {
            for (int x = 0; x< _trees.GetLength(1); x++)
            {
                if (_visibleTrees[y, x])
                {
                    var heightColor = (byte)(200 - _trees[y, x] * 15);

                    canvas.SetPixel(x, y, new Color(0, 0, heightColor));
                }
                else
                {
                    var heightColor = (byte)(200 - _trees[y, x] * 15);
                    canvas.SetPixel(x, y, new Color(0, heightColor, 0));
                }
            }
        }

        //canvas.PixelWidth = 1;
        canvas.SetPixel(startX, startY, Color.Red);

        AnsiConsole.Write(canvas);
    }
}
