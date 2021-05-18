using System;
using System.Collections.Generic;
using System.Linq;

namespace DistanceDrivenCalc
{
    internal class ExcelImporter
    {
        public ExcelImporter()
        {
        }

        internal List<TravelItem> Execute(string inFileName)
        {
            ExcelReader excelReader = new ExcelReader
            {
                NameFile = inFileName,
                Path = AppDomain.CurrentDomain.BaseDirectory
            };

            var result = new List<TravelItem>();

            if (excelReader.LoadXLS())
            {
                Data[] trtaB = excelReader.ReturnExcelSheetData();
                result = trtaB.Select(data => new TravelItem(data)).ToList();
            }

            return result;
        }
    }
}