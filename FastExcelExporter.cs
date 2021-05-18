using FastExcel;
using System;
using System.Collections.Generic;
using System.IO;

namespace DistanceDrivenCalc
{
    internal class FastExcelExporter
    {
        public FastExcelExporter()
        {
        }

        internal void Export(string[][] exportTable, string outFileName, string templateFileName = "")
        {
            var worksheet = new Worksheet();
            var rows = new List<Row>();

            for (int row = 0; row < exportTable.Length; row++)
            {
                List<Cell> cells = new List<Cell>();
                for (int col = 0; col < exportTable[row].Length; col++)
                {
                    cells.Add(new Cell(col+1, exportTable[row][col]));
                }

                rows.Add(new Row(row+1, cells));
            }

            worksheet.Rows = rows;

            var templateFile = new FileInfo(templateFileName);
            var outFile = new FileInfo(outFileName);

            if (File.Exists(outFile.FullName))
            {
                File.Delete(outFile.FullName);
            }

            string worksheetName = "";
            
            //Read worksheet
            using(FastExcel.FastExcel fastExcel = new FastExcel.FastExcel(templateFile))
            {
                worksheetName = fastExcel.Worksheets[0].Name;
            }

            // Create an instance of FastExcel
            using (FastExcel.FastExcel fastExcel = new FastExcel.FastExcel(templateFile, outFile))
            {
                // Write the data
                fastExcel.Write(worksheet, worksheetName);
            }
        }
    }
}