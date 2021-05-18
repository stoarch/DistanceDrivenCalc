using QuikGraph;
using QuikGraph.Algorithms;
using System;
using System.Collections.Generic;

namespace DistanceDrivenCalc
{
    internal class ShortestPathCalculator
    {
        public ShortestPathCalculator()
        {
        }

        internal DayShortestPathDict Execute(AdjacencyGraph<TravelItem, Edge<TravelItem>> graph, Dictionary<Edge<TravelItem>, double> distance, DayTravelDict dayTravel)
        {
            var result = new DayShortestPathDict();

            foreach (var (day, travel) in dayTravel)
            {
                double edgeCost(Edge<TravelItem> edge) => distance[edge];

                TravelItem root = travel[0];

                var tryGetPaths = graph.ShortestPathsDijkstra(edgeCost, root);

                TravelItem target = travel[^1];
                if (tryGetPaths(target, out IEnumerable<Edge<TravelItem>> path))
                {
                    var optimalPath = new ShortestPathList();

                    foreach (Edge<TravelItem> edge in path)
                    {
                        optimalPath.Add(edge);
                    }

                    result.Add(day, optimalPath);
                }
                else
                {
                    Console.WriteLine($"Unable to find shortest path from {root} to {target}");
                }
            }

            return result;
        }
    }
}