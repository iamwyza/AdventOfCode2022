using System.Text.RegularExpressions;

namespace AdventOfCode2022.Day13
;
internal class Day13 : DayBase
{
    private class PacketList
    {
        public Dictionary<double, double[]> Lookups = new Dictionary<double, double[]>();

        public double[] Values { get; set; }

        public string Line { get; set; }
        public string OriginalLine { get; set; }
    }

    private (PacketList left, PacketList right)[] _packets;
    Regex _regex = new Regex(@"(\[[0-9,.-]*\])", RegexOptions.Compiled);

    private async Task Init()
    {
        var lines = await GetLines();
        _packets = new (PacketList left, PacketList right)[(lines.Length / 3) + 1];

        int position = 0;

        for (int i = 0; i < lines.Length - 1; i += 3)
        {
            var left = ParseList(lines[i]);
            var right = ParseList(lines[i + 1]);
            left.OriginalLine = lines[i];
            right.OriginalLine = lines[i + 1];
            left.Values = left.Line.Split(",").Select(double.Parse).ToArray();
            right.Values = right.Line.Split(",").Select(double.Parse).ToArray();


            _packets[position++] = (left, right);
        }
    }

    public override async Task RunPart1()
    {
        PrintStart(1);
        await Init();

        var sum = 0;
        for (int i = 0; i < _packets.Length; i++)
        {
            AnsiConsole.MarkupLine($"== Pair {i + 1} ==");
            var packet = _packets[i];

            var left = packet.left;
            var right = packet.right;

            AnsiConsole.MarkupLineInterpolated($" Comparing [green]{left.OriginalLine}[/] vs [green]{right.OriginalLine}[/]");


            //AnsiConsole.MarkupLine($"[green]{left.Line}[/] vs [green]{right.Line}[/]");
            var isOk = Compare(left.Values, right.Values, left.Lookups, right.Lookups, 0);
            if (isOk ?? true)
            {
                sum += i + 1;
            }
            AnsiConsole.MarkupLine($"Is Correct: [red]{isOk}[/]");
            Console.WriteLine();
        }

        AnsiConsole.MarkupLine($"Sum of Valid Indices: [green]{sum}[/]");
    }


    private bool? Compare(double[] left, double[] right, Dictionary<double, double[]> leftLookup, Dictionary<double, double[]> rightLookup, int depth)
    {
        string indent = new string(' ', depth * 2);

        
        for (int i = 0; i < left.Length; i++)
        {
            if (right.Length > i && (left[i] < 0 || right[i] < 0)) // one of these is a list, convert the other to a lookup
            {
                var leftCompare = left[i] < 0 ? leftLookup[left[i]] : new[] { left[i] };
                var rightCompare = right[i] < 0 ? rightLookup[right[i]] : new[] { right[i] };
                string leftString = DereferenceString(leftCompare, leftLookup);
                string rightString = DereferenceString(rightCompare, rightLookup);

                //AnsiConsole.MarkupLineInterpolated($"{indent}- Compare [yellow]{string.Join(',', leftCompare)}[/] vs [yellow]{string.Join(',', rightCompare)}[/]");

                AnsiConsole.MarkupLineInterpolated($"{indent}- Compare [yellow]{leftString}[/] vs [yellow]{rightString}[/]");

                var result = Compare(leftCompare, rightCompare, leftLookup, rightLookup, depth + 2);
                if (result.HasValue)
                    return result.Value;

            }else if (right.Length > i)
            {
                AnsiConsole.MarkupLine($"{indent}  - Compare [yellow]{left[i]}[/] vs [yellow]{right[i]}[/]");
                if (left[i] < right[i])
                {
                    AnsiConsole.MarkupLine($"{indent}    - [green]Left side[/] is [red]smaller[/], so inputs are [white]in the right order[/]");
                    return true;
                }

                if (left[i] > right[i])
                {
                    AnsiConsole.MarkupLine($"{indent}    - [green]right side[/] is [red]smaller[/], so inputs are [white]not in the right order[/]");
                    return false;
                }

                continue;
            }
            else
            {
                AnsiConsole.MarkupLine($"{indent}    - [green]Right side[/] ran out of items, so inputs are [white]not in the right order[/]");
                return false;
            }
        }

        if (left.Length < right.Length)
        {
            AnsiConsole.MarkupLine($"{indent}    - [green]Left side[/] ran out of items, so inputs are [white]in the right order[/]");
            return true;
        }

        return null;
    }

    private PacketList ParseList(string line)
    {
        var packetList = new PacketList();
        //AnsiConsole.MarkupLineInterpolated($"Parsing: [yellow]{line}[/]");
        Match? match = null;
        double id = 1;
        while (match == null || match.Success)
        {
            match = _regex.Match(line);
            if (match.Success)
            {
                for (int m = 1; m < match.Groups.Count; m++)
                {
                    string value = match.Groups[m].Value;
                    //AnsiConsole.MarkupLineInterpolated($"Matched: [yellow]{value}[/] in [green]{line}[/]");
                    double marker = -id / 100;
                    packetList.Lookups.Add(marker, value.Substring(1, value.Length - 2).Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(double.Parse).ToArray());
                    line = line.Replace(value, marker.ToString());
                    //AnsiConsole.MarkupLineInterpolated($"Line is now: [green]{line}[/]");
                    id++;
                }
            }
        }

        packetList.Line = line.Replace("[", "").Replace("]", "");
        return packetList;
    }
    private Regex _deReferenceRegex = new Regex(@"(-0\.\d+)", RegexOptions.Compiled);

    private string DereferenceString(double[] values, Dictionary<double, double[]> lookup)
    {
        string line = "[" + string.Join(',', values) + "]";

        Match? match = null;
        while (match == null || match.Success)
        {
            match = _deReferenceRegex.Match(line);
            if (match.Success)
            {
                string value = match.Groups[1].Value.Replace(",","").Replace("]", "");
                //AnsiConsole.MarkupLineInterpolated($"{line}");
                line = _deReferenceRegex.Replace(line, "[" + string.Join(',', lookup[double.Parse(value)]) + "]", 1);
            }
        }

        return line;
    }

    public override async Task RunPart2()
    {
        PrintStart(2);
        await Init();

        var sum = 0;
        var allPackets = _packets.Select(x => x.left).Concat(_packets.Select(x => x.right)).ToList();

        var dividerA = ParseList("[[2]]");
        var dividerB = ParseList("[[6]]");
        dividerA.OriginalLine = "[[2]]";
        dividerB.OriginalLine = "[[6]]";
        dividerA.Values = dividerA.Line.Split(",").Select(double.Parse).ToArray();
        dividerB.Values = dividerB.Line.Split(",").Select(double.Parse).ToArray();
        allPackets.Add(dividerA);
        allPackets.Add(dividerB);

        allPackets.Sort(ComparePacket);

        int key = 1;
        for (int i = 0; i < allPackets.Count; i++)
        {
            var packet = allPackets[i];

            AnsiConsole.MarkupLineInterpolated($"{packet.OriginalLine}");
            if (packet.OriginalLine is "[[2]]" or "[[6]]")
            {
                key *= (i + 1);
            }
        }

        AnsiConsole.MarkupLine($"Decoder Key: [green]{key}[/]");
    }

    private int ComparePacket(PacketList a, PacketList b)
    {
        var result = Compare(a.Values, b.Values, a.Lookups, b.Lookups, 0);
        return (result ?? true) ? -1 : 1;
    }
}