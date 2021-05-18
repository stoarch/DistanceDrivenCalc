using System;
using System.Collections.Generic;
using System.Text;

namespace DistanceDrivenCalc
{
    internal class DayShortestPathReport : DayReport
    {
        List<(DateTime date, ShortestPathList shortestPath)> reportData = new List<(DateTime date, ShortestPathList shortestPath)>(); 

        public DayShortestPathReport()
        {
        }

        public override int RowCount => reportData.Count;

        internal void Generate(DayShortestPathDict shortestPaths)
        {
            reportData.Clear();

            foreach(var (key, value) in shortestPaths)
            {
                reportData.Add((key, value));
            }
        }

        const int COLUMN_COUNT = 2;

        internal int GetColCount(int row)
        {
            return COLUMN_COUNT;
        }

        const int DATE_COLUMN = 0;
        const int SHORTEST_PATH_COLUMN = 1;

        internal override string ColumnValue(int row, int col)
        {
            return col switch
            {
                DATE_COLUMN => reportData[row].date.ToShortDateString(),
                SHORTEST_PATH_COLUMN => GetShortestPathString(row),
                _ => ""
            };
        }

        private string GetShortestPathString(int row)
        {
            StringBuilder result = new StringBuilder();
            var path = reportData[row].shortestPath;

            for(int i = 0; i < path.Count; i++ )
            {
                result.Append(path[i].Source.ToString());

                if( i < path.Count )
                {
                    result.Append("-");
                }

                if( i == path.Count - 1 )
                {
                    result.Append(path[i].Target.ToString());
                }
            }

            return result.ToString();
        }
    }
}