namespace AdventOfCode2022.Day9;
internal class Solution : DayBase
{
    enum Direction
    {
        R,
        L,
        U,
        D
    }

    private List<KeyValuePair<Direction, short>> _steps;
    private HashSet<string> _visited;
    private int xMin = 0;
    private int yMin = 0;
    private int xMax = 0;
    private int yMax = 0;

    private async Task Init()
    {
        var lines = await GetLines();
        _steps = new List<KeyValuePair<Direction, short>>();
        _visited = new();

        foreach (var line in lines)
        {
            var temp = line.Split(" ");
            _steps.Add(new KeyValuePair<Direction, short>(Enum.Parse<Direction>(temp[0]), short.Parse(temp[1])));
        }
    }

    public override async Task RunPart1()
    {
        PrintStart(1);
        await Init();

        (int X, int Y) head = (0, 0);
        (int X, int Y) tail = (0, 0);

        foreach (var (direction, count) in _steps)
        {
            int offset = direction is Direction.L or Direction.U ? -1 : 1;

            Console.WriteLine($"== {direction} {count} ==");

            for (int i = 0; i < count; i++)
            {
                if (direction is Direction.L or Direction.R)
                {
                    head.X += offset;
                }
                else
                {
                    head.Y += offset;
                }

                int distanceY = head.Y - tail.Y;
                int distanceX = head.X - tail.X;

                if (Math.Abs(distanceX) + Math.Abs(distanceY) == 3)
                {
                    tail.Y += distanceY > 0 ? 1 : -1;
                    tail.X += distanceX > 0 ? 1 : -1;
                }
                else if (Math.Abs(distanceY) > 1)
                {
                    tail.Y += distanceY > 0 ? 1 : -1;
                }
                else if (Math.Abs(distanceX) > 1)
                {
                    tail.X += distanceX > 0 ? 1 : -1;
                }

                if (!_visited.Contains(tail.X + "," + tail.Y))
                    _visited.Add(tail.X + "," + tail.Y);

                if (xMin > head.X)
                    xMin = head.X;
                if (xMax < head.X)
                    xMax = head.X;

                if (yMin > head.Y)
                    yMin = head.Y;
                if (yMax < head.Y)
                    yMax = head.Y;
                //PrintState(head, tail);

            }
        }

        int visited = PrintVisitedState();

        Console.WriteLine($"Tail visited {visited} unique locations.");
    }





    public override async Task RunPart2()
    {
        PrintStart(2);
        await Init();

        (int X, int Y)[] rope = new (int X, int Y)[10];
        PrintState(rope);

        foreach (var (direction, count) in _steps)
        {
            int offset = direction is Direction.L or Direction.U ? -1 : 1;

           // Console.WriteLine($"== {direction} {count} ==");

            for (int i = 0; i < count; i++)
            {
                if (direction is Direction.L or Direction.R)
                {
                    rope[0].X += offset;
                }
                else
                {
                    rope[0].Y += offset;
                }

                for (int r = 0; r < rope.Length - 1; r++)
                {
                    int distanceY = rope[r].Y - rope[r + 1].Y;
                    int distanceX = rope[r].X - rope[r + 1].X;

                    //Console.WriteLine($"Distance for {r} to {r+1}:  {rope[r].X},{rope[r].Y} to {rope[r+1].X},{rope[r+1].Y} X,Y = {distanceX},{distanceY}");

                    if (Math.Abs(distanceX) + Math.Abs(distanceY) >= 3)
                    {
                        rope[r + 1].Y += distanceY > 0 ? 1 : -1;
                        rope[r + 1].X += distanceX > 0 ? 1 : -1;
                    }
                    else if (Math.Abs(distanceY) > 1)
                    {
                        rope[r + 1].Y += distanceY > 0 ? 1 : -1;
                    }
                    else if (Math.Abs(distanceX) > 1)
                    {
                        rope[r + 1].X += distanceX > 0 ? 1 : -1;
                    }
                    //PrintState(rope);


                }

                if (!_visited.Contains(rope[9].X + "," + rope[9].Y))
                    _visited.Add(rope[9].X + "," + rope[9].Y);

                if (xMin > rope[0].X)
                    xMin = rope[0].X;
                if (xMax < rope[0].X)
                    xMax = rope[0].X;

                if (yMin > rope[0].Y)
                    yMin = rope[0].Y;
                if (yMax < rope[0].Y)
                    yMax = rope[0].Y;

            }
               // PrintState(rope);
        }

        int visited = PrintVisitedState();

        Console.WriteLine($"Tail visited {visited} unique locations.");

    }

    private void PrintState((int X, int Y)[] rope)
    {
        for (int y = yMin; y <= yMax; y++)
        {
            //Console.Write(y.ToString().PadRight(3));
            for (int x = xMin; x <= xMax; x++)
            {
                char part = '.';

                for (int r = rope.Length - 1; r >= 0; r--)
                {
                    var temp = r == 0 ? 'H' : r.ToString()[0];

                    if (rope[r].X == x && rope[r].Y == y)
                    {
                        part = temp;
                    }
                    else if (rope[r].X == x && rope[r].Y == y)
                    {
                        part = temp;
                    }
                }

                Console.Write(part);

            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }

    private int PrintVisitedState()
    {
        int visited = 0;
        for (int y = yMin; y <= yMax; y++)
        {
            for (int x = xMin; x <= xMax; x++)
            {

                if (_visited.Contains(x + "," + y))
                {
                    visited++;
                    Console.Write('#');

                }
                else
                {
                    Console.Write('.');
                }
            }
            Console.WriteLine();
        }
        Console.WriteLine();

        return visited;
    }

    //    private int IsTouching()
    //    {
    //        int offset = direction is Direction.Left or Direction.Up ? -1 : 1;

    //        if (direction is Direction.Left or Direction.Right)
    //        {

    //        }
    //        else
    //        {

    //        }
    //    }
}
