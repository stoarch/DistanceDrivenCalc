using QuikGraph;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DistanceDrivenCalc
{
    internal class CalculatorManager
    {
        const int MAX_LIMIT = 10; //TODO: Set limit thru args (how much rows to process)

        public CalculatorManager()
        {
        }

        internal void MakeShortestPathReport(string inFileName, string outFileName, string googleKey)
        {
            Console.WriteLine("Reading excel file");

            var importer = new ExcelImporter();
            List<TravelItem> importList = importer.Execute(inFileName).Take(MAX_LIMIT).ToList(); 

            Console.WriteLine("Building graph from import list");

            var builder = new GraphBuilder();
            AdjacencyGraph<TravelItem, Edge<TravelItem>> graph = builder.MakeFrom(importList);

            Console.WriteLine("Retrieve distance from google");

            var distanceRetriever = new GoogleMapDistanceRetriever(googleKey);
            Dictionary<Edge<TravelItem>, double> distance = distanceRetriever.Execute(graph);

            Console.WriteLine("Calculate drive");

            var driveCalc = new DriveCalculator();
            var calcResult = driveCalc.Execute( graph, distance, importList, 
                                (e) => distanceRetriever.GetEdgeDistanceDouble(e));

            Console.WriteLine("Generating reports");

            var reportGen = new ExportGenerator();
            DriveReport report = reportGen.Execute(calcResult.DayDistances, calcResult.DayShortestPath, calcResult.DayTravel, calcResult.DayEdgeDistance);

            Console.WriteLine("Exporting reports");

            var exporter = new FastExcelExporter();
            exporter.Export(report.ExportTable, outFileName, templateFileName: inFileName);
        }
    }
}