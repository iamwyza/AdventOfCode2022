namespace AdventOfCode2022;
internal struct Coord
{
    //private int _x;
    //private int _y;
    //public int XOffset { get; set; }
    //public int YOffset { get; set; }

    public int X;
    //{
    //    get => _x + XOffset;
    //    set => _x = value - XOffset;
    //}

    public int Y;
    //{
    //    get => _y + YOffset;
    //    set => _y = value - YOffset;
    //}

    public Coord()
    {

    }

    public Coord(int x, int y)
    {
        X = x;
        Y = y;
    }

    public override string ToString()
    {
        return String.Format($"{X},{Y}");
    }

}

internal class Grid<T> where T : notnull
{

    public T this[Coord index]
    {
        get => Map[index.X + XOffset, index.Y + YOffset];
        set => Map[index.X + XOffset, index.Y + YOffset] = value;
    }

    public T this[int x, int y]
    {
        get => Map[x + XOffset, y + YOffset];
        set => Map[x + XOffset, y + YOffset] = value;
    }

    public T[,] Map { get; internal set; }

    public (int minX, int minY, int maxX, int maxY) Bounds;

    public static int XOffset;
    public static int YOffset;

    //public Coord MakeCord(int x, int y)
    //{
    //  return new Coord { X = x, Y = y, XOffset = XOffset, YOffset = YOffset};
    //}

    public Func<T, (char letter, Color color)> DefaultPrintConfig { get; set; } = (input) => ('.', Color.Default);

    public void InitMap()
    {
        Map = new T[Bounds.maxX+1, Bounds.maxY+1];
    }

    public void ResetMap(T value)
    {
        for (int yRow = Bounds.minY; yRow <= Bounds.maxY; yRow++)
        {
            for (int xCol = Bounds.minX; xCol <= Bounds.maxX; xCol++)
            {
                Map[xCol, yRow] = value;
            }
        }
    }

    public void CalculateOffsets()
    {
        if (Bounds.minX < 0)
        {
            XOffset = Bounds.minX * -1;
            Bounds.minX = 0;
            Bounds.maxX += XOffset;
        }

        if (Bounds.minY < 0)
        {
            YOffset = Bounds.minY * -1;
            Bounds.minY = 0;
            Bounds.maxY += YOffset;
        }
    }

    public bool InBounds(Coord coord)
    {
        return coord.Y + YOffset >= 0 
               && coord.X + XOffset >= 0 
               && coord.Y + YOffset <= Bounds.maxY 
               && coord.X + XOffset <= Bounds.maxX;
    }

    public void CheckBounds(Coord coord)
    {
        if (coord.X > Bounds.maxX)
            Bounds.maxX = coord.X;
        if (coord.Y > Bounds.maxY)
            Bounds.maxY = coord.Y;
        if (coord.X < Bounds.minX)
            Bounds.minX = coord.X;
        if (coord.Y < Bounds.minY)
            Bounds.minY = coord.Y;
    }


    public void PrintMap(Func<T, (char letter, Color color)>? config)
    {
        config ??= DefaultPrintConfig;
                
        for (int yRow = Bounds.minY; yRow <= Bounds.maxY; yRow++)
        {
            AnsiConsole.Markup($"{yRow:0000;-000}");
            for (int xCol = Bounds.minX; xCol <= Bounds.maxX; xCol++)
            {
                var setting = config(Map[xCol, yRow]);

                AnsiConsole.Markup($"[{setting.color.ToMarkup()}]{setting.letter}[/]");
            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }
}
