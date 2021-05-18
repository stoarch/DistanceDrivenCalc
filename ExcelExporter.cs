using System;
using Microsoft.Office.Interop;

using Excel = Microsoft.Office.Interop.Excel;

namespace DistanceDrivenCalc
{
    internal class ExcelExporter
    {
        public ExcelExporter()
        {
        }

        internal void Export(string[][] exportTable, string outFileName)
        {
            Excel.Application excelApp = new Excel.Application();
            if (excelApp != null)
            {
                Excel.Workbook excelWorkbook = excelApp.Workbooks.Add();
                Excel.Worksheet excelWorksheet = (Excel.Worksheet)excelWorkbook.Sheets.Add();

                for (int row = 0; row < exportTable.Length; row++)
                {
                    for (int col = 0; col < exportTable[row].Length; col++)
                    {
                        excelWorksheet.Cells[row, col] = exportTable[row][col];
                    }
                }

                excelApp.ActiveWorkbook.SaveAs(outFileName, Excel.XlFileFormat.xlWorkbookNormal);

                excelWorkbook.Close();
                excelApp.Quit();

                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(excelWorksheet);
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(excelWorkbook);
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(excelApp);
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }
    }
}