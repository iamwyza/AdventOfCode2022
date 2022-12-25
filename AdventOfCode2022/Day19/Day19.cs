using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static AdventOfCode2022.Day16.Day16;

namespace AdventOfCode2022.Day19;
internal class Day19 : DayBase
{

    private class Blueprint
    {
        public int OreRobotCost;
        public int ClayRobotCost;
        public int ObsidianRobotOreCost;
        public int ObsidianRobotClayCost;
        public int GeodeRobotOreCost;
        public int GeodeRobotObsidianCost;

        public int MaxGeodeCount = 0;
    }

    private abstract class Robot
    {
        protected readonly Blueprint Blueprint;

        protected Robot(Blueprint blueprint)
        {
            Blueprint = blueprint;
        }

        public abstract bool CanMake(State state);

        public abstract void Make(State state);
    }

    private class OreRobot : Robot
    {
        public OreRobot(Blueprint blueprint) : base(blueprint)
        {
        }

        public override bool CanMake(State state)
        {
            return Blueprint.OreRobotCost <= state.Ore;
        }

        public override void Make(State state)
        {
            state.OreRobot++;
            state.Ore -= Blueprint.OreRobotCost;
        }
    }

    private class ClayRobot : Robot
    {
        public ClayRobot(Blueprint blueprint) : base(blueprint)
        {
        }

        public override bool CanMake(State state)
        {
            return Blueprint.ClayRobotCost <= state.Ore;
        }

        public override void Make(State state)
        {
            state.ClayRobot++;
            state.Ore -= Blueprint.ClayRobotCost;
        }
    }

    private class ObsidianRobot : Robot
    {
        public ObsidianRobot(Blueprint blueprint) : base(blueprint)
        {
        }

        public override bool CanMake(State state)
        {
            return Blueprint.ObsidianRobotOreCost <= state.Ore && Blueprint.ObsidianRobotClayCost <= state.Clay;
        }

        public override void Make(State state)
        {
            state.ObsidianRobot++;
            state.Ore -= Blueprint.ObsidianRobotOreCost;
            state.Clay -= Blueprint.ObsidianRobotClayCost;
        }
    }
    private class GeodeRobot : Robot
    {
        public GeodeRobot(Blueprint blueprint) : base(blueprint)
        {
        }

        public override bool CanMake(State state)
        {
            return Blueprint.GeodeRobotOreCost <= state.Ore && Blueprint.GeodeRobotObsidianCost <= state.Obsidian;
        }

        public override void Make(State state)
        {
            state.GeodeRobot++;
            state.Ore -= Blueprint.GeodeRobotOreCost;
            state.Obsidian -= Blueprint.GeodeRobotObsidianCost;
        }
    }
    private class State
    {
        public long Ore
        {
            get => GetValue(_state, 0);
            set => SetState(value, 0);
        }
        public long Clay
        {
            get => GetValue(_state, 7);
            set => SetState(value, 7);
        }
        public long Obsidian
        {
            get => GetValue(_state, 14);
            set => SetState(value, 14);
        }
        public long Geode
        {
            get => GetValue(_state, 21);
            set => SetState(value, 21);
        }

        public long OreRobot
        {
            get => GetValue(_state, 28);
            set => SetState(value, 28);
        }
        public long ClayRobot
        {
            get => GetValue(_state, 35);
            set => SetState(value, 35);
        }
        public long ObsidianRobot
        {
            get => GetValue(_state, 42);
            set => SetState(value, 42);
        }

        public long GeodeRobot
        {
            get => GetValue(_state, 49);
            set => SetState(value, 49);
        }

        public long Minute
        {
            get => GetValue(_state, 57, 6);
            set => SetState(value, 57, 6);
        }

        private long _state;

        public long StateCode => _state;

        public static implicit operator long(State state) => state.StateCode;

        public State()
        {

        }

        private const long Mask = 0b_1111111;

        public State(long fromCode)
        {
            _state = fromCode;
;
           // Console.WriteLine($"Ore Bits:    {Convert.ToString(mask & fromCode, toBase: 2).PadLeft(64, '0')}");
            //Console.WriteLine($"Clay Bits:   {Convert.ToString((Mask << 7) & (fromCode << 7), toBase: 2).PadLeft(64, '0')}");

            //Ore = GetValue(fromCode, 0);
            //Clay = GetValue(fromCode, 7);
            //Obsidian = GetValue(fromCode, 14);
            //Geode = GetValue(fromCode, 21);
            //OreRobot = GetValue(fromCode, 28);
            //ClayRobot = GetValue(fromCode, 35);
            //ObsidianRobot = GetValue(fromCode, 42);
            //GeodeRobot = GetValue(fromCode, 49);
            //Minute = GetValue(fromCode, 57, 6);
        }

        private static long GetValue(long fromCode, int offset, int width = 7)
        {
            /*
            PrintHighlightedBits("FromCode", fromCode, 64-offset-width, width);
            PrintHighlightedBits("Mask", (Mask << offset), 64-offset - width, width);
            PrintHighlightedBits("Bits", ((Mask << offset) & fromCode),64-offset - width, width);
            PrintHighlightedBits("Shifted", ((Mask << offset) & fromCode) >> offset, 64 - width, width);
            Console.WriteLine();
            */

            return ((Mask << offset) & fromCode) >> offset;
        }

        private void SetState(long value, int offset, int width=7)
        {
            
            //PrintHighlightedBits("FromCode", _state, 0, 0);

            long mask = ((long.MaxValue) ^ (127L << offset));
            

            //Console.WriteLine($"Mask :         {Convert.ToString(mask, toBase: 2).PadLeft(64, '0')}");
            //Console.WriteLine($"MaskedState:   {Convert.ToString((_state & mask) | (value << offset), toBase: 2).PadLeft(64, '0')}");
            

            _state = (_state & mask) | (value << offset);
            //PrintHighlightedBits("State", _state, 64 - offset - width, width);
            Console.WriteLine();
        }

        private static void PrintHighlightedBits(string text, long value, int start, int length)
        {
            AnsiConsole.Markup(text.PadRight(15, ' '));
            string toPrint = Convert.ToString(value, toBase: 2).PadLeft(64, '0');

            if (start == 0)
            {
                AnsiConsole.Markup($"[green]{toPrint.Substring(start, length)}[/]{toPrint.Substring(start)}");
            }
            else
            {
                AnsiConsole.Markup($"{toPrint[..start]}[green]{toPrint.Substring(start, length)}[/]{toPrint[(start+length)..]}");
            }
            AnsiConsole.WriteLine();
        }
    }

    private readonly Regex _regex =
        new Regex(
            @"Blueprint (\d+): Each ore robot costs (\d+) ore\. Each clay robot costs (\d+) ore\. Each obsidian robot costs (\d+) ore and (\d+) clay\. Each geode robot costs (\d+) ore and (\d+) obsidian\.",
            RegexOptions.Compiled);

    private enum Action
    {
        None, Ore, Clay, Obsidian, Geode
    }

    private Dictionary<int, Blueprint> _blueprints;

    private HashSet<long> _actionsHash;

    private async Task Init()
    {
        _blueprints = new Dictionary<int, Blueprint>();

        //var lines = await GetLines();
        var lines = new string[]
        {
            "Blueprint 1: Each ore robot costs 4 ore. Each clay robot costs 2 ore. Each obsidian robot costs 3 ore and 14 clay. Each geode robot costs 2 ore and 7 obsidian.",
            "Blueprint 2: Each ore robot costs 2 ore. Each clay robot costs 3 ore. Each obsidian robot costs 3 ore and 8 clay. Each geode robot costs 3 ore and 12 obsidian."

        };

        for (int i = 0; i < lines.Length; i++)
        {
            var match = _regex.Match(lines[i]);
            var blueprint = new Blueprint
            {
                OreRobotCost = int.Parse(match.Groups[2].Value),
                ClayRobotCost = int.Parse(match.Groups[3].Value),
                ObsidianRobotOreCost = int.Parse(match.Groups[4].Value),
                ObsidianRobotClayCost = int.Parse(match.Groups[5].Value),
                GeodeRobotOreCost = int.Parse(match.Groups[6].Value),
                GeodeRobotObsidianCost = int.Parse(match.Groups[7].Value)
            };
            _blueprints.Add(int.Parse(match.Groups[1].Value), blueprint);
        }
    }

    public override async Task RunPart1()
    {
        PrintStart(1);
        await Init();

        long total = 0;
        object locker = new { };

        foreach(var blueprint in _blueprints)
        {
            AnsiConsole.MarkupLine($"Blueprint [yellow]{blueprint.Key}[/]");

            long best = Simulate(blueprint.Key);
            lock (locker)
            {
                total += best * blueprint.Key;
            }

            AnsiConsole.MarkupLine($"Blueprint [yellow]{blueprint.Key}[/] is [green]{best}[/].");
            Console.WriteLine();
        }

        AnsiConsole.MarkupLine($"Total score is [green]{total}[/]");
    }

    private long Simulate(int blueprintId)
    {
        var blueprint = _blueprints[blueprintId];

        long best = 0;

        Dictionary<Action, Robot> robots = new Dictionary<Action, Robot>
        {
            { Action.Ore, new OreRobot(blueprint) },
            { Action.Clay, new ClayRobot(blueprint) },
            { Action.Obsidian, new ObsidianRobot(blueprint) },
            { Action.Geode, new GeodeRobot(blueprint) }
        };


        foreach(var actionPermutation in ActionPermutations)
        {
            //AnsiConsole.Clear();

            var state = new State
            {
                OreRobot = 1
            };

            Action? nextAction = null;
            int actionIndex = 0;
            List<(long day, Action action)> actionsTaken = new List<(long day, Action action)>();
            for (state.Minute = 1; state.Minute <= 24; state.Minute++)
            {

                // Phase 1 - Action phase
                var actionTaken = Action.None;
                if (nextAction == null)
                {
                    if (actionIndex < actionPermutation.Length)
                    {
                        nextAction = actionPermutation[actionIndex++];
                    }
                    else
                    {
                        nextAction = Action.None;
                    }
                }

                //nextAction = GetAction(state, robots, blueprint, ref shouldBreak);


                if (nextAction != Action.None)
                {
                    if (robots[nextAction.Value].CanMake(state))
                    {
                        robots[nextAction.Value].Make(state);
                        actionTaken = nextAction.Value;
                        nextAction = null;
                    }
                }
              

                // Phase 2 - Accumulation phase - if we added a bot this round, it doesn't take effect till next round. so subtract one from the one we just added.
                state.Ore += state.OreRobot - (int)(actionTaken == Action.Ore ? 1 : 0);
                state.Clay += state.ClayRobot - (int)(actionTaken == Action.Clay ? 1 : 0);
                state.Obsidian += state.ObsidianRobot - (int)(actionTaken == Action.Obsidian ? 1 : 0);
                state.Geode += state.GeodeRobot - (int)(actionTaken == Action.Geode ? 1 : 0);

                if (actionTaken != Action.None)
                {
                    actionsTaken.Add((state.Minute, actionTaken));
                }
                //Console.WriteLine($"Before: {Convert.ToString(state.StateCode, toBase: 2).PadLeft(64, '0')}");

            }
            PrintState(state, blueprint, actionsTaken);
            PrintState(new State(state.StateCode), blueprint, actionsTaken);
            if (state.Geode > best)
            {
                best = state.Geode;
            }
        }

        return best;
    }

    private IEnumerable<Action[]> ActionPermutations { get; } = new List<Action[]>()
    {
        new [] { Action.Clay, Action.Clay, Action.Obsidian, Action.Obsidian, Action.Geode }
    };

    /*private long DepthFirstSearch(State initialState, Blueprint blueprint, int minutes)
    {
        var visited = new HashSet<long>();
        long best = 0;

        var stack = new Stack<State>();
        stack.Push(initialState);

        while (stack.Count > 0)
        {
            var vertex = stack.Pop();

            if (visited.Contains(vertex))
                continue;

            long[] neighbors = new long[4];
            

            visited.Add(vertex);

            foreach (var neighbor in vertex.GetVertices())
                if (!visited.Contains(neighbor))
                    stack.Push(neighbor);
        }

        return best;
    }*/


    private void PrintState(State state, Blueprint blueprint, List<(long day, Action action)> actionsTaken)
    {
        AnsiConsole.MarkupLine($"Minute [green]{state.Minute}[/]");
        var table = new Table();
        table.Border(TableBorder.Rounded);

        table.AddColumns("Robots", "Inventory", "Actions Taken", "Costs");
        var robotTable = new Table() { Border = TableBorder.DoubleEdge, BorderStyle = new Style(Color.Orange1)};
        robotTable.AddColumn("Robot");
        robotTable.AddColumn("Count");
        robotTable.AddRow("[blue]Ore[/]", $"[white]{state.OreRobot}[/]");
        robotTable.AddRow("[blue]Clay[/]", $"[white]{state.ClayRobot}[/]");
        robotTable.AddRow("[blue]Obsidian[/]", $"[white]{state.ObsidianRobot}[/]");
        robotTable.AddRow("[blue]Geode[/]", $"[white]{state.GeodeRobot}[/]");

        var inventoryTable = new Table() { Border = TableBorder.DoubleEdge, BorderStyle = new Style(Color.Red3) };
        inventoryTable.AddColumn("Robot");
        inventoryTable.AddColumn("Count");
        inventoryTable.AddRow("[blue]Ore[/]", $"[white]{state.Ore}[/]");
        inventoryTable.AddRow("[blue]Clay[/]", $"[white]{state.Clay}[/]");
        inventoryTable.AddRow("[blue]Obsidian[/]", $"[white]{state.Obsidian}[/]");
        inventoryTable.AddRow("[blue]Geode[/]", $"[white]{state.Geode}[/]");

        var actionsTable = new Table() { Border = TableBorder.DoubleEdge, BorderStyle = new Style(Color.Purple) };
        actionsTable.AddColumn("Turn");
        actionsTable.AddColumn("Action");
        foreach (var action in actionsTaken)
        {
            actionsTable.AddRow($"[green]{action.day}[/]", $"Build a [green]{action.action}[/] robot.");
        }

        var costsTable = new Table() { Border = TableBorder.DoubleEdge, BorderStyle = new Style(Color.SandyBrown) };
        costsTable.AddColumns("Robot", "Cost");
        costsTable.AddRow("[blue]Ore[/]", $"[white]{blueprint.OreRobotCost}[/] Ore");
        costsTable.AddRow("[blue]Clay[/]", $"[white]{blueprint.ClayRobotCost}[/] Ore");
        costsTable.AddRow("[blue]Obsidian[/]", $"[white]{blueprint.ObsidianRobotOreCost}[/] Ore & [white]{blueprint.ObsidianRobotClayCost}[/] Clay");
        costsTable.AddRow("[blue]Geode[/]", $"[white]{blueprint.GeodeRobotOreCost}[/] Ore & [white]{blueprint.GeodeRobotObsidianCost}[/] Obsidian");


        table.AddRow(robotTable, inventoryTable, actionsTable, costsTable);

        AnsiConsole.Write(table);
    }

    public override async Task RunPart2()
    {
        PrintStart(2);
        await Init();
    }
}