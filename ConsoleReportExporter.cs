using System;

namespace DistanceDrivenCalc
{
    internal class ConsoleReportExporter
    {
        public ConsoleReportExporter()
        {
        }

        internal void Export(string[][] exportTable, string outFileName)
        {
            Console.WriteLine("Report generated:");
            for (int row = 0; row < exportTable.Length; row++)
            {
                Console.Write($"{row} ");
                for (int col = 0; col < exportTable[row].Length; col++)
                {
                    Console.Write($"\t {exportTable[row][col]}");
                }
                Console.WriteLine();
            }
            Console.WriteLine("Report end.");
        }
    }
}