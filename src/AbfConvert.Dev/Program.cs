using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace AbfConvert.Dev
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine($"ERROR: invalid arguments");
                ShowHelp();
                return;
            }

            string pathIn = System.IO.Path.GetFullPath(args[0]);
            if (!System.IO.File.Exists(pathIn))
            {
                Console.WriteLine($"ERROR: file does not exist ({pathIn})");
                return;
            }

            string pathOut = System.IO.Path.GetFullPath(args[1]);
            string pathOutFolder = System.IO.Path.GetDirectoryName(pathOut);
            if (!System.IO.Directory.Exists(pathOutFolder))
            {
                Console.WriteLine($"ERROR: output folder does not exist ({pathOutFolder})");
                return;
            }

            Console.WriteLine($"Reading {System.IO.Path.GetFileName(pathOut)}...");

            using (var abf = new ABF(pathIn))
            {
                Console.WriteLine(abf);

                float[][] sweepValues = new float[abf.SweepCount][];
                for (int i = 0; i < abf.SweepCount; i++)
                    sweepValues[i] = abf.GetSweep(i);

                Console.WriteLine($"Exporting {System.IO.Path.GetFileName(pathOut)}...");

                if (pathOut.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                {
                    Export.CSV(sweepValues, pathOut, abf.SampleRate, abf.SweepStartTimes);
                }
                else if (pathOut.EndsWith(".tsv", StringComparison.OrdinalIgnoreCase))
                {
                    Export.TSV(sweepValues, pathOut, abf.SampleRate, abf.SweepStartTimes);
                }
                else if (pathOut.EndsWith(".atf", StringComparison.OrdinalIgnoreCase))
                {
                    Export.ATF(sweepValues, pathOut, abf.SampleRate, abf.SweepStartTimes);
                }
                else
                {
                    Console.WriteLine($"ERROR: output file must end with .csv, .tsv, or .atf");
                    return;
                }

                Console.WriteLine($"Conversion complete!");
            }
        }

        static void ShowHelp()
        {
            Version version = System.Reflection.Assembly.GetAssembly(typeof(ABF)).GetName().Version;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine($"AbfConvert {version.Major}.{version.Minor}");
            sb.AppendLine();
            sb.AppendLine("Purpose:");
            sb.AppendLine("  Converts ABF (ABF1 and ABF2) files to ATF, CSV, or TSV format.");
            sb.AppendLine();
            sb.AppendLine("Usage:");
            sb.AppendLine("  AbfConvert.exe input.abf output.csv");
            sb.AppendLine();
            sb.AppendLine("Supported output formats:");
            sb.AppendLine("  .csv, .tsv, .atf");
            sb.AppendLine();
            Console.WriteLine(sb); ;
        }
    }
}
