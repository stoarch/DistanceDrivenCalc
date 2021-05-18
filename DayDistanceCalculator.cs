using QuikGraph;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DistanceDrivenCalc
{
    internal class DayDistanceCalculator
    {
        public DayDistanceCalculator()
        {
        }

        internal (DayDistancesDict, DayEdgeDistanceDict) Execute(
            AdjacencyGraph<TravelItem, Edge<TravelItem>> graph,
            Dictionary<Edge<TravelItem>, double> edgedDistances,
            DayTravelDict dayTravel,
            Func<Edge<TravelItem>, double> retriever)
        {
            var dayDistances = new DayDistancesDict();
            var dayEdgeDist = new DayEdgeDistanceDict();

            foreach(DateTime key in dayTravel.Keys)
            {
                var routes = dayTravel[key];
                var pairEdges = routes.Zip(routes.Skip(1), (a, b) => new Edge<TravelItem>(a, b));

                foreach(var edge in pairEdges)
                {
                    if (edgedDistances.ContainsKey(edge))
                    {
                        dayEdgeDist.Add((key, edge), edgedDistances[edge]);
                    }
                    else
                    {
                        Console.WriteLine($"Unable to find edge distance {edge}: querying...");

                        if(retriever != null)
                        {
                            double distance = retriever(edge);
                            edgedDistances.Add(edge, distance);
                            dayEdgeDist.Add((key, edge), distance);
                        }
                    }
                }

                double resDist = pairEdges.Aggregate(0.0, (acc, item) => acc + GetEdgeDistance(edgedDistances, item));
                dayDistances.Add(key, resDist);
            }

            return (dayDistances, dayEdgeDist);

            double GetEdgeDistance(Dictionary<Edge<TravelItem>, double> edgedDistances, Edge<TravelItem> edge)
            {
                double result;
                if (edgedDistances.TryGetValue(edge, out result))
                    return result;

                Console.WriteLine($"Unable to find edge distance {edge}: querying...");

                if (retriever != null)
                {
                    double distance = retriever(edge);
                    edgedDistances.Add(edge, distance);
                    return distance;
                }

                return -1.0f;
            }
        }

        
    }
}