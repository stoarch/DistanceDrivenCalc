using System;

namespace DistanceDrivenCalc
{
    internal class DriveReport
    {
        public string[][] ExportTable { get; internal set; }

        internal void Combine(DayDistanceReport dayDistanceReport, DayShortestPathReport dayShortestPathReport)
        {
            int rowCount = dayDistanceReport.RowCount;

            ExportTable = new string[rowCount][];

            for(int row = 0; row < rowCount; row++)
            {
                int distColCount = dayDistanceReport.GetColCount(row);
                int shortestColCount = dayShortestPathReport.GetColCount(row);

                int colCount = distColCount + shortestColCount;

                ExportTable[row] = new string[colCount];

                int overallCol = 0;
                for(int col = 0; col < distColCount; col++)
                {
                    ExportTable[row][overallCol] = dayDistanceReport.ColumnValue(row, col);
                    overallCol += 1;
                }

                for(int col = 0; col < shortestColCount; col++)
                {
                    ExportTable[row][overallCol] = dayShortestPathReport.ColumnValue(row, col);
                    overallCol += 1;
                }
            }
        }
    }
}