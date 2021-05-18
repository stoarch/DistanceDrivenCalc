using QuikGraph;
using System;
using System.Collections.Generic;

namespace DistanceDrivenCalc
{
    internal class DayDistancesDict : Dictionary<DateTime, double> { };
    internal class ShortestPathList : List<Edge<TravelItem>> { };
    internal class DayShortestPathDict : Dictionary<DateTime, ShortestPathList> { };
    internal class DayTravelDict : Dictionary<DateTime, List<TravelItem>> { };

    internal class DayEdgePair 
    {
        public DateTime date { get; internal set; }
        public Edge<TravelItem> edge { get; internal set; }
    }

    internal class DayEdgeDistanceDict : Dictionary<(DateTime date, Edge<TravelItem> edge), double> { };

    internal class DriveCalculator
    {
        public DriveCalculator()
        {
        }

        internal class CalculatorResult
        {
            public DayDistancesDict DayDistances { get; set; }
            public DayShortestPathDict DayShortestPath { get; set; }
            public DayTravelDict DayTravel { get; set; }
            public DayEdgeDistanceDict DayEdgeDistance { get; set; }
        }

        internal CalculatorResult Execute(AdjacencyGraph<TravelItem, Edge<TravelItem>> graph,
                                          Dictionary<Edge<TravelItem>, double> distance,
                                          List<TravelItem> travelItems,
                                          Func<Edge<TravelItem>,double> distanceReceiver)
        {
            if (graph is null)
            {
                throw new ArgumentNullException(nameof(graph));
            }

            if (distance is null)
            {
                throw new ArgumentNullException(nameof(distance));
            }

            if (travelItems is null)
            {
                throw new ArgumentNullException(nameof(travelItems));
            }

            var dayCalc = new DayDistanceCalculator();
            var pathCalc = new ShortestPathCalculator();
            var dayTravelCalc = new DayTravelCalculator();

            DayTravelDict dayTravel = dayTravelCalc.Execute(travelItems);
            (DayDistancesDict dayDistances, DayEdgeDistanceDict edgeDist) = dayCalc.Execute(graph, distance, dayTravel, distanceReceiver);
            DayShortestPathDict shortestPaths = pathCalc.Execute(graph, distance, dayTravel);

            return new CalculatorResult(){ 
                DayDistances = dayDistances, 
                DayShortestPath = shortestPaths, 
                DayTravel = dayTravel, 
                DayEdgeDistance = edgeDist
            };
        }
    }
}