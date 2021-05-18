using QuikGraph;
using System;
using System.Collections.Generic;

namespace DistanceDrivenCalc
{
    internal class CalculatorManager
    {
        public CalculatorManager()
        {
        }

        internal void MakeShortestPathReport(string inFileName, string outFileName, string googleKey)
        {
            var importer = new ExcelImporter();
            List<TravelItem> importList = importer.Execute(inFileName);

            var builder = new GraphBuilder();
            AdjacencyGraph<TravelItem, Edge<TravelItem>> graph = builder.MakeFrom(importList);

            var distanceRetriever = new GoogleMapDistanceRetriever(googleKey);
            Dictionary<Edge<TravelItem>, double> distance = distanceRetriever.Execute(graph);

            var driveCalc = new DriveCalculator();
            var calcResult = driveCalc.Execute(graph, distance, importList);

            var reportGen = new ExportGenerator();
            DriveReport report = reportGen.Execute(calcResult.DayDistances, calcResult.DayShortestPath, calcResult.DayTravel, calcResult.DayEdgeDistance);

            var exporter = new ConsoleReportExporter();
            exporter.Export(report.ExportTable, outFileName);
        }
    }
}