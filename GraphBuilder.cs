using QuikGraph;
using System;
using System.Collections.Generic;

namespace DistanceDrivenCalc
{
    internal class GraphBuilder
    {
        public GraphBuilder()
        {
        }

        internal AdjacencyGraph<TravelItem, Edge<TravelItem>> MakeFrom(List<TravelItem> importList)
        {
            TravelItem nextItem = null;

            AdjacencyGraph<TravelItem, Edge<TravelItem>> graph = new AdjacencyGraph<TravelItem, Edge<TravelItem>>();

            const int MAX_LIMIT = 30; //TODO: Set limit thru args (how much rows to process)

            int limit = 0;

            foreach(var item in importList)
            {
                limit += 1;
                if (limit > MAX_LIMIT)
                {
                    break;
                }

                if ((item.StreetAddress == "")||(item.City == ""))
                {
                    continue;
                }

                graph.AddVertex(item);

                if (nextItem != null)
                {
                    Edge<TravelItem> edge = new Edge<TravelItem>(item, nextItem);

                    graph.AddEdge(edge);
                }

                nextItem = item;
            }

            return graph;
        }
    }
}