using System.Diagnostics;
using System.Text.RegularExpressions;

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

        var lines = await GetLines();

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

        _actionsHash = new HashSet<long>();

        var random = new Random(1337);

        for (int count = 0; count < 8000000; count++)
        {
            long sequence = 0;

            int oreBots = 1;
            int clayBots = 0;
            int obsidianBots = 0;
            int geodeBots = 0;
            for (int i = 0; i < 21; i++)
            {
                var max = Action.Clay;
                if (clayBots > 0)
                {
                    max = Action.Obsidian;
                }

                if (obsidianBots > 0)
                {
                    max = Action.Geode;
                }

                // each action simply shifts the last action to the left (bit shift). each action is at most 3 bits (since actions are 1,2,3,4).
                // with a long, that gives a max of 21 actions, but we're not likely to need more than 16
                int action;

                while (true)
                {
                    action = random.Next(1, (int)max + 1);

                    bool canBreak = false;
                    // lets assume that after round 16, the value of ore generators is lower.
                    if (i > 16 && action == (int)Action.Ore)
                    {
                        continue;
                    }
                    // after round 19, probably won't need clay either. 
                    if (i > 19 && action == (int)Action.Clay)
                    {
                        continue;
                    }
                    switch ((Action)action)
                    {
                        case Action.Clay:
                            canBreak = clayBots < 7;
                            if (canBreak)
                                clayBots++;
                            break;
                        case Action.Obsidian:
                            canBreak = obsidianBots < 7;
                            if (canBreak)
                                obsidianBots++;
                            break;
                        case Action.Ore:
                            canBreak = oreBots < 7;
                            if (canBreak)
                                oreBots++;
                            break;
                        case Action.Geode:
                            canBreak = geodeBots < 7;
                            if (canBreak)
                                geodeBots++;
                            break;
                    }

                    if (canBreak)
                    {
                        sequence <<= 3;
                        sequence += action;
                        break;
                    }
                }

            }

            if (!_actionsHash.Contains(sequence))
            {
                _actionsHash.Add(sequence);
            }
        }

        Console.WriteLine($"Total Possible permutations is {_actionsHash.Count}");

        int total = 0;
        object locker = new { };

        Parallel.ForEach(_blueprints, blueprint =>
        {
            int best = Simulate(blueprint.Key);
            lock (locker)
            {
                total += best * blueprint.Key;
            }

            AnsiConsole.MarkupLine($"Blueprint [yellow]{blueprint.Key}[/] is [green]{best}[/].");
        });

        AnsiConsole.MarkupLine($"Total score is [green]{total}[/]");
    }

    private int Simulate(int blueprintId)
    {
        var blueprint = _blueprints[blueprintId];

        var best = 0;

        foreach (var action in _actionsHash)
        {
            var ore = 0;
            var clay = 0;
            var obsidian = 0;
            var geodes = 0;

            var oreRobot = 1;
            var clayRobot = 0;
            var obsidianRobot = 0;
            var geodeRobot = 0;

            // figure out the list of actions.  Since we pushed these on using bitshifting left, they are in reverse order, so once we get them, we'll reverse it.
            var actions = new List<Action>();
            const byte mask = 0b_111;
            var temp = action;
            while (true)
            {

                //Console.WriteLine($"Before: {Convert.ToString(temp, toBase: 2).PadLeft(64, '0')}");

                var toAdd = mask & temp;
                actions.Add((Action)toAdd);
                temp = temp >> 3;
                //Console.WriteLine($"After:  {Convert.ToString(temp, toBase: 2).PadLeft(64, '0')}");
                if (temp == 0)
                {
                    break;
                }
            }

            actions.Reverse();
            var index = 0;

            Action? nextAction = null;
            for (var i = 1; i <= 24; i++)
            {

                // Phase 1 - Action phase
                var actionTaken = Action.None;
                if (actions.Count > index)
                {
                    nextAction ??= actions[index++];
                }
                else
                {
                    nextAction = Action.None;
                }


                switch (nextAction)
                {
                    case Action.Ore when ore >= blueprint.OreRobotCost:
                        ore -= blueprint.OreRobotCost;
                        actionTaken = Action.Ore;
                        nextAction = null;
                        oreRobot++;
                        break;
                    case Action.Clay when ore >= blueprint.ClayRobotCost:
                        ore -= blueprint.ClayRobotCost;
                        actionTaken = Action.Clay;
                        nextAction = null;
                        clayRobot++;
                        break;
                    case Action.Obsidian when ore >= blueprint.ObsidianRobotOreCost && clay >= blueprint.ObsidianRobotClayCost:
                        ore -= blueprint.ObsidianRobotOreCost;
                        clay -= blueprint.ObsidianRobotClayCost;
                        actionTaken = Action.Obsidian;
                        nextAction = null;
                        obsidianRobot++;
                        break;
                    case Action.Geode when ore >= blueprint.GeodeRobotOreCost && obsidian >= blueprint.GeodeRobotObsidianCost:
                        ore -= blueprint.GeodeRobotOreCost;
                        obsidian -= blueprint.GeodeRobotObsidianCost;
                        actionTaken = Action.Geode;
                        nextAction = null;
                        geodeRobot++;
                        break;
                }

                // Phase 2 - Accumulation phase - if we added a bot this round, it doesn't take effect till next round. so subtract one from the one we just added.
                ore += oreRobot - (actionTaken == Action.Ore ? 1 : 0);
                clay += clayRobot - (actionTaken == Action.Clay ? 1 : 0);
                obsidian += obsidianRobot - (actionTaken == Action.Obsidian ? 1 : 0);
                geodes += geodeRobot - (actionTaken == Action.Geode ? 1 : 0);
            }

            if (geodes > best)
            {
                best = geodes;
            }
        }

        return best;
    }

    public override async Task RunPart2()
    {
        PrintStart(2);
        await Init();
    }
}