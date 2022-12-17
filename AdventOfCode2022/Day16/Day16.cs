using System.Text.RegularExpressions;

namespace AdventOfCode2022.Day16;
internal class Day16 : DayBase
{
    public class Valve
    {
        public string Id;
        public int FlowRate = 0;
        public bool IsOpen;
        public List<string> Edges = new List<string>();
    }

    private Dictionary<string, Valve> ValveList;
  
    private async Task Init()
    {
        ValveList = new Dictionary<string, Valve>();

        var lines = await GetLines();
        Regex parse = new Regex(@"Valve (\w\w) has flow rate=(\d+); tunnels? leads? to valves? (.*)");

        foreach (var line in lines)
        {
            var match = parse.Match(line);
            if (match.Success)
            {
                var valve = new Valve()
                {
                    Id = match.Groups[1].Value,
                    FlowRate = int.Parse(match.Groups[2].Value),
                    Edges = match.Groups[3].Value.Split(", ").Select(x => x).ToList()
                };

                ValveList[match.Groups[1].Value] = valve;
            }
        }

        

    }

    public override async Task RunPart1()
    {
        PrintStart(1);
        await Init();

        var graph = new Cave(ValveList);
        var result = Algorithms<Valve>.Dijkstra(graph, ValveList["AA"]);

        while (graph.TimeRemaining > 0)
        {
            graph.TimeRemaining--;
        }
    }

    public override async Task RunPart2()
    {
        PrintStart(2);
        await Init();
    }


    public class Cave : IGraph<Valve>
    {
        private readonly Dictionary<string, Valve> _valves;

        public int TimeRemaining = 30;

        public Cave(Dictionary<string, Valve> valves)
        {
            _valves = valves;
        }

        public List<Valve> GetVertices()
        {
            return _valves.Values.ToList();
        }
        public List<Valve> GetNeighbors(Valve vertex)
        {
            return vertex.Edges.Select(x => _valves[x]).ToList();
        }
        public double GetWeight(Valve u, Valve v)
        {
            if (v.IsOpen) return 0;

            return ((v.FlowRate * TimeRemaining) /10);
        }
    }



    // Dijkstra's from https://gist.github.com/wjmolina/ceedf22b9d41c6f9dc71

    public interface IGraph<VertexType>
    {
        List<VertexType> GetVertices();
        List<VertexType> GetNeighbors(VertexType vertex);
        double GetWeight(VertexType u, VertexType v);
    }

    static class Algorithms<VertexType>
    {
        public static Dictionary<VertexType, double> Dijkstra(IGraph<VertexType> Graph, VertexType source)
        {
            var distance = new Dictionary<VertexType, double>();
            var remaining = new HashSet<VertexType>();

            distance[source] = 0d;

            foreach (var vertex in Graph.GetVertices())
            {
                if (!vertex.Equals(source))
                {
                    distance[vertex] = double.PositiveInfinity;
                }

                remaining.Add(vertex);
            }

            while (remaining.Count > 0)
            {
                var subject = default(VertexType);
                var flag = true;

                foreach (var vertex in remaining)
                {
                    if (flag || distance[vertex] < distance[subject])
                    {
                        subject = vertex;
                        flag = false;
                    }
                }

                remaining.Remove(subject);

                foreach (var vertex in Graph.GetNeighbors(subject))
                {
                    var weight = Graph.GetWeight(subject, vertex);
                    
                    var measure = weight == 0 ? 1 : distance[subject] + weight;

                    if (measure < distance[vertex])
                    {
                        distance[vertex] = measure;
                    }
                }
            }

            return distance;
        }
    }
}