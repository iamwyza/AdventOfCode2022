using System.Text.RegularExpressions;

namespace AdventOfCode2022.Day11
;
internal class Day11 : DayBase
{
    private Monkey[] _monkeys = null!;
    private delegate long OperationDelegate(long old);

    private long _maxDivisibleBy;

    private class Monkey
    {
        public List<long> Items = new();

        public long Inspections;

        public required OperationDelegate Operation { get; init; }

        public long TestDivisibleBy { get; init; }

        public (int trueMonkey, int falseMonkey) ThrowToMonkey { get; init; }
    }

    private async Task Init()
    {
        var lines = await GetLines();

        _monkeys = new Monkey[(lines.Length + 1) / 7];
        _maxDivisibleBy = 0;
        /*
Monkey 0:
  Starting items: 79, 98
  Operation: new = old * 19
  Test: divisible by 23
    If true: throw to monkey 2
    If false: throw to monkey 3
         */

        Regex operationRegex = new Regex(@"Operation: new = old (\*|\+) (old|\d+)", RegexOptions.Compiled);
        int monkeyIndex = 0;
        for (int i = 0; i < lines.Length; i += 7)
        {
            var match = operationRegex.Match(lines[i + 2]);
            OperationDelegate operation = match.Groups[1].Value switch
            {
                "*" when match.Groups[2].Value == "old" => old => old * old,
                "*" when int.Parse(match.Groups[2].Value) is var offset => old => old * offset,
                "+" when match.Groups[2].Value == "old" => old => old + old,
                "+" when int.Parse(match.Groups[2].Value) is var offset => old => old + offset,
                _ => throw new ArgumentOutOfRangeException()
            };

            var monkey = new Monkey
            {
                Items = lines[i + 1].Replace("  Starting items: ", "").Split(", ").Select(long.Parse).ToList(),
                TestDivisibleBy = int.Parse(lines[i + 3].Split(" ").Last()),
                ThrowToMonkey = (int.Parse(lines[i + 4].Split(" ").Last()), int.Parse(lines[i + 5].Split(" ").Last())),
                Operation = operation
            };

            if (monkey.TestDivisibleBy > _maxDivisibleBy)
            {
                _maxDivisibleBy = monkey.TestDivisibleBy;
            }
            _monkeys[monkeyIndex] = monkey;
            monkeyIndex++;
        }

        _maxDivisibleBy = LCM(_monkeys.Select(x => x.TestDivisibleBy).ToArray());
        //_maxDivisibleBy = (_maxDivisibleBy * _maxDivisibleBy) + 1;
    }

    public override async Task RunPart1()
    {
        PrintStart(1);
        await Init();

        for (int i = 0; i < 20; i++)
        {
            foreach (var monkey in _monkeys)
            {
                foreach (var item in monkey.Items.ToArray())
                {
                    monkey.Inspections++;
                    var worry = monkey.Operation(item) / 3;
                    monkey.Items.Remove(item);
                    if (worry % monkey.TestDivisibleBy == 0)
                    {
                        _monkeys[monkey.ThrowToMonkey.trueMonkey].Items.Add(worry);
                    }
                    else
                    {
                        _monkeys[monkey.ThrowToMonkey.falseMonkey].Items.Add(worry);
                    }
                }
            }

            AnsiConsole.MarkupLine($"After round [green]{i + 1}[/], the monkeys are holding items with these worry levels:");
            PrintState();
            Console.WriteLine();
        }

        for (int i = 0; i < _monkeys.Length; i++)
        {
            AnsiConsole.MarkupLine($"Monkey [green]{i}[/] inspected items [red]{_monkeys[i].Inspections}[/] times.");
        }

        var top = _monkeys.OrderByDescending(x => x.Inspections).Take(2).ToArray();

        AnsiConsole.MarkupLine($"Top [green]2[/] monkeys were {top[0].Inspections} and {top[1].Inspections} which scores {top[0].Inspections * top[1].Inspections}");

    }

    private void PrintState()
    {
        for (int i = 0; i < _monkeys.Length; i++)
        {
            AnsiConsole.MarkupLine($"Monkey [green]{i}[/]: {string.Join(", ", _monkeys[i].Items.Select(x => $"[yellow]{x}[/]"))}");

        }
    }

    public override async Task RunPart2()
    {
        PrintStart(2);
        await Init();
        PrintState();
        Console.WriteLine($"Max divisible by: {_maxDivisibleBy}");

        for (int i = 0; i < 10000; i++)
        {
            int monkeyCount = 0;
            foreach (var monkey in _monkeys)
            {
                foreach (var item in monkey.Items.ToArray())
                {
                    //AnsiConsole.MarkupLine($"Monkey [green]{monkeycount}[/] currently has sent [yellow]{monkey.Inspections}[/]");
                    monkey.Inspections++;

                    var worry = monkey.Operation(item);
                    //AnsiConsole.MarkupLine($"item afterop = [yellow]{worry}[/]");

                    if (worry < 0)
                    {
                        throw new ArgumentOutOfRangeException(nameof(worry));
                    }

                    // Prevent exponential growth by ensuring that any time the value exceeds the Lowest common multiple of all the tests we can divide by that to shrink it down.
                    if (worry >= _maxDivisibleBy)
                    {
                        //AnsiConsole.MarkupLine($"item before = [yellow]{worry}[/] which would have been {(worry % monkey.TestDivisibleBy == 0 ? "[green]true" : "[red]false")}[/]");

                        worry = worry % _maxDivisibleBy;

                        //AnsiConsole.MarkupLine($"item after = [yellow]{worry}[/] which would have been {(worry % monkey.TestDivisibleBy == 0 ? "[green]true" : "[red]false")}[/]");

                        //AnsiConsole.MarkupLine($"item modded {worry}");
                    }


                    //AnsiConsole.MarkupLine($"item aftermod = [yellow]{worry}[/]");

                    //monkey.Items.Remove(item);
                    if (worry % monkey.TestDivisibleBy == 0)
                    {
                        _monkeys[monkey.ThrowToMonkey.trueMonkey].Items.Add(worry);
                    }
                    else
                    {
                        _monkeys[monkey.ThrowToMonkey.falseMonkey].Items.Add(worry);
                    }

                    //PrintState();
                    //Console.WriteLine();

                }
                monkey.Items.Clear();


                monkeyCount++;

            }

            if (i % 1000 == 0)
            {
                AnsiConsole.MarkupLine($"After round [green]{i}[/], the monkeys are holding items with these worry levels:");
                PrintState();
                Console.WriteLine();
            }
        }

        for (int i2 = 0; i2 < _monkeys.Length; i2++)
        {
            AnsiConsole.MarkupLine($"Monkey [green]{i2}[/] inspected items [red]{_monkeys[i2].Inspections}[/] times.");
        }

        var top = _monkeys.OrderByDescending(x => x.Inspections).Take(2).ToArray();

        AnsiConsole.MarkupLine($"Top [green]2[/] monkeys were {top[0].Inspections} and {top[1].Inspections} which scores {top[0].Inspections * top[1].Inspections}");

    }

    // Definitely googled how to calculate lowest common denominator for multiple numbers.  So I take no credit for this little bit of math/code.
    static long LCM(long[] numbers)
    {
        return numbers.Aggregate(lcm);
    }
    static long lcm(long a, long b)
    {
        return Math.Abs(a * b) / GCD(a, b);
    }
    static long GCD(long a, long b)
    {
        return b == 0 ? a : GCD(b, a % b);
    }
}
