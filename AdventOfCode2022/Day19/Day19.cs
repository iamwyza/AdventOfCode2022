using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static AdventOfCode2022.Day16.Day16;

namespace AdventOfCode2022.Day19;
internal class Day19 : DayBase
{

    private static short _totalMinutes = 24;

    private class Blueprint
    {
        public int OreRobotCost;
        public int ClayRobotCost;
        public int ObsidianRobotOreCost;
        public int ObsidianRobotClayCost;
        public int GeodeRobotOreCost;
        public int GeodeRobotObsidianCost;

        private int _maxOreCost = 0;
        public int MaxOreCost
        {
            get
            {
                if (_maxOreCost > 0)
                {
                    return _maxOreCost;
                }

                _maxOreCost = new [] { OreRobotCost, ClayRobotCost, ObsidianRobotOreCost, GeodeRobotOreCost }.Max();
                return _maxOreCost;
            }
        }

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

        public abstract bool CanMakeFuture(State state, out short minutes);

        public abstract void Make(State state);

        public abstract bool MaxRobotsMade(State state);
    }

    private class OreRobot : Robot
    {
        public OreRobot(Blueprint blueprint) : base(blueprint)
        {
        }

        public override bool CanMake(State state)
        {

            if (MaxRobotsMade(state)) return false;

            return Blueprint.OreRobotCost <= state.Ore;
        }

        public override bool CanMakeFuture(State state, out short minutes)
        {
            minutes = 1;

            if (MaxRobotsMade(state)) return false;

            

            for (var i = state.Minute; i <= _totalMinutes; i++)
            {
                minutes++;
                if (Blueprint.OreRobotCost <= state.Ore + (state.OreRobot * minutes) + (state.LastAction == Action.Ore ? -1 : 0))
                {
                    return true;
                }
            }

            minutes = -1;

            return false;
        }

        public override void Make(State state)
        {
            state.OreRobot++;
            state.Ore -= Blueprint.OreRobotCost;
            state.LastAction = Action.Ore;
        }

        public override bool MaxRobotsMade(State state)
        {
            // No purpose in building more robots than we could consume in 1 minute
            return state.OreRobot >= Blueprint.MaxOreCost;
        }
    }

    private class ClayRobot : Robot
    {
        public ClayRobot(Blueprint blueprint) : base(blueprint)
        {
        }

        public override bool CanMake(State state)
        {
            if (MaxRobotsMade(state)) return false;

            return Blueprint.ClayRobotCost <= state.Ore;
        }

        public override bool CanMakeFuture(State state, out short minutes)
        {
            minutes = 1;

            if (MaxRobotsMade(state)) return false;

            for (var i = state.Minute; i <= _totalMinutes; i++)
            {
                minutes++;
                if (Blueprint.ClayRobotCost <= state.Ore + state.OreRobot * minutes + (state.LastAction == Action.Ore ? -1 : 0))
                {
                    return true;
                }
            }

            minutes = -1;

            return false;
        }

        public override void Make(State state)
        {
            state.ClayRobot++;
            state.Ore -= Blueprint.ClayRobotCost;
            state.LastAction = Action.Clay;
        }

        public override bool MaxRobotsMade(State state)
        {
            // No purpose in building more robots than we could consume in 1 minute
            return state.ClayRobot >= Blueprint.ObsidianRobotClayCost;
        }
    }

    private class ObsidianRobot : Robot
    {
        public ObsidianRobot(Blueprint blueprint) : base(blueprint)
        {
        }

        public override bool CanMake(State state)
        {
            if (MaxRobotsMade(state)) return false;

            return Blueprint.ObsidianRobotOreCost <= state.Ore && Blueprint.ObsidianRobotClayCost <= state.Clay;
        }

        public override bool CanMakeFuture(State state, out short minutes)
        {
            minutes = 1;

            if (state.ClayRobot == 0 | MaxRobotsMade(state)) return false;

            for (var i = state.Minute; i <= _totalMinutes; i++)
            {
                minutes++;
                if (Blueprint.ObsidianRobotOreCost <= state.Ore + state.OreRobot * minutes + (state.LastAction == Action.Ore ? -1 : 0)
                    && Blueprint.ObsidianRobotClayCost <= state.Clay + state.ClayRobot * minutes + (state.LastAction == Action.Clay ? -1 : 0))
                {
                    return true;
                }
            }

            minutes = -1;

            return false;
        }

        public override void Make(State state)
        {
            state.ObsidianRobot++;
            state.Ore -= Blueprint.ObsidianRobotOreCost;
            state.Clay -= Blueprint.ObsidianRobotClayCost;
            state.LastAction = Action.Obsidian;

        }

        public override bool MaxRobotsMade(State state)
        {
            // No purpose in building more robots than we could consume in 1 minute
            return state.ObsidianRobot >= Blueprint.GeodeRobotObsidianCost;
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

        public override bool CanMakeFuture(State state, out short minutes)
        {
            minutes = 1;

            if (state.ObsidianRobot == 0) return false;

            for (var i = state.Minute; i <= _totalMinutes; i++)
            {
                minutes++;
                if (Blueprint.GeodeRobotOreCost <= state.Ore + state.OreRobot * minutes + (state.LastAction == Action.Ore ? -1 : 0)
                    && Blueprint.GeodeRobotObsidianCost <= state.Obsidian + state.ObsidianRobot * minutes + (state.LastAction == Action.Obsidian ? -1 : 0))
                {
                    return true;
                }
            }

            minutes = -1;

            return false;
        }

        public override void Make(State state)
        {
            state.GeodeRobot++;
            state.Ore -= Blueprint.GeodeRobotOreCost;
            state.Obsidian -= Blueprint.GeodeRobotObsidianCost;
            state.LastAction = Action.Geode;

        }

        public override bool MaxRobotsMade(State state)
        {
            return false;
        }
    }
    
    // Does this actually save memory by using bit shifting to read/write a single value with property lookups? IDK, i think so, but it was more just fun to play with bitwise stuff since I rarely do it in my day-to-day.
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
            get
            {
                var value = GetValue(_state, 57, 6);
                if ( value == -1)
                {
                    Debugger.Break();
                }

                return value;
            }
            set => SetState(value, 57, 6);
        }

        private long _state;

        public long StateCode => _state;

        public static implicit operator long(State state) => state.StateCode;

        public State()
        {
            History = new();
            ActionsTaken = new List<(long day, Action action)>();
        }

        public readonly List<State> History;
        public List<(long day, Action action)> ActionsTaken;

        private const long Mask = 0b_1111111;

        public State(long fromCode)
        {
            _state = fromCode;
            History = new();
            ActionsTaken = new List<(long day, Action action)>();

        }

        public void FastForward(short minutes)
        {
            for (int i = 0; i < minutes; i++)
            {
                History.Add(new State(this) { ActionsTaken  = new(ActionsTaken), LastAction = LastAction});
                Ore += OreRobot;
                Clay += ClayRobot;
                Obsidian += ObsidianRobot;
                Geode += GeodeRobot;
                switch (LastAction)
                {
                    case Action.Ore:
                        Ore--;
                        break;
                    case Action.Clay:
                        Clay--;
                        break;
                    case Action.Obsidian:
                        Obsidian--;
                        break;
                    case Action.Geode:
                        Geode--;
                        break;
                }

                

                LastAction = Action.None;

                Minute += 1;
            }

            

            LastAction = Action.None;

        }

        public Action LastAction;

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

            // Masks off the state so that only the bits were changing are altered.  They are set to 0.  

            //PrintHighlightedBits("FromCode", _state, 0, 0);

            long mask = ((long.MaxValue) ^ (127L << offset));
            

            //Console.WriteLine($"Mask :         {Convert.ToString(mask, toBase: 2).PadLeft(64, '0')}");
            //Console.WriteLine($"MaskedState:   {Convert.ToString((_state & mask) | (value << offset), toBase: 2).PadLeft(64, '0')}");
            
            // Then set the state to the combination of the state before and the new value for those bits only. 

            _state = (_state & mask) | (value << offset);
            //PrintHighlightedBits("State", _state, 64 - offset - width, width);
            //Console.WriteLine();
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

            var state = new State
            {
                OreRobot = 1,
                Minute = 1
            };
            //state.History.Add(state);
            //long best = Simulate(blueprint.Key);

            long best = DepthFirstSearch(state, blueprint.Value, _totalMinutes);
            lock (locker)
            {
                total += best * blueprint.Key;
            }

            AnsiConsole.MarkupLine($"Blueprint [yellow]{blueprint.Key}[/] is [green]{best}[/].");
            Console.WriteLine();
        }

        AnsiConsole.MarkupLine($"Total score is [green]{total}[/]");
    }

    private long DepthFirstSearch(State initialState, Blueprint blueprint, int minutes)
    {
        var visited = new HashSet<long>();
        State bestState = initialState;
        long best = 0;


        Dictionary<Action, Robot> robots = new Dictionary<Action, Robot>
        {
            { Action.Ore, new OreRobot(blueprint) },
            { Action.Clay, new ClayRobot(blueprint) },
            { Action.Obsidian, new ObsidianRobot(blueprint) },
            { Action.Geode, new GeodeRobot(blueprint) }
        };

        var stack = new Stack<State>();
        stack.Push(initialState);

        while (stack.Count > 0)
        {
            var vertex = stack.Pop();

            if (visited.Contains(vertex))
                continue;
            //PrintState(vertex, blueprint, null);
            if (vertex.Geode > best)
            {
                best = vertex.Geode;
                bestState = vertex;
            }

            // theoretical best case scenario of this branch.  If it is already less than the best we could do in another branch, it's dead.
            var remainingTime = _totalMinutes + 1 - vertex.Minute;
            long possibleBest = vertex.Geode + remainingTime * vertex.GeodeRobot +
                                (remainingTime * (remainingTime - 1)) / 2;

            //if (possibleBest < best) 
            //    continue;

            List<State> neighbors = new List<State>();

            foreach (var (action, robot) in robots)
            {
                if (robot.CanMakeFuture(vertex, out short minutesUntilCanMake))
                {
                    var neighbor = new State(vertex) { LastAction = vertex.LastAction};
                    
                    neighbor.History.AddRange(vertex.History);
                    neighbor.ActionsTaken.AddRange(vertex.ActionsTaken);
                    //neighbor.History.Add(neighbor);

                    // If the next thing we can build happens after the last minute, then fast-forward until the last minute and get the state of the geodes, check for best and return;
                    if (neighbor.Minute + minutesUntilCanMake > _totalMinutes)
                    {
                        neighbor.FastForward((short)(_totalMinutes - neighbor.Minute));

                        
                        if (neighbor.Geode > best)
                        {
                            best = neighbor.Geode;
                            bestState = neighbor;
                        }

                        continue;
                    }

                    neighbor.FastForward(minutesUntilCanMake);
                    robot.Make(neighbor);
                    neighbor.ActionsTaken.Add((neighbor.Minute, action));
                    neighbors.Add(neighbor);
                }
            }


            visited.Add(vertex);

            foreach (var neighbor in neighbors)
                if (!visited.Contains(neighbor))
                    stack.Push(neighbor);
        }

        foreach (var state in bestState.History)
        {
            PrintState(state, blueprint, state.ActionsTaken);
        }

        PrintState(bestState, blueprint, bestState.ActionsTaken);


        return best;
    }


    private void PrintState(State state, Blueprint blueprint, List<(long day, Action action)>? actionsTaken)
    {
        AnsiConsole.MarkupLine($"Minute [green]{state.Minute}[/]. Last Action: [green]{state.LastAction}[/]");
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
        inventoryTable.AddColumn("Type");
        inventoryTable.AddColumn("Count");
        inventoryTable.AddRow("[blue]Ore[/]", $"[white]{state.Ore}[/] + [green]{(state.LastAction == Action.Ore ? state.OreRobot -1 : state.OreRobot)}[/]");
        inventoryTable.AddRow("[blue]Clay[/]", $"[white]{state.Clay}[/] + [green]{(state.LastAction == Action.Clay ? state.ClayRobot - 1 : state.ClayRobot)}[/]");
        inventoryTable.AddRow("[blue]Obsidian[/]", $"[white]{state.Obsidian}[/] + [green]{(state.LastAction == Action.Obsidian ? state.ObsidianRobot - 1 : state.ObsidianRobot)}[/]");
        inventoryTable.AddRow("[blue]Geode[/]", $"[white]{state.Geode}[/] + [green]{(state.LastAction == Action.Geode ? state.GeodeRobot - 1 : state.GeodeRobot)}[/]");

        var actionsTable = new Table() { Border = TableBorder.DoubleEdge, BorderStyle = new Style(Color.Purple) };
        actionsTable.AddColumn("Turn");
        actionsTable.AddColumn("Action");
        if (actionsTaken != null)
        {
            foreach (var action in actionsTaken)
            {
                actionsTable.AddRow($"[green]{action.day}[/]", $"Build a [green]{action.action}[/] robot.");
            }
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