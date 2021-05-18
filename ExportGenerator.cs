using System;

namespace DistanceDrivenCalc
{
    internal class ExportGenerator
    {
        public ExportGenerator()
        {
        }

        internal DriveReport Execute(DayDistancesDict dayDistance, DayShortestPathDict dayShortestPath, DayTravelDict dayTravel, DayEdgeDistanceDict dayEdgeDistance)
        {
            var dayDistanceReport = new DayDistanceReport();
            var dayShortestPathReport = new DayShortestPathReport();
            var driveReport = new DriveReport();

            dayDistanceReport.Generate(dayEdgeDistance);
            dayShortestPathReport.Generate(dayShortestPath);

            driveReport.Combine(dayDistanceReport, dayShortestPathReport);

            return driveReport;
        }
    }
}