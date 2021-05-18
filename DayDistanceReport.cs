using System;
using System.Collections.Generic;

namespace DistanceDrivenCalc
{
    internal class DayDistanceReport : DayReport
    {
        readonly List<(DateTime date, TravelItem src, TravelItem dest, double dist)> reportData = new List<(DateTime date, TravelItem src, TravelItem dest, double dist)>();

        public DayDistanceReport()
        {
        }

        internal void Generate(DayEdgeDistanceDict dayEdgeDistance)
        {
            reportData.Clear();

            foreach (var (key, value) in dayEdgeDistance)
            {
                reportData.Add((key.date, src: key.edge.Source, dest: key.edge.Target, dist: value));
            }
        }

        const int COLUMN_COUNT = 4;

        public override int RowCount => reportData.Count;

        internal int GetColCount(int row) => COLUMN_COUNT;

        const int DATE_COLUMN = 0;
        const int SOURCE_COLUMN = 1;
        const int DESTINATION_COLUMN = 2;
        const int DISTANCE_COLUMN = 3;

        internal override string ColumnValue(int row, int col)
        {
            return col switch
            {
                DATE_COLUMN => reportData[row].date.ToShortDateString(),
                SOURCE_COLUMN => reportData[row].src.ToString(),
                DESTINATION_COLUMN => reportData[row].dest.ToString(),
                DISTANCE_COLUMN => reportData[row].dist.ToString(),
                _ => "",
            };
        }
    }
}