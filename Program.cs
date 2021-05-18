using System;

namespace DistanceDrivenCalc
{
    class Program
    {
        static void Main(string[] args)
        {
            const string VERSION = "0.10.8";

            Console.WriteLine($"Drive calculator v.{VERSION}");
            CheckCommandLineArgs(args);

            CalculatorManager manager = new CalculatorManager();
            string inFile = args[0];
            string outFile = args[1];
            string googleKey = args[2];

            manager.MakeShortestPathReport(inFile, outFile, googleKey);
        }

        private static void CheckCommandLineArgs(string[] args)
        {
            if(args.Length < 2)
            {
                ShowUsage();
                Environment.Exit(-1); 
            }
        }

        private static void ShowUsage()
        {
            Console.WriteLine("Usage: DistanceDrivenCalc.exe input_file.xls output_file.xls google-map-matrix-key");
        }
    }
}
