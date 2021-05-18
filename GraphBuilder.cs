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

            foreach(var item in importList)
            {
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