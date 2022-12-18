using System.Diagnostics;

namespace AdventOfCode2022;
internal struct Coord
{

    public int X;


    public int Y;

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

internal class Grid<T> where T : struct, IComparable, IComparable<T>
{

    public T this[Coord index]
    {
        get => Map[GetXIndex(index.X), GetYIndex(index.Y)];
        set => Map[GetXIndex(index.X), GetYIndex(index.Y)] = value;
    }

    public T this[int x, int y]
    {
        get => Map[GetXIndex(x), GetYIndex(y)];
        set => Map[GetXIndex(x), GetYIndex(y)] = value;
    }

    //public T this[Coord index]
    //{
    //    get => Map[index.X + XOffset, index.Y + YOffset];
    //    set => Map[index.X + XOffset, index.Y + YOffset] = value;
    //}

    //public T this[int x, int y]
    //{
    //    get => Map[x + XOffset, y + YOffset];
    //    set => Map[x + XOffset, y + YOffset] = value;
    //}

    private int _xLength;
    private int _yLength;

    private int GetXIndex(int x)
    {
        if (!CanWrapX) return x + XOffset;

        if (x + XOffset < 0)
        {
            return _xLength + x + XOffset;
        }

        if (x + XOffset > _xLength)
        {
            return _xLength + x + XOffset;
        }

        return x + XOffset;
    }

    private int GetYIndex(int y)
    {
        //if (!CanWrapY) return y + YOffset;

        if (y + YOffset < 0)
        {
            //if (y == -16188) Debugger.Break();
            if (y % _yLength != 0) 
                return (_yLength + (y % _yLength) + YOffset) ;

            return YOffset; // Y = 0 + offset
        }

        if (y + YOffset >= _yLength)
        {
            if (y % _yLength != 0)
                return y % _yLength + YOffset;

            return YOffset;
        }

        return y + YOffset;
    }


    public bool CanWrapY = false;
    public bool CanWrapX = false;

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
        Map = new T[Bounds.maxX + 1, Bounds.maxY + 1];
        _xLength = Map.GetLength(0);
        _yLength = Map.GetLength(1);
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


    public void PrintMap(Func<T, (char letter, Color color)>? config = null, bool printRowLabel = false, bool printEmptyRow = true, int yMin = 0, int yMax = 0)
    {
        config ??= DefaultPrintConfig;

        for (int yRow = Bounds.minY; yRow <= Bounds.maxY; yRow++)
        {


            bool hasData = false;

            if (!printEmptyRow)
            {
                for (int xCol = Bounds.minX; xCol <= Bounds.maxX; xCol++)
                {
                    if (!Map[xCol, yRow].Equals(default(T)))
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
                    var setting = config(Map[xCol, yRow]);

                    AnsiConsole.Markup($"[{setting.color.ToMarkup()}]{setting.letter}[/]");
                }
                Console.WriteLine();
            }
        }

        Console.WriteLine();
    }
}
