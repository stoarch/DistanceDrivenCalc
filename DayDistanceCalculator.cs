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
            DayTravelDict dayTravel )
        {
            var dayDistances = new DayDistancesDict();
            var dayEdgeDist = new DayEdgeDistanceDict();

            foreach(DateTime key in dayTravel.Keys)
            {
                var routes = dayTravel[key];
                var pairEdges = routes.Zip(routes.Skip(1), (a, b) => new Edge<TravelItem>(a, b));

                foreach(var edge in pairEdges)
                {
                    dayEdgeDist.Add(key, (edge, edgedDistances[edge]));
                }

                double resDist = pairEdges.Aggregate(0.0, (acc, item) => acc + edgedDistances[item]);
                dayDistances.Add(key, resDist);
            }

            return (dayDistances, dayEdgeDist);
        }
    }
}